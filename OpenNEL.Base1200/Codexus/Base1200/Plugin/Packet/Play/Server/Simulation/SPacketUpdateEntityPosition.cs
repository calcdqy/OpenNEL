using Codexus.Base1200.Plugin.Extensions;
using Codexus.Base1200.Plugin.Utils;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Server.Simulation;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, new int[] { 43, 46 }, new EnumProtocolVersion[]
{
	EnumProtocolVersion.V1200,
	EnumProtocolVersion.V1206
}, false)]
public class SPacketUpdateEntityPosition : IPacket
{
	private int EntityId { get; set; }

	private short DeltaX { get; set; }

	private short DeltaY { get; set; }

	private short DeltaZ { get; set; }

	private bool OnGround { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		EntityId = buffer.ReadVarIntFromBuffer();
		DeltaX = buffer.ReadShort();
		DeltaY = buffer.ReadShort();
		DeltaZ = buffer.ReadShort();
		OnGround = buffer.ReadBoolean();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteVarInt(EntityId);
		buffer.WriteShort((int)DeltaX);
		buffer.WriteShort((int)DeltaY);
		buffer.WriteShort((int)DeltaZ);
		buffer.WriteBoolean(OnGround);
	}

	public bool HandlePacket(GameConnection connection)
	{
		Entity entity = connection.GetWorld()?.GetEntity(EntityId);
		if (entity == null)
		{
			return false;
		}
		entity.X += DeltaX;
		entity.Y += DeltaY;
		entity.Z += DeltaZ;
		entity.OnGround = OnGround;
		return false;
	}
}
