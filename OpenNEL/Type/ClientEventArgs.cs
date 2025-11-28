using System;
using System.Net.WebSockets;

namespace OpenNEL.Type;

public class ClientEventArgs(Guid clientId, WebSocket client) : EventArgs()
{
	public Guid ClientId { get; } = clientId;

	public WebSocket Client { get; } = client;
}
