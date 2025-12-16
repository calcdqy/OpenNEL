using System;
using System.Linq;
using OpenNEL.Entities;
using OpenNEL.type;
using OpenNEL.Manager;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Codexus.Cipher.Entities;
using Codexus.Cipher.Entities.WPFLauncher.NetGame;
using Codexus.Cipher.Protocol;
using Codexus.Game.Launcher.Services.Java;
using Codexus.Game.Launcher.Utils;
using Codexus.Interceptors;
using Codexus.OpenSDK;
using OpenNEL.Entities.Web.NetGame;
using OpenNEL.Utils;
using Serilog;

namespace OpenNEL_WinUI.Handlers.Game;

public class JoinGame
{
    private EntityJoinGame? _request;
    private string _lastIp;
    private int _lastPort;

    public async Task<object> Execute(EntityJoinGame request)
    {
        _request = request;
        var serverId = _request.ServerId;
        var serverName = _request.ServerName;
        var role = _request.Role;
        var last = UserManager.Instance.GetLastAvailableUser();
        if (last == null) return new { type = "notlogin" };
        if (string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(role))
        {
            return new { type = "start_error", message = "参数错误" };
        }
        try
        {
            var ok = await StartAsync(serverId!, serverName, role!);
            if (!ok) return new { type = "start_error", message = "启动失败" };
            return new { type = "channels_updated", ip = _lastIp, port = _lastPort };
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, "启动失败");
            return new { type = "start_error", message = "启动失败" };
        }
    }

    public async Task<bool> StartAsync(string serverId, string serverName, string roleId)
    {
        var available = UserManager.Instance.GetLastAvailableUser();
        if (available == null) return false;
        var entityId = available.UserId;
        var roles = AppState.X19.QueryNetGameCharacters(entityId, available.AccessToken, serverId);
        var selected = roles.Data.FirstOrDefault(r => r.Name == roleId);
        if (selected == null) return false;
        var details = AppState.X19.QueryNetGameDetailById(entityId, available.AccessToken, serverId);
        var address = AppState.X19.GetNetGameServerAddress(entityId, available.AccessToken, serverId);
        var version = details.Data!.McVersionList[0];
        var gameVersion = GameVersionUtil.GetEnumFromGameVersion(version.Name);
        var serverModInfo = await InstallerService.InstallGameMods(
            entityId,
            available.AccessToken,
            gameVersion,
            AppState.X19,
            serverId,
            false);
        var mods = JsonSerializer.Serialize(serverModInfo);
        SemaphoreSlim authorizedSignal = new SemaphoreSlim(0);
        var pair = Md5Mapping.GetMd5FromGameVersion(version.Name);

        _lastIp = address.Data!.Ip;
        _lastPort = address.Data!.Port;
        var socksCfg = _request.Socks5;
        var socksAddr = socksCfg != null ? (socksCfg.Address ?? string.Empty) : string.Empty;
        var socksPort = socksCfg != null ? socksCfg.Port : 0;
        if (!string.IsNullOrWhiteSpace(socksAddr) && socksPort <= 0) return false;
        if (!string.IsNullOrWhiteSpace(socksAddr) && socksPort > 0)
        {
            try { Dns.GetHostAddresses(socksAddr); }
            catch { return false; }
        }
        Interceptor interceptor = Interceptor.CreateInterceptor(_request.Socks5, mods, serverId, serverName, version.Name, address.Data!.Ip, address.Data!.Port, _request.Role, available.UserId, available.AccessToken, delegate(string certification)
        {
            Log.Logger.Information("Server certification: {Certification}", certification);
            Task.Run(async delegate
            {
                try
                {
                    var latest = UserManager.Instance.GetAvailableUser(entityId);
                    var currentToken = latest?.AccessToken ?? available.AccessToken;
                    var success = await AppState.Services!.Yggdrasil.JoinServerAsync(new Codexus.OpenSDK.Entities.Yggdrasil.GameProfile
                    {
                        GameId = serverId,
                        GameVersion = version.Name,
                        BootstrapMd5 = pair.BootstrapMd5,
                        DatFileMd5 = pair.DatFileMd5,
                        Mods = JsonSerializer.Deserialize<Codexus.OpenSDK.Entities.Yggdrasil.ModList>(mods)!,
                        User = new Codexus.OpenSDK.Entities.Yggdrasil.UserProfile { UserId = int.Parse(entityId), UserToken = currentToken }
                    }, certification);
                    if (success.IsSuccess) if (AppState.Debug) Log.Information("消息认证成功");
                        else
                        {
                            if (AppState.Debug)Log.Error(new Exception(success.Error ?? "未知错误"), "消息认证失败，详细信息: {Error}", success.Error);
                            else Log.Error("消息认证失败: {Error}", success.Error);
                        }
                }
                catch (Exception e)
                {
                    Log.Error(e, "认证过程中发生异常");
                }
                finally
                {
                    authorizedSignal.Release();
                }
            });
            authorizedSignal.Wait();
        });
        InterConn.GameStart(available.UserId, available.AccessToken, _request.GameId).GetAwaiter().GetResult();
        GameManager.Instance.AddInterceptor(interceptor);
        _lastIp = interceptor.LocalAddress;
        _lastPort = interceptor.LocalPort;

        await X19.InterconnectionApi.GameStartAsync(entityId, available.AccessToken, serverId);
        return true;
    }
}
