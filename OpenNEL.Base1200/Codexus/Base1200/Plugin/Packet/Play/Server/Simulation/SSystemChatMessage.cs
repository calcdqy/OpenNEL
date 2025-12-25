using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Server.Simulation;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, 100, new EnumProtocolVersion[] { EnumProtocolVersion.V1200 }, false)]
public class SSystemChatMessage : IPacket
{
	public string Message { get; set; } = string.Empty;

	public bool Overlay { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		Message = buffer.ReadStringFromBuffer(65536);
		Overlay = buffer.ReadBoolean();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteStringToBuffer(Message);
		buffer.WriteBoolean(Overlay);
	}

	public bool HandlePacket(GameConnection connection)
	{
		return false;
	}
}
