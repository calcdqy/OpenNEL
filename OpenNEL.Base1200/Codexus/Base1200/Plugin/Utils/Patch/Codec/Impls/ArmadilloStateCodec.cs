using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class ArmadilloStateCodec : IStreamCodec<IByteBuffer, Armadillo.ArmadilloState>
{
	public Armadillo.ArmadilloState Decode(IByteBuffer buffer)
	{
		return (Armadillo.ArmadilloState)buffer.ReadVarIntFromBuffer();
	}

	public void Encode(IByteBuffer buffer, Armadillo.ArmadilloState value)
	{
		buffer.WriteVarInt((int)value);
	}
}
