/*
<OpenNEL>
Copyright (C) <2025>  <OpenNEL>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Codexus.OpenSDK;
using OpenNEL.GameLauncher.Entities;
using OpenNEL.GameLauncher.Services.Java;
using OpenNEL.GameLauncher.Utils;
using OpenNEL.Interceptors;
using OpenNEL.SDK.Entities;
using OpenNEL.WPFLauncher;
using OpenNEL.WPFLauncher.Entities;
using OpenNEL.WPFLauncher.Entities.NetGame.Texture;
using OpenNEL_WinUI.Manager;
using OpenNEL_WinUI.type;
using OpenNEL_WinUI.Utils;
using Serilog;

namespace OpenNEL_WinUI.Handlers.Game;

public class LaunchWhiteGame
{
    private readonly IProgress<EntityProgressUpdate>? _progress;

    public LaunchWhiteGame(IProgress<EntityProgressUpdate>? progress = null)
    {
        _progress = progress;
    }

    public async Task<object> Execute(string accountId, string serverId, string serverName, string roleId)
    {
        if (string.IsNullOrWhiteSpace(accountId) || string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(roleId))
        {
            return new { type = "start_error", message = "参数错误" };
        }

        var available = UserManager.Instance.GetAvailableUser(accountId);
        if (available == null)
        {
            return new { type = "notlogin" };
        }

        try
        {
            PathUtil.EnsureDirectoriesExist();
            var result = await LaunchAsync(available.UserId, available.AccessToken, serverId, serverName, roleId);
            return result ? new { type = "launch_success" } : new { type = "start_error", message = "启动失败" };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "白端启动失败");
            return new { type = "start_error", message = ex.Message };
        }
    }

    private async Task<bool> LaunchAsync(string userId, string accessToken, string serverId, string serverName, string roleId)
    {
        Log.Debug("白端启动: userId={UserId}, serverId={ServerId}, role={Role}", userId, serverId, roleId);

        var wpf = new WPFLauncherClient();
        var details = AppState.X19.QueryNetGameDetailById(userId, accessToken, serverId);
        var address = AppState.X19.GetNetGameServerAddress(userId, accessToken, serverId);
        var version = details.Data!.McVersionList[0];
        var gameVersion = GameVersionUtil.GetEnumFromGameVersion(version.Name);

        Log.Debug("白端启动: 游戏版本={Version}, IP={Ip}, Port={Port}", version.Name, address.Data!.Ip, address.Data!.Port);

        ReportProgress(10, "正在安装游戏模组");
        var serverMod = await InstallerService.InstallGameMods(userId, accessToken, gameVersion, wpf, serverId, false);
        var mods = JsonSerializer.Serialize(serverMod);
        var pair = Md5Mapping.GetMd5FromGameVersion(version.Name);

        ReportProgress(30, "正在创建代理通道");
        SemaphoreSlim authorizedSignal = new SemaphoreSlim(0);
        var interceptor = Interceptor.CreateInterceptor(
            new EntitySocks5(),
            mods,
            serverId,
            serverName,
            version.Name,
            address.Data!.Ip,
            address.Data!.Port,
            roleId,
            userId,
            accessToken,
            delegate(string certification)
            {
                Task.Run(async delegate
                {
                    try
                    {
                        var latest = UserManager.Instance.GetAvailableUser(userId);
                        var currentToken = latest?.AccessToken ?? accessToken;
                        var success = await AppState.Services!.Yggdrasil.JoinServerAsync(new Codexus.OpenSDK.Entities.Yggdrasil.GameProfile
                        {
                            GameId = serverId,
                            GameVersion = version.Name,
                            BootstrapMd5 = pair.BootstrapMd5,
                            DatFileMd5 = pair.DatFileMd5,
                            Mods = JsonSerializer.Deserialize<Codexus.OpenSDK.Entities.Yggdrasil.ModList>(mods)!,
                            User = new Codexus.OpenSDK.Entities.Yggdrasil.UserProfile { UserId = int.Parse(userId), UserToken = currentToken }
                        }, certification);
                        if (success.IsSuccess)
                        {
                            Log.Information("白端启动: 服务器认证成功");
                        }
                        else
                        {
                            Log.Error("白端启动: 服务器认证失败: {Error}", success.Error);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "白端启动: 认证过程中发生异常");
                    }
                    finally
                    {
                        authorizedSignal.Release();
                    }
                });
                authorizedSignal.Wait();
            });
        GameManager.Instance.AddInterceptor(interceptor);

        ReportProgress(50, "正在启动游戏");
        var launchRequest = new EntityLaunchGame
        {
            GameName = serverName,
            GameId = serverId,
            RoleName = roleId,
            UserId = userId,
            ClientType = EnumGameClientType.Java,
            GameType = EnumGType.ServerGame,
            GameVersionId = (int)gameVersion,
            GameVersion = version.Name,
            AccessToken = accessToken,
            ServerIp = interceptor.LocalAddress,
            ServerPort = interceptor.LocalPort,
            MaxGameMemory = 4096,
            LoadCoreMods = true
        };

        // 创建并启动 Launcher
        var launcher = LauncherService.CreateLauncher(launchRequest, accessToken, wpf, wpf.MPay.GameVersion, _progress ?? new Progress<EntityProgressUpdate>());
        GameManager.Instance.AddLauncher(launcher);
        
        launcher.Exited += (guid) =>
        {
            GameManager.Instance.ShutdownInterceptor(interceptor.Identifier);
        };

        await InterConnClient.GameStart(userId, accessToken, serverId);
        await X19.InterconnectionApi.GameStartAsync(userId, accessToken, serverId);

        ReportProgress(100, "启动完成");
        Log.Information("白端启动成功: ServerId={ServerId}, Role={Role}, LocalAddress={Addr}:{Port}", serverId, roleId, interceptor.LocalAddress, interceptor.LocalPort);
        return true;
    }

    private void ReportProgress(int percent, string message)
    {
        _progress?.Report(new EntityProgressUpdate
        {
            Id = Guid.Empty,
            Percent = percent,
            Message = message
        });
    }
}
