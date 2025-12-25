namespace Codexus.Base1200.Plugin.Utils;

public class Entity
{
	public int EntityId { get; set; }

	public byte[] EntityGuid { get; set; }

	public double X { get; set; }

	public double Y { get; set; }

	public double Z { get; set; }

	public float Yaw { get; set; }

	public float Pitch { get; set; }

	public bool OnGround { get; set; }
}
