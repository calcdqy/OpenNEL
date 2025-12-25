namespace Codexus.Base1200.Plugin.Utils.Minecraft;

public class Byte(byte value)
{
	public byte Value { get; set; } = value;

	public static implicit operator byte(Byte b)
	{
		return b.Value;
	}

	public static implicit operator Byte(byte b)
	{
		return new Byte(b);
	}
}
