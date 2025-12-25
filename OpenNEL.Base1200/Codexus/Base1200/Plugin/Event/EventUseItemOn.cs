using Codexus.Base1200.Plugin.Packet.Play.Client.Simulation;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Manager;

namespace Codexus.Base1200.Plugin.Event;

public class EventUseItemOn : EventArgsBase
{
	public CPacketUseItemOn Data { get; set; }

	public EventUseItemOn(GameConnection connection, CPacketUseItemOn data): base(connection)
	{
		Data = data;
	}
}
