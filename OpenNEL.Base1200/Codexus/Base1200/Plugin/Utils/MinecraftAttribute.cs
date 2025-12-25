using Codexus.Base1200.Plugin.Utils.Patch;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Extensions;
using DotNetty.Common.Utilities;

namespace Codexus.Base1200.Plugin.Utils;

public static class MinecraftAttribute
{
	private static readonly AttributeKey<LocalPlayer?> Player = AttributeKey<LocalPlayer>.ValueOf("minecraft:player");

	private static readonly AttributeKey<World?> World = AttributeKey<World>.ValueOf("minecraft:world");

	private static readonly AttributeKey<Teams?> Teams = AttributeKey<Teams>.ValueOf("minecraft:teams");

	public static LocalPlayer? GetLocalPlayer(GameConnection connection)
	{
		return connection.ClientChannel.GetAttribute<LocalPlayer>(Player).GetOrDefault(() => null);
	}

	public static void SetLocalPlayer(GameConnection connection, LocalPlayer player)
	{
		connection.ClientChannel.GetAttribute<LocalPlayer>(Player).Set(player);
	}

	public static World? GetWorld(GameConnection connection)
	{
		return connection.ClientChannel.GetAttribute<World>(World).GetOrDefault(() => null);
	}

	public static void SetWorld(GameConnection connection, World world)
	{
		connection.ClientChannel.GetAttribute<World>(World).Set(world);
	}

	public static Teams GetTeams(GameConnection connection)
	{
		return connection.ClientChannel.GetAttribute<Teams>(Teams).GetOrDefault(() => new Teams());
	}
}
