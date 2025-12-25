using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class PaintingVariantCodec : IStreamCodec<IByteBuffer, Holder<PaintingVariant>>
{
	public Holder<PaintingVariant> Decode(IByteBuffer buffer)
	{
		return new Holder<PaintingVariant>(new PaintingVariant(buffer.ReadVarIntFromBuffer()));
	}

	public void Encode(IByteBuffer buffer, Holder<PaintingVariant> value)
	{
		buffer.WriteVarInt(value.Value.VariantId);
	}
}
