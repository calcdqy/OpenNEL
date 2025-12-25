using DotNetty.Buffers;
using Minecraft = Codexus.Base1200.Plugin.Utils.Minecraft;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class BooleanCodec : IStreamCodec<IByteBuffer, Minecraft.Boolean>
{
	public Minecraft.Boolean Decode(IByteBuffer buffer)
	{
		return new Minecraft.Boolean(buffer.ReadBoolean());
	}

	public void Encode(IByteBuffer buffer, Minecraft.Boolean value)
	{
		buffer.WriteBoolean(value.Value);
	}
}
