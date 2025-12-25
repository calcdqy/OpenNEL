using Codexus.Base1200.Plugin.Extensions;
using Codexus.Base1200.Plugin.Utils;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Client.Simulation;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ServerBound, new int[] { 21, 27 }, new EnumProtocolVersion[]
{
	EnumProtocolVersion.V1200,
	EnumProtocolVersion.V1206
}, false)]
public class CPacketSetPlayerPositionAndRotation : IPacket
{
	private double X { get; set; }

	private double Y { get; set; }

	private double Z { get; set; }

	private float Yaw { get; set; }

	private float Pitch { get; set; }

	private bool OnGround { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		X = buffer.ReadDouble();
		Y = buffer.ReadDouble();
		Z = buffer.ReadDouble();
		Yaw = buffer.ReadFloat();
		Pitch = buffer.ReadFloat();
		OnGround = buffer.ReadBoolean();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteDouble(X);
		buffer.WriteDouble(Y);
		buffer.WriteDouble(Z);
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
		localPlayer.X = X;
		localPlayer.Y = Y;
		localPlayer.Z = Z;
		localPlayer.Yaw = Yaw;
		localPlayer.Pitch = Pitch;
		localPlayer.OnGround = OnGround;
		return false;
	}
}
