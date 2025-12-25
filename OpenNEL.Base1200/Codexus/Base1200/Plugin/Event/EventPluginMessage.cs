using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Manager;

namespace Codexus.Base1200.Plugin.Event;

public class EventPluginMessage : EventArgsBase
{
	public EnumPacketDirection Direction { get; set; }

	public string Identifier { get; set; }

	public byte[] Payload { get; set; }

	public EventPluginMessage(GameConnection connection, EnumPacketDirection direction, string identifier, byte[] payload): base(connection)
	{
		Direction = direction;
		Identifier = identifier;
		Payload = payload;
	}
}
