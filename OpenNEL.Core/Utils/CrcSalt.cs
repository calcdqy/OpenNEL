using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenNEL.Core.Utils;

public static class CrcSalt
{
    private const string CrcSaltEndpoint = "https://api.fandmc.cn/v8/crcsalt";
    
    private static string? Cached = null;
    private static DateTime LastFetch = DateTime.MinValue;
    private static readonly TimeSpan Refresh = TimeSpan.FromHours(1);
    private static readonly HttpClient Http = new();

    public static Func<string>? TokenProvider { get; set; }

    public static async Task<string> Compute()
    {
        if (Cached != null && DateTime.UtcNow - LastFetch < Refresh) return Cached;
        try
        {
            var token = TokenProvider?.Invoke() ?? "";
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("未提供 Token，无法获取 CrcSalt");
            }
            var hwid = Hwid.Compute();
            var payload = JsonSerializer.Serialize(new { token, hwid });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var resp = await Http.PostAsync(CrcSaltEndpoint, content);
            var json = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"获取 CrcSalt 失败: {resp.StatusCode}");
            }
            var obj = JsonSerializer.Deserialize<CrcSaltResponse>(json);
            if (obj == null || obj.success != true || string.IsNullOrWhiteSpace(obj.crcSalt))
            {
                throw new InvalidOperationException($"获取 CrcSalt 失败: {obj?.error ?? "未知错误"}");
            }
            Cached = obj.crcSalt;
            LastFetch = DateTime.UtcNow;
            return Cached;
        }
        catch
        {
            throw;
        }
    }

    public static string GetCached() => Cached ?? throw new InvalidOperationException("CrcSalt 未初始化");

    public static void InvalidateCache()
    {
        LastFetch = DateTime.MinValue;
        Cached = null;
    }

    private record CrcSaltResponse(bool success, string? crcSalt, string? gameVersion, string? error);
}
