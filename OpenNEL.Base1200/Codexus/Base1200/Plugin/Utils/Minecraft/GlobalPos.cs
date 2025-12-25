namespace Codexus.Base1200.Plugin.Utils.Minecraft;

public class GlobalPos(string dimension, BlockPos position)
{
	public string Dimension { get; set; } = dimension;

	public BlockPos Position { get; set; } = position;
}
