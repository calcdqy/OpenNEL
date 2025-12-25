using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class SnifferStateCodec : IStreamCodec<IByteBuffer, Sniffer.State>
{
	public Sniffer.State Decode(IByteBuffer buffer)
	{
		return (Sniffer.State)buffer.ReadVarIntFromBuffer();
	}

	public void Encode(IByteBuffer buffer, Sniffer.State value)
	{
		buffer.WriteVarInt((int)value);
	}
}
