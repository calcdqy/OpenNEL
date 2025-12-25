namespace Codexus.Base1200.Plugin.Utils.Minecraft;

public class Long(long value)
{
	public long Value { get; set; } = value;

	public static implicit operator long(Long l)
	{
		return l.Value;
	}

	public static implicit operator Long(long l)
	{
		return new Long(l);
	}
}
