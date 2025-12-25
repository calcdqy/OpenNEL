using Codexus.Base1200.Plugin.Packet.Play.Client.Simulation;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Manager;

namespace Codexus.Base1200.Plugin.Event;

public class EventInteract : EventArgsBase
{
	public CPacketInteract Packet { get; set; }

	public EventInteract(GameConnection connection, CPacketInteract packet): base(connection)
	{
		Packet = packet;
	}
}
