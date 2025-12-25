namespace Codexus.Base1200.Plugin.Utils;

public class LocalPlayer(int playerId) : Player()
{
	public int PlayerId { get; set; } = playerId;
}
