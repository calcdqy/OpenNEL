using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class BlockStateCodec : IStreamCodec<IByteBuffer, BlockState>
{
	public BlockState Decode(IByteBuffer buffer)
	{
		return new BlockState(buffer.ReadVarIntFromBuffer());
	}

	public void Encode(IByteBuffer buffer, BlockState value)
	{
		buffer.WriteVarInt(value.StateId);
	}
}
