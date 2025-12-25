using Codexus.Base1200.Plugin.Utils.Minecraft;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class OptionalBlockPosCodec : IStreamCodec<IByteBuffer, Optional<BlockPos>>
{
	private readonly BlockPosCodec _blockPosCodec = new BlockPosCodec();

	public Optional<BlockPos> Decode(IByteBuffer buffer)
	{
		if (!buffer.ReadBoolean())
		{
			return Optional<BlockPos>.Empty();
		}
		return Optional<BlockPos>.Of(_blockPosCodec.Decode(buffer));
	}

	public void Encode(IByteBuffer buffer, Optional<BlockPos> value)
	{
		buffer.WriteBoolean(value.IsPresent());
		if (value.IsPresent())
		{
			_blockPosCodec.Encode(buffer, value.Get());
		}
	}
}
