using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class IntegerCodec : IStreamCodec<IByteBuffer, Integer>
{
	public Integer Decode(IByteBuffer buffer)
	{
		return new Integer(buffer.ReadVarIntFromBuffer());
	}

	public void Encode(IByteBuffer buffer, Integer value)
	{
		buffer.WriteVarInt(value.Value);
	}
}
