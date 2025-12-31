using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenNEL.Core.Utils;

public static class CrcSalt
{
    private const string Default = "54BB806A61CC561CDC3596E917E0032E";
    private const string CrcSaltEndpoint = "https://api.fandmc.cn/v8/crcsalt";
    
    private static string Cached = Default;
    private static DateTime LastFetch = DateTime.MinValue;
    private static readonly TimeSpan Refresh = TimeSpan.FromHours(1);
    private static readonly HttpClient Http = new();

    public static Func<string>? TokenProvider { get; set; }

    public static async Task<string> Compute()
    {
        if (DateTime.UtcNow - LastFetch < Refresh) return Cached;
        try
        {
            var token = TokenProvider?.Invoke() ?? "";
            if (string.IsNullOrEmpty(token))
            {
                return Cached;
            }
            var hwid = Hwid.Compute();
            var payload = JsonSerializer.Serialize(new { token, hwid });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var resp = await Http.PostAsync(CrcSaltEndpoint, content);
            var json = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                return Cached;
            }
            var obj = JsonSerializer.Deserialize<CrcSaltResponse>(json);
            if (obj == null || obj.success != true || string.IsNullOrWhiteSpace(obj.crcSalt))
            {
                return Cached;
            }
            Cached = obj.crcSalt;
            LastFetch = DateTime.UtcNow;
            return Cached;
        }
        catch
        {
            return Cached;
        }
    }

    public static string GetCached() => Cached;

    public static void InvalidateCache()
    {
        LastFetch = DateTime.MinValue;
    }

    private record CrcSaltResponse(bool success, string? crcSalt, string? gameVersion, string? error);
}
