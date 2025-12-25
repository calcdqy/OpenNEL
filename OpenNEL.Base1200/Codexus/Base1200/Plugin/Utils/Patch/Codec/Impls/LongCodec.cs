using Codexus.Base1200.Plugin.Utils.Minecraft;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class LongCodec : IStreamCodec<IByteBuffer, Long>
{
	public Long Decode(IByteBuffer buffer)
	{
		return new Long(buffer.ReadLong());
	}

	public void Encode(IByteBuffer buffer, Long value)
	{
		buffer.WriteLong(value.Value);
	}
}
