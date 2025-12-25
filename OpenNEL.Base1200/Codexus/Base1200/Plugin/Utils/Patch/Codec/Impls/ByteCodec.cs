using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class ByteCodec : IStreamCodec<IByteBuffer, Minecraft.Byte>
{
	public Minecraft.Byte Decode(IByteBuffer buffer)
	{
		return new Minecraft.Byte(buffer.ReadByte());
	}

	public void Encode(IByteBuffer buffer, Minecraft.Byte value)
	{
		buffer.WriteByte((int)value.Value);
	}
}
