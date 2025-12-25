using Codexus.Base1200.Plugin.Utils.Minecraft;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class QuaternionCodec : IStreamCodec<IByteBuffer, Quaternionf>
{
	public Quaternionf Decode(IByteBuffer buffer)
	{
		float x = buffer.ReadFloat();
		float y = buffer.ReadFloat();
		float z = buffer.ReadFloat();
		float w = buffer.ReadFloat();
		return new Quaternionf(x, y, z, w);
	}

	public void Encode(IByteBuffer buffer, Quaternionf value)
	{
		buffer.WriteFloat(value.X);
		buffer.WriteFloat(value.Y);
		buffer.WriteFloat(value.Z);
		buffer.WriteFloat(value.W);
	}
}
