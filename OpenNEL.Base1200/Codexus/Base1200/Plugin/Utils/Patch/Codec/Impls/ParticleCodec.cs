using System;
using Codexus.Base1200.Plugin.Utils.Minecraft;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class ParticleCodec : IStreamCodec<IByteBuffer, ParticleOptions>
{
	public ParticleOptions Decode(IByteBuffer buffer)
	{
		throw new NotImplementedException("Particle decoding functionality is not implemented");
	}

	public void Encode(IByteBuffer buffer, ParticleOptions value)
	{
		throw new NotImplementedException("Particle encoding functionality is not implemented");
	}
}
