using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec;

public class EntityDataSerializerRef<T>(IStreamCodec<IByteBuffer, T> codec) : IEntityDataSerializer<T>
{
	public IStreamCodec<IByteBuffer, T> Codec()
	{
		return codec;
	}
}
