using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Manager;

namespace Codexus.Base1200.Plugin.Event;

public class EventSwingArm : EventArgsBase
{
	public int Hand { get; set; }

	public EventSwingArm(GameConnection connection, int hand): base(connection)
	{
		Hand = hand;
	}
}
