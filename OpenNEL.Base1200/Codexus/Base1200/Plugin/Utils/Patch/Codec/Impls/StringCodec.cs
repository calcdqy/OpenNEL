using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;
using Minecraft = Codexus.Base1200.Plugin.Utils.Minecraft;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class StringCodec : IStreamCodec<IByteBuffer, Minecraft.String>
{
	public Minecraft.String Decode(IByteBuffer buffer)
	{
		return new Minecraft.String(buffer.ReadStringFromBuffer(32767));
	}

	public void Encode(IByteBuffer buffer, Minecraft.String value)
	{
		buffer.WriteStringToBuffer(value.Value);
	}
}
