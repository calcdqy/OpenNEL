using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class OptionalUnsignedIntCodec : IStreamCodec<IByteBuffer, OptionalInt>
{
	public OptionalInt Decode(IByteBuffer buffer)
	{
		int num = buffer.ReadVarIntFromBuffer();
		if (num != 0)
		{
			return new OptionalInt(num - 1);
		}
		return new OptionalInt();
	}

	public void Encode(IByteBuffer buffer, OptionalInt value)
	{
		if (value.HasValue)
		{
			buffer.WriteVarInt(value.Value + 1);
		}
		else
		{
			buffer.WriteVarInt(0);
		}
	}
}
