namespace Codexus.Base1200.Plugin.Utils.Minecraft;

public class Holder<T>(T value) where T : class
{
	public T Value { get; set; } = value;
}
