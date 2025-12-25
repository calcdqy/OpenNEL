using System.Text.Json;
using System.Text.Json.Serialization;

namespace Codexus.Base1200.Plugin.Utils.Nbt;

public class TextComponent
{
	[JsonPropertyName("text")]
	public string Text { get; set; } = string.Empty;

	[JsonPropertyName("color")]
	public string Color { get; set; } = string.Empty;

	public string ToJson()
	{
		return JsonSerializer.Serialize(this);
	}
}
