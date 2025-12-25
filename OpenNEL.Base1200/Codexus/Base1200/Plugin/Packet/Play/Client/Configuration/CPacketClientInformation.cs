using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;
using Serilog;

namespace Codexus.Base1200.Plugin.Packet.Play.Client.Configuration;

[RegisterPacket(EnumConnectionState.Configuration, EnumPacketDirection.ServerBound, 0, EnumProtocolVersion.V1206, false)]
public class CPacketClientInformation : IPacket
{
	public string Locale { get; set; } = string.Empty;

	public byte ViewDistance { get; set; }

	public int ChatMode { get; set; }

	public bool ChatColors { get; set; }

	public byte DisplayedSkinParts { get; set; }

	public int MainHand { get; set; }

	public bool EnableTextFiltering { get; set; }

	public bool AllowServerListings { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		Locale = buffer.ReadStringFromBuffer(16);
		ViewDistance = buffer.ReadByte();
		ChatMode = buffer.ReadVarIntFromBuffer();
		ChatColors = buffer.ReadBoolean();
		DisplayedSkinParts = buffer.ReadByte();
		MainHand = buffer.ReadVarIntFromBuffer();
		EnableTextFiltering = buffer.ReadBoolean();
		AllowServerListings = buffer.ReadBoolean();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteStringToBuffer(Locale);
		buffer.WriteByte((int)ViewDistance);
		buffer.WriteVarInt(ChatMode);
		buffer.WriteBoolean(ChatColors);
		buffer.WriteByte((int)DisplayedSkinParts);
		buffer.WriteVarInt(MainHand);
		buffer.WriteBoolean(EnableTextFiltering);
		buffer.WriteBoolean(AllowServerListings);
	}

	public bool HandlePacket(GameConnection connection)
	{
		Log.Debug("Received Client Information: Locale = {Locale}, ViewDistance = {ViewDistance}, ChatMode = {ChatMode}, ChatColors = {ChatColors}, DisplayedSkinParts = {DisplayedSkinParts}, MainHand = {MainHand}, EnableTextFiltering = {EnableTextFiltering}, AllowServerListings = {AllowServerListings}", new object[8] { Locale, ViewDistance, ChatMode, ChatColors, DisplayedSkinParts, MainHand, EnableTextFiltering, AllowServerListings });
		return false;
	}
}
