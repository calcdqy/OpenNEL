using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Common;

namespace Codexus.Base1200.Plugin.Utils.Nbt;

public static class TextComponentSerializer
{
	public static IByteBuffer Serialize(TextComponent component, IByteBufferAllocator? allocator = null)
	{
		if (allocator == null)
		{
			allocator = (IByteBufferAllocator?)(object)PooledByteBufferAllocator.Default;
		}
		IByteBuffer val = allocator.Buffer();
		try
		{
			val.WriteByte(10);
			val.WriteByte(8);
			WriteString(val, "text");
			WriteString(val, component.Text);
			val.WriteByte(8);
			WriteString(val, "color");
			WriteString(val, component.Color);
			val.WriteByte(0);
			return val;
		}
		catch
		{
			((IReferenceCounted)val).Release();
			throw;
		}
	}

	public static TextComponent Deserialize(IByteBuffer buffer)
	{
		throw new NotImplementedException();
	}

	private static void WriteString(IByteBuffer buffer, string value)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(value);
		buffer.WriteUnsignedShort((ushort)bytes.Length);
		buffer.WriteBytes(bytes);
	}
}
