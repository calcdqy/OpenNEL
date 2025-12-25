using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class OptionalBlockStateCodec : IStreamCodec<IByteBuffer, Optional<BlockState>>
{
	private readonly BlockStateCodec _blockStateCodec = new BlockStateCodec();

	public Optional<BlockState> Decode(IByteBuffer buffer)
	{
		int num = buffer.ReadVarIntFromBuffer();
		if (num != 0)
		{
			return Optional<BlockState>.Of(new BlockState(num));
		}
		return Optional<BlockState>.Empty();
	}

	public void Encode(IByteBuffer buffer, Optional<BlockState> value)
	{
		buffer.WriteVarInt(value.IsPresent() ? value.Get().StateId : 0);
	}
}
