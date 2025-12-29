using DotNetty.Buffers;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Event;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Manager;
using OpenNEL.SDK.Utils;
using Serilog;
using System.Collections.Concurrent;

namespace OpenNEL.IRC;

public static class IrcEventHandler
{
    static readonly ConcurrentDictionary<GameConnection, bool> _processed = new();

    public static void Register(Func<string> tokenProvider, string hwid)
    {
        IrcManager.TokenProvider = tokenProvider;
        IrcManager.Hwid = hwid;
        IrcManager.OnClientRemoved = conn => _processed.TryRemove(conn, out _);

        foreach (var channel in MessageChannels.AllVersions)
        {
            EventManager.Instance.RegisterHandler<EventLoginSuccess>(channel, OnLoginSuccess);
        }

        EventManager.Instance.RegisterHandler<EventConnectionClosed>("channel_connection", OnConnectionClosed);
    }

    static void OnLoginSuccess(EventLoginSuccess args)
    {
        var nickName = args.Connection.NickName;
        if (string.IsNullOrEmpty(nickName)) return;

        if (!_processed.TryAdd(args.Connection, true)) return;

        var client = IrcManager.GetOrCreate(args.Connection);
        client.ChatReceived += OnChatReceived;
        client.Start(nickName);
    }

    static void OnConnectionClosed(EventConnectionClosed args)
    {
        IrcManager.Remove(args.Connection);
    }

    static void OnChatReceived(object? sender, IrcChatEventArgs e)
    {
        try
        {
            if (sender is not IrcClient client) return;
            var conn = client.Connection;
            if (conn?.ClientChannel == null) return;

            var text = $"§b[OpenNEL {e.Username}]§r <{e.PlayerName}> {e.Message}";
            var buffer = Unpooled.Buffer();

            buffer.WriteVarInt(108);

            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            buffer.WriteByte(0x08);
            buffer.WriteShort(bytes.Length);
            buffer.WriteBytes(bytes);
            buffer.WriteBoolean(false);

            conn.ClientChannel.WriteAndFlushAsync(buffer);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[IRC] 显示消息失败");
        }
    }
}
