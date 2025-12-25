using System;
using Codexus.Base1200.Plugin.Extensions;
using Codexus.Base1200.Plugin.Utils;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Server.Simulation;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, 1, new EnumProtocolVersion[]
{
	EnumProtocolVersion.V1200,
	EnumProtocolVersion.V1206
}, false)]
public class SPacketSpawnEntity : IPacket
{
	private int EntityId { get; set; }

	private byte[] EntityGuid { get; } = new byte[16];

	private int Type { get; set; }

	private double X { get; set; }

	private double Y { get; set; }

	private double Z { get; set; }

	private byte Pitch { get; set; }

	private byte Yaw { get; set; }

	private byte[] Data { get; set; } = Array.Empty<byte>();

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		EntityId = buffer.ReadVarIntFromBuffer();
		buffer.ReadBytes(EntityGuid);
		Type = buffer.ReadVarIntFromBuffer();
		X = buffer.ReadDouble();
		Y = buffer.ReadDouble();
		Z = buffer.ReadDouble();
		Pitch = buffer.ReadByte();
		Yaw = buffer.ReadByte();
		Data = buffer.ReadByteArrayReadableBytes();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteVarInt(EntityId);
		buffer.WriteBytes(EntityGuid);
		buffer.WriteVarInt(Type);
		buffer.WriteDouble(X);
		buffer.WriteDouble(Y);
		buffer.WriteDouble(Z);
		buffer.WriteByte((int)Pitch);
		buffer.WriteByte((int)Yaw);
		buffer.WriteBytes(Data);
	}

	public bool HandlePacket(GameConnection connection)
	{
		connection.GetWorld()?.AddEntity(new Entity
		{
			EntityId = EntityId,
			EntityGuid = EntityGuid,
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
