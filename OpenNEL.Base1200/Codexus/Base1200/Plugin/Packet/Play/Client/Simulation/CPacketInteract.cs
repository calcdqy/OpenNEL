using Codexus.Base1200.Plugin.Event;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Manager;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Client.Simulation;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ServerBound, 16, new EnumProtocolVersion[] { EnumProtocolVersion.V1200 }, true)]
public class CPacketInteract : IPacket
{
	public int EntityId { get; set; }

	public int Type { get; set; }

	public float TargetX { get; set; }

	public float TargetY { get; set; }

	public float TargetZ { get; set; }

	public int Hand { get; set; }

	public bool Sneaking { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		EntityId = buffer.ReadVarIntFromBuffer();
		Type = buffer.ReadVarIntFromBuffer();
		if (Type == 2)
		{
			TargetX = buffer.ReadFloat();
			TargetY = buffer.ReadFloat();
			TargetZ = buffer.ReadFloat();
		}
		int type = Type;
		if ((type == 0 || type == 2) ? true : false)
		{
			Hand = buffer.ReadVarIntFromBuffer();
		}
		Sneaking = buffer.ReadBoolean();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteVarInt(EntityId);
		buffer.WriteVarInt(Type);
		if (Type == 2)
		{
			buffer.WriteFloat(TargetX);
			buffer.WriteFloat(TargetY);
			buffer.WriteFloat(TargetZ);
		}
		int type = Type;
		if ((uint)(type - 1) <= 1u)
		{
			buffer.WriteVarInt(Hand);
		}
		buffer.WriteBoolean(Sneaking);
	}

	public bool HandlePacket(GameConnection connection)
	{
		return EventManager.Instance.TriggerEvent("base_1200", new EventInteract(connection, this)).IsCancelled;
	}
}
