using Codexus.Base1200.Plugin.Utils.Minecraft;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class RotationsCodec : IStreamCodec<IByteBuffer, Rotations>
{
	public Rotations Decode(IByteBuffer buffer)
	{
		float x = buffer.ReadFloat();
		float y = buffer.ReadFloat();
		float z = buffer.ReadFloat();
		return new Rotations(x, y, z);
	}

	public void Encode(IByteBuffer buffer, Rotations value)
	{
		buffer.WriteFloat(value.X);
		buffer.WriteFloat(value.Y);
		buffer.WriteFloat(value.Z);
	}
}
