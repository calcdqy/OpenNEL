namespace Codexus.Base1200.Plugin.Utils.Minecraft;

public class Float(float value)
{
	public float Value { get; set; } = value;

	public static implicit operator float(Float f)
	{
		return f.Value;
	}

	public static implicit operator Float(float f)
	{
		return new Float(f);
	}
}
