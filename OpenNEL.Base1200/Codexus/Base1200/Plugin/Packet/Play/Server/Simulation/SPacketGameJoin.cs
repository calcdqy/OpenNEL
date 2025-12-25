using System;
using Codexus.Base1200.Plugin.Event;
using Codexus.Base1200.Plugin.Extensions;
using Codexus.Base1200.Plugin.Utils;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Manager;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Server.Simulation;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, new int[] { 40, 43 }, new EnumProtocolVersion[]
{
	EnumProtocolVersion.V1200,
	EnumProtocolVersion.V1206
}, false)]
public class SPacketGameJoin : IPacket
{
	private int PlayerId;

	private byte[] Payload { get; set; } = Array.Empty<byte>();

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		PlayerId = buffer.ReadInt();
		Payload = buffer.ReadByteArrayReadableBytes();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteInt(PlayerId);
		buffer.WriteBytes(Payload);
		Payload = Array.Empty<byte>();
	}

	public bool HandlePacket(GameConnection connection)
	{
		connection.SetLocalPlayer(new LocalPlayer(PlayerId));
		connection.SetWorld(new World());
		connection.GetTeams().Clear();
		return EventManager.Instance.TriggerEvent("base_1200", new EventGameJoin(connection)).IsCancelled;
	}
}
