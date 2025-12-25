using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class WolfVariantCodec : IStreamCodec<IByteBuffer, Holder<WolfVariant>>
{
	public Holder<WolfVariant> Decode(IByteBuffer buffer)
	{
		return new Holder<WolfVariant>(new WolfVariant(buffer.ReadVarIntFromBuffer()));
	}

	public void Encode(IByteBuffer buffer, Holder<WolfVariant> value)
	{
		buffer.WriteVarInt(value.Value.VariantId);
	}
}
