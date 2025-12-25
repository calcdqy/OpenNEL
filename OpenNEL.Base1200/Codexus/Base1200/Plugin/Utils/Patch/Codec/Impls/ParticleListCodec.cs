using System.Collections.Generic;
using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class ParticleListCodec : IStreamCodec<IByteBuffer, List<ParticleOptions>>
{
	private readonly ParticleCodec _particleCodec = new ParticleCodec();

	public List<ParticleOptions> Decode(IByteBuffer buffer)
	{
		int num = buffer.ReadVarIntFromBuffer();
		List<ParticleOptions> list = new List<ParticleOptions>(num);
		for (int i = 0; i < num; i++)
		{
			list.Add(_particleCodec.Decode(buffer));
		}
		return list;
	}

	public void Encode(IByteBuffer buffer, List<ParticleOptions> value)
	{
		buffer.WriteVarInt(value.Count);
		foreach (ParticleOptions item in value)
		{
			_particleCodec.Encode(buffer, item);
		}
	}
}
