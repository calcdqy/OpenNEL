using Codexus.Base1200.Plugin.Utils.Minecraft;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class OptionalComponentCodec : IStreamCodec<IByteBuffer, Optional<Component>>
{
	private readonly ComponentCodec _componentCodec = new ComponentCodec();

	public Optional<Component> Decode(IByteBuffer buffer)
	{
		if (!buffer.ReadBoolean())
		{
			return Optional<Component>.Empty();
		}
		return Optional<Component>.Of(_componentCodec.Decode(buffer));
	}

	public void Encode(IByteBuffer buffer, Optional<Component> value)
	{
		buffer.WriteBoolean(value.IsPresent());
		if (value.IsPresent())
		{
			_componentCodec.Encode(buffer, value.Get());
		}
	}
}
