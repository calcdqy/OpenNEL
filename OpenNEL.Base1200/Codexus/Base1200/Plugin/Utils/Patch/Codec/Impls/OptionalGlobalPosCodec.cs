using Codexus.Base1200.Plugin.Utils.Minecraft;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class OptionalGlobalPosCodec : IStreamCodec<IByteBuffer, Optional<GlobalPos>>
{
	private readonly StringCodec _stringCodec = new StringCodec();

	private readonly BlockPosCodec _blockPosCodec = new BlockPosCodec();

	public Optional<GlobalPos> Decode(IByteBuffer buffer)
	{
		if (!buffer.ReadBoolean())
		{
			return Optional<GlobalPos>.Empty();
		}
		Minecraft.String obj = _stringCodec.Decode(buffer);
		return Optional<GlobalPos>.Of(new GlobalPos(position: _blockPosCodec.Decode(buffer), dimension: obj.Value));
	}

	public void Encode(IByteBuffer buffer, Optional<GlobalPos> value)
	{
		buffer.WriteBoolean(value.IsPresent());
		if (value.IsPresent())
		{
			GlobalPos globalPos = value.Get();
			_stringCodec.Encode(buffer, new Minecraft.String(globalPos.Dimension));
			_blockPosCodec.Encode(buffer, globalPos.Position);
		}
	}
}
