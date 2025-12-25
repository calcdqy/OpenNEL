namespace Codexus.Base1200.Plugin.Utils.Minecraft;

public class VillagerData(int villagerType, int villagerProfession, int level)
{
	public int VillagerType { get; set; } = villagerType;

	public int VillagerProfession { get; set; } = villagerProfession;

	public int Level { get; set; } = level;
}
