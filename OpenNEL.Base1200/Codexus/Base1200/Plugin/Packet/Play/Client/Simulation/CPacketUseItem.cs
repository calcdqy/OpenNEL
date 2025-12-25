using Codexus.Base1200.Plugin.Event;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Manager;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Client.Simulation;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ServerBound, new int[] { 50, 57 }, new EnumProtocolVersion[]
{
	EnumProtocolVersion.V1200,
	EnumProtocolVersion.V1206
}, false)]
public class CPacketUseItem : IPacket
{
	public int Hand { get; set; }

	public int Sequence { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		Hand = buffer.ReadVarIntFromBuffer();
		Sequence = buffer.ReadVarIntFromBuffer();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteVarInt(Hand);
		buffer.WriteVarInt(Sequence);
	}

	public bool HandlePacket(GameConnection connection)
	{
		return EventManager.Instance.TriggerEvent("base_1200", new EventUseItem(connection, this)).IsCancelled;
	}
}
