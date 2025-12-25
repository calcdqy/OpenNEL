using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class FrogVariantCodec : IStreamCodec<IByteBuffer, Holder<FrogVariant>>
{
	public Holder<FrogVariant> Decode(IByteBuffer buffer)
	{
		return new Holder<FrogVariant>(new FrogVariant(buffer.ReadVarIntFromBuffer()));
	}

	public void Encode(IByteBuffer buffer, Holder<FrogVariant> value)
	{
		buffer.WriteVarInt(value.Value.VariantId);
	}
}
