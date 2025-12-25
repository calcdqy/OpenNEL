namespace Codexus.Base1200.Plugin.Utils.Minecraft;

public class Integer(int value)
{
	public int Value { get; set; } = value;

	public static implicit operator int(Integer i)
	{
		return i.Value;
	}

	public static implicit operator Integer(int i)
	{
		return new Integer(i);
	}
}
