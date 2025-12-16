using System.Text.Json.Serialization;

namespace OpenNEL.type;

public class SettingData
{
    [JsonPropertyName("themeMode")] public string ThemeMode { get; set; } = "image";
    [JsonPropertyName("themeColor")] public string ThemeColor { get; set; } = "#181818";
    [JsonPropertyName("themeImage")] public string ThemeImage { get; set; } = string.Empty;
    [JsonPropertyName("backdrop")] public string Backdrop { get; set; } = "mica";
    [JsonPropertyName("vetaProcessKeyword")] public string VetaProcessKeyword { get; set; } = "Veta";
    [JsonPropertyName("autoCopyIpOnStart")] public bool AutoCopyIpOnStart { get; set; } = false;
    [JsonPropertyName("debug")] public bool Debug { get; set; } = false;
    [JsonPropertyName("socks5Enabled")] public bool Socks5Enabled { get; set; } = false;

    [JsonPropertyName("socks5Address")] public string Socks5Address { get; set; } = string.Empty;
    [JsonPropertyName("socks5Port")] public int Socks5Port { get; set; } = 1080;
    [JsonPropertyName("socks5Username")] public string Socks5Username { get; set; } = string.Empty;
    [JsonPropertyName("socks5Password")] public string Socks5Password { get; set; } = string.Empty;
}
