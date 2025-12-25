namespace Codexus.Base1200.Plugin.Utils.Patch.Codec;

public interface IStreamCodec<in TBuffer, TValue>
{
	TValue Decode(TBuffer buffer);

	void Encode(TBuffer buffer, TValue value);
}
