namespace Codexus.Base1200.Plugin.Utils.Minecraft;

public class ItemStack
{
	public int Count { get; set; }

	public int ItemId { get; set; }

	public byte[]? RawData { get; set; }

	public static ItemStack Empty => new ItemStack
	{
		Count = 0
	};

	public bool IsEmpty => Count <= 0;
}
