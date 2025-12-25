using Codexus.Base1200.Plugin.Utils.Minecraft;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class FloatCodec : IStreamCodec<IByteBuffer, Float>
{
	public Float Decode(IByteBuffer buffer)
	{
		return new Float(buffer.ReadFloat());
	}

	public void Encode(IByteBuffer buffer, Float value)
	{
		buffer.WriteFloat(value.Value);
	}
}
