using System;
using System.Net.WebSockets;

namespace OpenNEL.Type;

public class MessageReceivedEventArgs(Guid clientId, WebSocket client, byte[] message, WebSocketMessageType messageType, Action<byte[]> addSendQueue) : EventArgs()
{
	public Guid ClientId { get; } = clientId;

	public WebSocket Client { get; } = client;

	public byte[] Message { get; } = message;

	public WebSocketMessageType MessageType { get; } = messageType;

	public Action<byte[]> AddSendQueue { get; } = addSendQueue;
}
