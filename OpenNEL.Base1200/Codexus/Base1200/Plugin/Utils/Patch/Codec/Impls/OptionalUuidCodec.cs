using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class OptionalUuidCodec : IStreamCodec<IByteBuffer, Optional<byte[]>>
{
	public Optional<byte[]> Decode(IByteBuffer buffer)
	{
		if (!buffer.ReadBoolean())
		{
			return Optional<byte[]>.Empty();
		}
		byte[] array = new byte[16];
		buffer.ReadBytes(array);
		return Optional<byte[]>.Of(array);
	}

	public void Encode(IByteBuffer buffer, Optional<byte[]> value)
	{
		buffer.WriteBoolean(value.IsPresent());
		if (value.IsPresent())
		{
			buffer.WriteBytes(value.Get());
		}
	}
}
