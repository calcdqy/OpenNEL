using Codexus.Base1200.Plugin.Utils.Minecraft;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class Vector3FCodec : IStreamCodec<IByteBuffer, Vector3F>
{
	public Vector3F Decode(IByteBuffer buffer)
	{
		float x = buffer.ReadFloat();
		float y = buffer.ReadFloat();
		float z = buffer.ReadFloat();
		return new Vector3F(x, y, z);
	}

	public void Encode(IByteBuffer buffer, Vector3F value)
	{
		buffer.WriteFloat(value.X);
		buffer.WriteFloat(value.Y);
		buffer.WriteFloat(value.Z);
	}
}
