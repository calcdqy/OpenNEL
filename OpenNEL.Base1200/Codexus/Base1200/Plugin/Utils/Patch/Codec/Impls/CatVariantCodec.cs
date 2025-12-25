using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class CatVariantCodec : IStreamCodec<IByteBuffer, Holder<CatVariant>>
{
	public Holder<CatVariant> Decode(IByteBuffer buffer)
	{
		return new Holder<CatVariant>(new CatVariant(buffer.ReadVarIntFromBuffer()));
	}

	public void Encode(IByteBuffer buffer, Holder<CatVariant> value)
	{
		buffer.WriteVarInt(value.Value.VariantId);
	}
}
