using System;
using System.Collections.Generic;
using System.IO;
using Codexus.Base1200.Plugin.Event;
using Codexus.Base1200.Plugin.Utils.Patch.Metadata;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Manager;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Packet.Play.Server.Patch;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, 88, new EnumProtocolVersion[] { EnumProtocolVersion.V1206 }, false)]
public class SSetEntityMetadata : IPacket
{
	public int EntityId { get; set; }

	public List<IDataValue> PackedItems { get; set; } = new List<IDataValue>();

	public byte[]? RawData { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		EntityId = buffer.ReadVarIntFromBuffer();
		int readerIndex = buffer.ReaderIndex;
		try
		{
			int id;
			while ((id = buffer.ReadByte()) != 255)
			{
				PackedItems.Add(DataValue<object>.Read(buffer, id));
			}
			if (buffer.ReadableBytes > 0)
			{
				throw new InvalidDataException("Unexpected end of stream, remaining " + buffer.ReadableBytes + " bytes");
			}
		}
		catch (NotImplementedException)
		{
			buffer.SetReaderIndex(readerIndex);
			RawData = buffer.ReadByteArrayReadableBytes();
		}
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteVarInt(EntityId);
		if (RawData != null)
		{
			buffer.WriteBytes(RawData);
			return;
		}
		foreach (IDataValue packedItem in PackedItems)
		{
			packedItem.Write(buffer);
		}
		buffer.WriteByte(255);
	}

	public bool HandlePacket(GameConnection connection)
	{
		return EventManager.Instance.TriggerEvent("base_1200", new EventSetEntityMetadata(connection, this)).IsCancelled;
	}
}
