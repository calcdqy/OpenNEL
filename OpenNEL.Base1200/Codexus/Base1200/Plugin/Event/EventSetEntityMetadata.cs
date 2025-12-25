using Codexus.Base1200.Plugin.Packet.Play.Server.Patch;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Manager;

namespace Codexus.Base1200.Plugin.Event;

public class EventSetEntityMetadata : EventArgsBase
{
	private readonly SSetEntityMetadata _packet;

	public SSetEntityMetadata Packet => _packet;

	public EventSetEntityMetadata(GameConnection connection, SSetEntityMetadata packet): base(connection)
	{
		_packet = packet;
	}
}
