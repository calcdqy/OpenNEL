using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Manager;

namespace Codexus.Base1200.Plugin.Event;

public class EventGameJoin : EventArgsBase
{
	public EventGameJoin(GameConnection connection)
		: base(connection)
	{
	}
}
