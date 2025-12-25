using Codexus.Base1200.Plugin.Packet.Play.Client.Simulation;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Manager;

namespace Codexus.Base1200.Plugin.Event;

public class EventUseItem : EventArgsBase
{
	public CPacketUseItem Data { get; set; }

	public EventUseItem(GameConnection connection, CPacketUseItem data): base(connection)
	{
		Data = data;
	}
}
