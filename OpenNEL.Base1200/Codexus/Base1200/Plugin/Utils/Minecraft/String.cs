namespace Codexus.Base1200.Plugin.Utils.Minecraft;

public class String(string value)
{
	public string Value { get; set; } = value;

	public static implicit operator string(String s)
	{
		return s.Value;
	}

	public static implicit operator String(string s)
	{
		return new String(s);
	}
}
