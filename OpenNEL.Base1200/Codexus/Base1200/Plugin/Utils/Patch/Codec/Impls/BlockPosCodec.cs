using Codexus.Base1200.Plugin.Utils.Minecraft;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class BlockPosCodec : IStreamCodec<IByteBuffer, BlockPos>
{
	public BlockPos Decode(IByteBuffer buffer)
	{
		long num = buffer.ReadLong();
		int x = (int)(num >> 38);
		int y = (int)(num << 52 >> 52);
		int z = (int)(num << 26 >> 38);
		return new BlockPos(x, y, z);
	}

	public void Encode(IByteBuffer buffer, BlockPos value)
	{
		long num = (long)((((ulong)value.X & 0x3FFFFFFuL) << 38) | ((ulong)value.Y & 0xFFFuL) | (((ulong)value.Z & 0x3FFFFFFuL) << 12));
		buffer.WriteLong(num);
	}
}
