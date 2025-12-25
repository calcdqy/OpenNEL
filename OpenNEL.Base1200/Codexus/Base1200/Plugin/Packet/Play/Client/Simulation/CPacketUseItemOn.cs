using Codexus.Base1200.Plugin.Event;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Manager;
using OpenNEL.SDK.Packet;
using OpenNEL.SDK.Utils;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Client.Simulation;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ServerBound, new int[] { 49, 56 }, new EnumProtocolVersion[]
{
	EnumProtocolVersion.V1200,
	EnumProtocolVersion.V1206
}, false)]
public class CPacketUseItemOn : IPacket
{
	public int Hand { get; set; }

	public Position Location { get; set; } = new Position(0, 0, 0);

	public int Face { get; set; }

	public float CursorPositionX { get; set; }

	public float CursorPositionY { get; set; }

	public float CursorPositionZ { get; set; }

	public bool InsideBlock { get; set; }

	public int Sequence { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		Hand = buffer.ReadVarIntFromBuffer();
		Location = buffer.ReadPosition();
		Face = buffer.ReadVarIntFromBuffer();
		CursorPositionX = buffer.ReadFloat();
		CursorPositionY = buffer.ReadFloat();
		CursorPositionZ = buffer.ReadFloat();
		InsideBlock = buffer.ReadBoolean();
		Sequence = buffer.ReadVarIntFromBuffer();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteVarInt(Hand);
		buffer.WritePosition(Location);
		buffer.WriteVarInt(Face);
		buffer.WriteFloat(CursorPositionX);
		buffer.WriteFloat(CursorPositionY);
		buffer.WriteFloat(CursorPositionZ);
		buffer.WriteBoolean(InsideBlock);
		buffer.WriteVarInt(Sequence);
	}

	public bool HandlePacket(GameConnection connection)
	{
		return EventManager.Instance.TriggerEvent("base_1200", new EventUseItemOn(connection, this)).IsCancelled;
	}
}
