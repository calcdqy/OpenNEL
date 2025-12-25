using Codexus.Base1200.Plugin.Extensions;
using Codexus.Base1200.Plugin.Utils;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Server.Simulation;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, 3, new EnumProtocolVersion[] { EnumProtocolVersion.V1200 }, false)]
public class SPacketSpawnPlayer : IPacket
{
	private int EntityId { get; set; }

	private byte[] PlayerGuid { get; } = new byte[16];

	private double X { get; set; }

	private double Y { get; set; }

	private double Z { get; set; }

	private byte Yaw { get; set; }

	private byte Pitch { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		EntityId = buffer.ReadVarIntFromBuffer();
		buffer.ReadBytes(PlayerGuid);
		X = buffer.ReadDouble();
		Y = buffer.ReadDouble();
		Z = buffer.ReadDouble();
		Yaw = buffer.ReadByte();
		Pitch = buffer.ReadByte();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteVarInt(EntityId);
		buffer.WriteBytes(PlayerGuid);
		buffer.WriteDouble(X);
		buffer.WriteDouble(Y);
		buffer.WriteDouble(Z);
		buffer.WriteByte((int)Pitch);
		buffer.WriteByte((int)Yaw);
	}

	public bool HandlePacket(GameConnection connection)
	{
		connection.GetWorld()?.AddEntity(new Player
		{
			EntityId = EntityId,
			EntityGuid = PlayerGuid,
			X = X,
			Y = Y,
			Z = Z,
			Yaw = (float)(int)Yaw * 360f / 256f,
			Pitch = (float)(int)Pitch * 360f / 256f,
			OnGround = false
		});
		return false;
	}
}
