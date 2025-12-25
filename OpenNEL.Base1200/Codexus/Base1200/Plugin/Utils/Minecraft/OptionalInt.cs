namespace Codexus.Base1200.Plugin.Utils.Minecraft;

public class OptionalInt
{
	public bool HasValue { get; set; }

	public int Value { get; set; }

	public OptionalInt(int value)
	{
		HasValue = true;
		Value = value;
	}

	public OptionalInt()
	{
		HasValue = false;
	}
}
