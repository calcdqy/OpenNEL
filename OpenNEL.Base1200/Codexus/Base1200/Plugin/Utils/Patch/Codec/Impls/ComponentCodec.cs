using Codexus.Base1200.Plugin.Utils.Minecraft;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class ComponentCodec : IStreamCodec<IByteBuffer, Component>
{
	private readonly CompoundTagCodec _nbtCodec = new CompoundTagCodec();

	public Component Decode(IByteBuffer buffer)
	{
		return new Component(_nbtCodec.Decode(buffer));
	}

	public void Encode(IByteBuffer buffer, Component value)
	{
		_nbtCodec.Encode(buffer, value.Tag);
	}
}
