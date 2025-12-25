using Codexus.Base1200.Plugin.Utils;
using Codexus.Base1200.Plugin.Utils.Patch;
using OpenNEL.SDK.Connection;

namespace Codexus.Base1200.Plugin.Extensions;

public static class AttributeExtensions
{
	public static void SetLocalPlayer(this GameConnection connection, LocalPlayer player)
	{
		MinecraftAttribute.SetLocalPlayer(connection, player);
	}

	public static LocalPlayer? GetLocalPlayer(this GameConnection connection)
	{
		return MinecraftAttribute.GetLocalPlayer(connection);
	}

	public static World? GetWorld(this GameConnection connection)
	{
		return MinecraftAttribute.GetWorld(connection);
	}

	public static void SetWorld(this GameConnection connection, World world)
	{
		MinecraftAttribute.SetWorld(connection, world);
	}

	public static Teams GetTeams(this GameConnection connection)
	{
		return MinecraftAttribute.GetTeams(connection);
	}
}
