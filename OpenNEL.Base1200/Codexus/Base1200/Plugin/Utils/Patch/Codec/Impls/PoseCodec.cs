using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class PoseCodec : IStreamCodec<IByteBuffer, Pose>
{
	public Pose Decode(IByteBuffer buffer)
	{
		return (Pose)buffer.ReadVarIntFromBuffer();
	}

	public void Encode(IByteBuffer buffer, Pose value)
	{
		buffer.WriteVarInt((int)value);
	}
}
