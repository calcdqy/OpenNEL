using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class DirectionCodec : IStreamCodec<IByteBuffer, Direction>
{
	public Direction Decode(IByteBuffer buffer)
	{
		return (Direction)buffer.ReadVarIntFromBuffer();
	}

	public void Encode(IByteBuffer buffer, Direction value)
	{
		buffer.WriteVarInt((int)value);
	}
}
