using Codexus.Base1200.Plugin.Utils.Nbt;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Server.Simulation;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, 108, new EnumProtocolVersion[] { EnumProtocolVersion.V1206 }, true)]
public class SSystemChatMessageV1206 : IPacket
{
	public TextComponent Message { get; set; } = new TextComponent();

	public bool Overlay { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		Message = TextComponentSerializer.Deserialize(buffer);
		Overlay = buffer.ReadBoolean();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteBytes(TextComponentSerializer.Serialize(Message));
		buffer.WriteBoolean(Overlay);
	}

	public bool HandlePacket(GameConnection connection)
	{
		return false;
	}
}
