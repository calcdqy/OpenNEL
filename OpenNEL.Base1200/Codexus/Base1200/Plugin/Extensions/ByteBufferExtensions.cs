using System;
using Codexus.Base1200.Plugin.Utils.Minecraft;
using Codexus.Base1200.Plugin.Utils.Nbt;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;
using Serilog;

namespace Codexus.Base1200.Plugin.Extensions;

public static class ByteBufferExtensions
{
	public static int NbtLength(this IByteBuffer buffer, bool withName = false)
	{
		int readerIndex = buffer.ReaderIndex;
		NbtTagType nbtTagType = (NbtTagType)buffer.ReadByte();
		if (nbtTagType == NbtTagType.TagEnd)
		{
			return 1;
		}
		if (withName)
		{
			short num = buffer.ReadShort();
			buffer.SkipBytes((int)num);
		}
		SkipTagPayload(buffer, nbtTagType);
		int result = buffer.ReaderIndex - readerIndex;
		buffer.SetReaderIndex(readerIndex);
		return result;
	}

	private static void ReadCompoundTagPayload(IByteBuffer buffer)
	{
		while (true)
		{
			NbtTagType nbtTagType = (NbtTagType)buffer.ReadByte();
			if (nbtTagType != NbtTagType.TagEnd)
			{
				short num = buffer.ReadShort();
				buffer.SkipBytes((int)num);
				SkipTagPayload(buffer, nbtTagType);
				continue;
			}
			break;
		}
	}

	private static void SkipTagPayload(IByteBuffer buffer, NbtTagType typeId)
	{
		switch (typeId)
		{
		case NbtTagType.TagByte:
			buffer.SkipBytes(1);
			break;
		case NbtTagType.TagShort:
			buffer.SkipBytes(2);
			break;
		case NbtTagType.TagInt:
			buffer.SkipBytes(4);
			break;
		case NbtTagType.TagLong:
			buffer.SkipBytes(8);
			break;
		case NbtTagType.TagFloat:
			buffer.SkipBytes(4);
			break;
		case NbtTagType.TagDouble:
			buffer.SkipBytes(8);
			break;
		case NbtTagType.TagByteArray:
		{
			int num5 = buffer.ReadInt();
			buffer.SkipBytes(num5);
			break;
		}
		case NbtTagType.TagString:
		{
			ushort num4 = buffer.ReadUnsignedShort();
			buffer.SkipBytes((int)num4);
			break;
		}
		case NbtTagType.TagList:
		{
			NbtTagType typeId2 = (NbtTagType)buffer.ReadByte();
			int num3 = buffer.ReadInt();
			for (int i = 0; i < num3; i++)
			{
				SkipTagPayload(buffer, typeId2);
			}
			break;
		}
		case NbtTagType.TagCompound:
			ReadCompoundTagPayload(buffer);
			break;
		case NbtTagType.TagIntArray:
		{
			int num2 = buffer.ReadInt();
			buffer.SkipBytes(num2 * 4);
			break;
		}
		case NbtTagType.TagLongArray:
		{
			int num = buffer.ReadInt();
			buffer.SkipBytes(num * 8);
			break;
		}
		default:
			throw new InvalidOperationException($"Unknown NBT tag type: {typeId}");
		case NbtTagType.TagEnd:
			break;
		}
	}

	public static int ItemStackLength(this IByteBuffer buffer)
	{
		int readerIndex = buffer.ReaderIndex;
		int num = buffer.ReadVarIntFromBuffer();
		int num2 = buffer.ReadVarIntFromBuffer();
		if (num == 0 && num2 == 0)
		{
			return -1;
		}
		for (int i = 0; i < num; i++)
		{
			DataComponentType typeId = (DataComponentType)buffer.ReadVarIntFromBuffer();
			SkipDataComponent(buffer, typeId);
		}
		for (int j = 0; j < num2; j++)
		{
			buffer.ReadVarIntFromBuffer();
		}
		int readerIndex2 = buffer.ReaderIndex;
		buffer.SetReaderIndex(readerIndex);
		return readerIndex2 - readerIndex;
	}

	private static void SkipDataComponent(IByteBuffer buffer, DataComponentType typeId)
	{
		switch (typeId)
		{
		case DataComponentType.CustomData:
			buffer.SkipBytes(buffer.NbtLength());
			return;
		case DataComponentType.Fireworks:
			SkipFireworks(buffer);
			return;
		case DataComponentType.FireworkExplosion:
			SkipFireworkExplosion(buffer);
			return;
		}
		Log.Warning<DataComponentType>("Unknown DataComponent type: {Type}", typeId);
		throw new NotImplementedException($"Unknown DataComponent type id: {typeId}");
	}

	private static void SkipFireworkExplosion(IByteBuffer buffer)
	{
		buffer.ReadVarIntFromBuffer();
		int num = buffer.ReadVarIntFromBuffer();
		buffer.SkipBytes(num * 4);
		int num2 = buffer.ReadVarIntFromBuffer();
		buffer.SkipBytes(num2 * 4);
		buffer.ReadBoolean();
		buffer.ReadBoolean();
	}

	private static void SkipFireworks(IByteBuffer buffer)
	{
		buffer.ReadVarIntFromBuffer();
		int num = buffer.ReadVarIntFromBuffer();
		for (int i = 0; i < num; i++)
		{
			SkipFireworkExplosion(buffer);
		}
	}
}
