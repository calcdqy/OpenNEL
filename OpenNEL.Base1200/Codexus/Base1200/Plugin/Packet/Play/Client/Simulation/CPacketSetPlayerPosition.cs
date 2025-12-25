using Codexus.Base1200.Plugin.Extensions;
using Codexus.Base1200.Plugin.Utils;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Client.Simulation;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ServerBound, new int[] { 20, 26 }, new EnumProtocolVersion[]
{
	EnumProtocolVersion.V1200,
	EnumProtocolVersion.V1206
}, false)]
public class CPacketSetPlayerPosition : IPacket
{
	private double X { get; set; }

	private double Y { get; set; }

	private double Z { get; set; }

	private bool OnGround { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		X = buffer.ReadDouble();
		Y = buffer.ReadDouble();
		Z = buffer.ReadDouble();
		OnGround = buffer.ReadBoolean();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteDouble(X);
		buffer.WriteDouble(Y);
		buffer.WriteDouble(Z);
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
		localPlayer.OnGround = OnGround;
		return false;
	}
}
