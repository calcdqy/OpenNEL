using OpenNEL.HandleWebSocket.Game;
using OpenNEL.HandleWebSocket.Login;
using OpenNEL.HandleWebSocket.Plugin;
using OpenNEL.HandleWebSocket.Connected;
using OpenNEL.network;

namespace OpenNEL.HandleWebSocket;

internal static class HandlerFactory
{
    private static readonly Dictionary<string, IWsHandler> Map;

    static HandlerFactory()
    {
        var login = new LoginMessage();
        var handlers = new IWsHandler[]
        {
            login,
            new CookieLoginHandler(),
            new DeleteAccountHandler(),
            new DeleteUserMessage(),
            new GetAccountMessage(),
            new ListAccountsHandler(),
            new SelectAccountHandler(),
            new SearchServersHandler(),
            new ListServersHandler(),
            new OpenServerHandler(),
            new CreateRoleNamedHandler(),
            new JoinGameHandler(),
            new ListChannelsHandler(),
            new ShutdownGameHandler(),
            new GetFreeAccountHandler(),
            new ListInstalledPluginsHandler(),
            new UninstallPluginHandler(),
            new RestartGatewayHandler(),
            new InstallPluginHandler(),
            new UpdatePluginHandler(),
            new ListAvailablePluginsHandler(),
            new QueryGameSessionHandler()
        };
        Map = handlers.ToDictionary(h => h.Type, h => h);
        Map["login_4399"] = login;
        Map["login_x19"] = login;
    }

    public static IWsHandler? Get(string type)
    {
        return Map.TryGetValue(type, out var h) ? h : null;
    }
}
