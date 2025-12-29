using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenNEL.SDK.Connection;
using Serilog;

namespace OpenNEL.IRC;

public class IrcChatEventArgs : EventArgs
{
    public string Username { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public static class IrcManager
{
    static readonly ConcurrentDictionary<GameConnection, IrcClient> _clients = new();
    
    public static Func<string>? TokenProvider { get; set; }
    public static string Hwid { get; set; } = string.Empty;
    public static Action<GameConnection>? OnClientRemoved { get; set; }

    public static IrcClient GetOrCreate(GameConnection conn)
    {
        return _clients.GetOrAdd(conn, c => new IrcClient(c, TokenProvider, Hwid));
    }

    public static IrcClient? Get(GameConnection conn)
    {
        return _clients.TryGetValue(conn, out var client) ? client : null;
    }

    public static void Remove(GameConnection conn)
    {
        if (_clients.TryRemove(conn, out var client))
        {
            client.Stop();
            OnClientRemoved?.Invoke(conn);
            Log.Information("[IRC] 已移除: {Id}", conn.GameId);
        }
    }

    public static Dictionary<string, string> GetAllOnlinePlayers()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var client in _clients.Values)
        {
            foreach (var kv in client.Players)
            {
                result[kv.Key] = kv.Value;
            }
        }
        return result;
    }
}

public class IrcClient : IDisposable
{
    const string HOST = "api.fandmc.cn";
    const int PORT = 9527;
    const int HEARTBEAT_INTERVAL = 30000;

    readonly GameConnection _conn;
    readonly Func<string>? _tokenProvider;
    readonly string _hwid;
    
    TcpClient? _tcpClient;
    NetworkStream? _stream;
    StreamReader? _reader;
    StreamWriter? _writer;
    Thread? _listenerThread;
    Timer? _heartbeatTimer;
    
    readonly object _lock = new();
    readonly ManualResetEventSlim _responseEvent = new(false);
    readonly ConcurrentDictionary<string, string> _players = new(StringComparer.OrdinalIgnoreCase);
    
    volatile bool _connected;
    volatile bool _running;
    string? _response;
    string? _playerName;

    public IReadOnlyDictionary<string, string> Players => _players;
    public event EventHandler<IrcChatEventArgs>? ChatReceived;
    public GameConnection Connection => _conn;
    public string ServerId => _conn.GameId;
    string Token => _tokenProvider?.Invoke() ?? string.Empty;

    public IrcClient(GameConnection conn, Func<string>? tokenProvider, string hwid)
    {
        _conn = conn;
        _tokenProvider = tokenProvider;
        _hwid = hwid;
    }

    public void Start(string playerName)
    {
        if (_running) return;
        _running = true;
        _playerName = playerName;

        if (Connect())
        {
            ReportPlayer(playerName);
        }

        _listenerThread = new Thread(ListenLoop) { IsBackground = true, Name = $"IRC-{ServerId}" };
        _listenerThread.Start();

        _heartbeatTimer = new Timer(_ => Heartbeat(), null, HEARTBEAT_INTERVAL, HEARTBEAT_INTERVAL);
        
        Log.Information("[IRC] 启动: {Id}, 玩家: {Name}", ServerId, playerName);
    }

    public void Stop()
    {
        if (!_running) return;
        _running = false;
        
        _heartbeatTimer?.Dispose();
        _heartbeatTimer = null;
        
        Disconnect();
        _players.Clear();
        
        Log.Information("[IRC] 停止: {Id}", ServerId);
    }

    bool Connect()
    {
        lock (_lock)
        {
            if (_connected) return true;
            
            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.Connect(HOST, PORT);
                _stream = _tcpClient.GetStream();
                _reader = new StreamReader(_stream, Encoding.UTF8);
                _writer = new StreamWriter(_stream, new UTF8Encoding(false)) { AutoFlush = true }; // 不带 BOM
                _connected = true;
                Log.Information("[IRC:{Id}] 已连接", ServerId);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[IRC:{Id}] 连接失败", ServerId);
                _connected = false;
                return false;
            }
        }
    }

    void Disconnect()
    {
        lock (_lock)
        {
            _connected = false;
            try { _writer?.Dispose(); } catch { }
            try { _reader?.Dispose(); } catch { }
            try { _stream?.Dispose(); } catch { }
            try { _tcpClient?.Dispose(); } catch { }
            _writer = null;
            _reader = null;
            _stream = null;
            _tcpClient = null;
        }
    }

    void Reconnect()
    {
        if (!_running) return;
        
        Log.Warning("[IRC:{Id}] 重连...", ServerId);
        Disconnect();
        Thread.Sleep(3000);
        
        if (Connect() && !string.IsNullOrEmpty(_playerName))
        {
            ReportPlayer(_playerName);
        }
    }

    void Heartbeat()
    {
        if (!_running) return;
        
        try
        {
            RefreshPlayers();
        }
        catch
        {
            if (_running) Reconnect();
        }
    }

    void ListenLoop()
    {
        while (_running)
        {
            try
            {
                if (!_connected || _reader == null)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                string? line;
                try
                {
                    line = _reader.ReadLine();
                }
                catch
                {
                    if (_running) Reconnect();
                    continue;
                }

                if (string.IsNullOrEmpty(line)) continue;
                
                ProcessMessage(line);
            }
            catch (Exception ex)
            {
                if (_running) Log.Warning(ex, "[IRC:{Id}] 监听异常", ServerId);
                Thread.Sleep(1000);
            }
        }
    }

    void ProcessMessage(string line)
    {
        var parts = line.Split('|');
        if (parts.Length < 2) return;

        switch (parts[0])
        {
            case "CHAT" when parts.Length >= 4:
                Log.Information("[IRC:{Id}] 收到消息: {User} <{Player}> {Msg}", 
                    ServerId, parts[1], parts[2], string.Join("|", parts.Skip(3)));
                ChatReceived?.Invoke(this, new IrcChatEventArgs
                {
                    Username = parts[1],
                    PlayerName = parts[2],
                    Message = string.Join("|", parts.Skip(3))
                });
                break;
                
            case "OK":
            case "ERR":
                _response = line;
                _responseEvent.Set();
                break;
        }
    }

    string? Send(string command, bool waitResponse = true)
    {
        lock (_lock)
        {
            if (!_connected || _writer == null)
            {
                if (!Connect()) return null;
            }

            try
            {
                if (waitResponse)
                {
                    _responseEvent.Reset();
                    _response = null;
                }
                
                _writer!.WriteLine(command);
                
                if (!waitResponse) return "OK";
                
                return _responseEvent.Wait(5000) ? _response : null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[IRC:{Id}] 发送失败", ServerId);
                _connected = false;
                return null;
            }
        }
    }

    public bool ReportPlayer(string name)
    {
        var response = Send($"REPORT|{Token}|{_hwid}|{ServerId}|{name}");
        if (response?.StartsWith("OK|") == true)
        {
            _playerName = name;
            Log.Information("[IRC:{Id}] 上报成功: {Name}", ServerId, name);
            return true;
        }
        Log.Warning("[IRC:{Id}] 上报失败: {Name}, 响应: {Resp}", ServerId, name, response);
        return false;
    }

    public void SendChat(string playerName, string message)
    {
        Send($"CHAT|{Token}|{_hwid}|{ServerId}|{playerName}|{message}", false);
        Log.Information("[IRC:{Id}] 发送: <{Name}> {Msg}", ServerId, playerName, message);
    }

    public bool RefreshPlayers()
    {
        var response = Send($"GET|{Token}|{_hwid}|{ServerId}|");
        if (response?.StartsWith("OK|") != true) return false;

        try
        {
            var json = response[3..];
            var list = JsonSerializer.Deserialize<PlayerInfo[]>(json);
            _players.Clear();
            if (list != null)
            {
                foreach (var p in list)
                {
                    _players[p.PlayerName] = p.Username;
                }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose() => Stop();

    class PlayerInfo
    {
        [JsonPropertyName("Username")] public string Username { get; set; } = string.Empty;
        [JsonPropertyName("PlayerName")] public string PlayerName { get; set; } = string.Empty;
    }
}
