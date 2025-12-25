namespace Codexus.Base1200.Plugin.Utils.Minecraft;

public class Boolean(bool value)
{
	public bool Value { get; set; } = value;

	public static implicit operator bool(Boolean b)
	{
		return b.Value;
	}

	public static implicit operator Boolean(bool b)
	{
		return new Boolean(b);
	}
}
