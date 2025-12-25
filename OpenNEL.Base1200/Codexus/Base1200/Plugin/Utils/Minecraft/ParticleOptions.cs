namespace Codexus.Base1200.Plugin.Utils.Minecraft;

public class ParticleOptions(int particleId, byte[] rawData)
{
	public int ParticleId { get; set; } = particleId;

	public byte[] RawData { get; set; } = rawData;
}
