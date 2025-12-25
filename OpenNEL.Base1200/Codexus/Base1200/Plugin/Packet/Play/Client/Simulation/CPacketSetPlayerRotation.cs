using Codexus.Base1200.Plugin.Extensions;
using Codexus.Base1200.Plugin.Utils;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Client.Simulation;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ServerBound, new int[] { 22, 28 }, new EnumProtocolVersion[]
{
	EnumProtocolVersion.V1200,
	EnumProtocolVersion.V1206
}, false)]
public class CPacketSetPlayerRotation : IPacket
{
	private float Yaw { get; set; }

	private float Pitch { get; set; }

	private bool OnGround { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		Yaw = buffer.ReadFloat();
		Pitch = buffer.ReadFloat();
		OnGround = buffer.ReadBoolean();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteFloat(Yaw);
		buffer.WriteFloat(Pitch);
		buffer.WriteBoolean(OnGround);
	}

	public bool HandlePacket(GameConnection connection)
	{
		LocalPlayer localPlayer = connection.GetLocalPlayer();
		if (localPlayer == null)
		{
			return false;
		}
		localPlayer.Yaw = Yaw;
		localPlayer.Pitch = Pitch;
		localPlayer.OnGround = OnGround;
		return false;
	}
}
