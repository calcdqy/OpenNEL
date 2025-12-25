using System;
using Codexus.Base1200.Plugin.Extensions;
using Codexus.Base1200.Plugin.Utils.Minecraft;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class CompoundTagCodec : IStreamCodec<IByteBuffer, CompoundTag>
{
	public CompoundTag Decode(IByteBuffer buffer)
	{
		byte[] array = new byte[buffer.NbtLength()];
		buffer.ReadBytes(array);
		return new CompoundTag
		{
			RawData = array
		};
	}

	public void Encode(IByteBuffer buffer, CompoundTag value)
	{
		if (value.RawData != null)
		{
			buffer.WriteBytes(value.RawData);
			return;
		}
		throw new InvalidOperationException("CompoundTag RawData is null");
	}
}
