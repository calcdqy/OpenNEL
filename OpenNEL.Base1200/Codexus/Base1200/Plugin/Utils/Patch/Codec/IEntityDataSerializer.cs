using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec;

public interface IEntityDataSerializer<TValue>
{
	IStreamCodec<IByteBuffer, TValue> Codec();

	static IEntityDataSerializer<T> ForValueType<T>(IStreamCodec<IByteBuffer, T> streamCodec)
	{
		return new EntityDataSerializerRef<T>(streamCodec);
	}
}
