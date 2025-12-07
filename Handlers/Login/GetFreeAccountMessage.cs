using System;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;
using OpenNEL.Utils;

namespace OpenNEL_WinUI.Handlers.Login
{
    public class GetFreeAccount
    {
        public async Task<object[]> Execute(string hwid = null,
                                            int timeoutSec = 30,
                                            string userAgent = null,
                                            int maxRetries = 3)
        {
            Log.Information("正在获取4399小号...");
            var status = new { type = "get_free_account_status", status = "processing", message = "获取小号中, 这可能需要点时间..." };
            HttpClient? client = null;
            object? resultPayload = null;
            try
            {
                var ua = string.IsNullOrWhiteSpace(userAgent) ? "OpenNEL-Client/1.0" : userAgent;
                var handler = new HttpClientHandler();
                handler.AutomaticDecompression = DecompressionMethods.All;
                client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(timeoutSec) };
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.ParseAdd(ua);
                var url = "https://api.fandmc.cn/v1/get4399";
                var hw = string.IsNullOrWhiteSpace(hwid) ? Hwid.Compute() : hwid;
                if (string.IsNullOrWhiteSpace(hw))
                {
                    resultPayload = new { type = "get_free_account_result", success = false, message = "空请求体" };
                    goto End;
                }
                if (!IsValidHwid(hw))
                {
                    resultPayload = new { type = "get_free_account_result", success = false, message = "请求错误" };
                    goto End;
                }
                HttpResponseMessage? resp = null;
                for (var attempt = 0; attempt < Math.Max(1, maxRetries); attempt++)
                {
                    try
                    {
                        var content = new StringContent(hw, System.Text.Encoding.UTF8, "text/plain");
                        resp = await client.PostAsync(url, content);
                        break;
                    }
                    catch when (attempt < Math.Max(1, maxRetries) - 1)
                    {
                        await Task.Delay(1000);
                    }
                }
                if (resp == null)
                {
                    resultPayload = new { type = "get_free_account_result", success = false, message = "网络错误" };
                }
                else if (resp.StatusCode != HttpStatusCode.OK)
                {
                    var msg = resp.StatusCode == HttpStatusCode.NotFound
                        ? "未找到 hwid"
                        : (resp.StatusCode == HttpStatusCode.BadRequest
                            ? "请求不合法"
                            : (resp.StatusCode == HttpStatusCode.TooManyRequests
                                ? "速率限制，请在20秒后重试"
                                : "上游错误"));
                    resultPayload = new { type = "get_free_account_result", success = false, message = msg };
                }
                else
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    JsonElement d;
                    try
                    {
                        d = JsonDocument.Parse(body).RootElement;
                    }
                    catch (Exception)
                    {
                        resultPayload = new { type = "get_free_account_result", success = false, message = "响应解析失败" };
                        goto End;
                    }
                    var codeOk = d.TryGetProperty("code", out var c) && c.ValueKind == JsonValueKind.Number && c.GetInt32() == 0;
                    if (codeOk)
                    {
                        var u = TryGetString(d, "account") ?? "";
                        var p = TryGetString(d, "password") ?? "";
                        var ck = TryGetString(d, "cookie");
                        Log.Information("获取成功: {Account} {Password}", u, p);
                        resultPayload = new
                        {
                            type = "get_free_account_result",
                            success = true,
                            username = u,
                            password = p,
                            cookie = ck,
                            message = "获取成功！",
                            raw = body
                        };
                    }
                    else
                    {
                        resultPayload = new { type = "get_free_account_result", success = false, message = body };
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "错误: {Message}", e.Message);
                resultPayload = new { type = "get_free_account_result", success = false, message = "错误: " + e.Message };
            }
            finally
            {
                client?.Dispose();
            }
            End:
            return new object[] { status, resultPayload ?? new { type = "get_free_account_result", success = false, message = "未知错误" } };
        }

    private static string? TryGetString(JsonElement root, string name)
    {
        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty(name, out var v))
        {
            if (v.ValueKind == JsonValueKind.String) return v.GetString();
            if (v.ValueKind == JsonValueKind.Number) return v.ToString();
            if (v.ValueKind == JsonValueKind.True || v.ValueKind == JsonValueKind.False) return v.ToString();
        }
        return null;
    }

    private static bool IsValidHwid(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        if (s.Length > 256) return false;
        foreach (var ch in s)
        {
            var ok = (ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z');
            if (!ok) return false;
        }
        return true;
    }
    }
}
