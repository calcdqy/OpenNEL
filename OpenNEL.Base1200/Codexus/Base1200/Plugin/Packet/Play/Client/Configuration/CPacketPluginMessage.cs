using System;
using Codexus.Base1200.Plugin.Event;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Manager;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Client.Configuration;

[RegisterPacket(EnumConnectionState.Configuration, EnumPacketDirection.ServerBound, 2, EnumProtocolVersion.V1206, false)]
public class CPacketPluginMessage : IPacket
{
	public string Identifier { get; set; } = string.Empty;

	public byte[] Payload { get; set; } = Array.Empty<byte>();

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		Identifier = buffer.ReadStringFromBuffer(32);
		Payload = buffer.ReadByteArrayReadableBytes();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteStringToBuffer(Identifier);
		buffer.WriteBytes(Payload);
	}

	public bool HandlePacket(GameConnection connection)
	{
		EventPluginMessage eventPluginMessage = EventManager.Instance.TriggerEvent("base_1200", new EventPluginMessage(connection, EnumPacketDirection.ServerBound, Identifier, Payload));
		Identifier = eventPluginMessage.Identifier;
		Payload = eventPluginMessage.Payload;
		return eventPluginMessage.IsCancelled;
	}
}
