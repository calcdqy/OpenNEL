using Codexus.Base1200.Plugin.Extensions;
using Codexus.Base1200.Plugin.Utils;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Server.Simulation;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, new int[] { 45, 48 }, new EnumProtocolVersion[]
{
	EnumProtocolVersion.V1200,
	EnumProtocolVersion.V1206
}, false)]
public class SPacketUpdateEntityRotation : IPacket
{
	private int EntityId { get; set; }

	private byte Yaw { get; set; }

	private byte Pitch { get; set; }

	private bool OnGround { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		EntityId = buffer.ReadVarIntFromBuffer();
		Yaw = buffer.ReadByte();
		Pitch = buffer.ReadByte();
		OnGround = buffer.ReadBoolean();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteVarInt(EntityId);
		buffer.WriteByte((int)Yaw);
		buffer.WriteByte((int)Pitch);
		buffer.WriteBoolean(OnGround);
	}

	public bool HandlePacket(GameConnection connection)
	{
		Entity entity = connection.GetWorld()?.GetEntity(EntityId);
		if (entity == null)
		{
			return false;
		}
		entity.Yaw = (float)(Yaw * 360) / 256f;
		entity.Pitch = (float)(Pitch * 360) / 256f;
		entity.OnGround = OnGround;
		return false;
	}
}
