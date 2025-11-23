using Codexus.OpenSDK;
using OpenNEL.network;
using OpenNEL.type;
using OpenNEL.Utils;
using System.Text.Json;
using System.Text;
using Serilog;
using Codexus.OpenSDK.Exceptions;

namespace OpenNEL.HandleWebSocket.Login;

internal class Login4399Handler : IWsHandler
{
    public string Type => "login_4399";

    public async Task ProcessAsync(System.Net.WebSockets.WebSocket ws, JsonElement root)
    {
        var req = Parse(root);
        if (string.IsNullOrWhiteSpace(req.account) || string.IsNullOrWhiteSpace(req.password))
        {
            await Send(ws, new { type = "login_error", message = "账号或密码为空" });
            return;
        }
        try
        {
            await AppState.Services!.X19.InitializeDeviceAsync();
            var cookieJson = await LoginAsync(req);
            var cont = await AppState.Services!.X19.ContinueAsync(cookieJson);
            var otp = cont.Item1;
            var channel = cont.Item2;
            await X19.InterconnectionApi.LoginStart(otp.EntityId, otp.Token);
            AppState.Accounts[otp.EntityId] = channel;
            AppState.Auths[otp.EntityId] = otp;
            AppState.SelectedAccountId = otp.EntityId;
            await Send(ws, new { type = "Success_login", entityId = otp.EntityId, channel });
        }
        catch (Exception ex) when (
            (ex.Data.Contains("captcha_url") || ex.Data.Contains("captchaUrl")) &&
            (ex.Data.Contains("session_id") || ex.Data.Contains("sessionId")))
        {
            var capUrl = ex.Data.Contains("captcha_url") ? ex.Data["captcha_url"]?.ToString() : ex.Data["captchaUrl"]?.ToString();
            var sidVal = ex.Data.Contains("session_id") ? ex.Data["session_id"]?.ToString() : ex.Data["sessionId"]?.ToString();
            if (Debug.Get()) Log.Information("login_4399 captcha_required ex: {Message} data: {Data}", ex.Message, DumpData(ex.Data));
            await Send(ws, new { type = "captcha_required", account = req.account, password = req.password, captchaUrl = capUrl, sessionId = sidVal });
        }
        catch (VerifyException vex)
        {
            var capUrl = vex.Data.Contains("captcha_url") ? vex.Data["captcha_url"]?.ToString() : (vex.Data.Contains("captchaUrl") ? vex.Data["captchaUrl"]?.ToString() : null);
            var sidVal = vex.Data.Contains("session_id") ? vex.Data["session_id"]?.ToString() : (vex.Data.Contains("sessionId") ? vex.Data["sessionId"]?.ToString() : null);
            if (!string.IsNullOrWhiteSpace(capUrl) && !string.IsNullOrWhiteSpace(sidVal))
            {
                if (Debug.Get()) Log.Information("login_4399 verify captcha info: url={Url} sid={Sid}", capUrl, sidVal);
                await Send(ws, new { type = "captcha_required", account = req.account, password = req.password, captchaUrl = capUrl, sessionId = sidVal });
            }
            else
            {
                if (Debug.Get()) Log.Information("login_4399 verify exception: {Message} data: {Data}", vex.Message, DumpData(vex.Data));
                await Send(ws, new { type = "captcha_required", account = req.account, password = req.password });
            }
        }
        catch (Exception ex) when (
            (ex.Message?.Contains("Parameter not found", StringComparison.OrdinalIgnoreCase) == true) ||
            (ex.StackTrace?.Contains("Codexus.OpenSDK.Http.QueryBuilder.Get", StringComparison.Ordinal) == true))
        {
            if (Debug.Get()) Log.Information("login_4399 exception: {Message}", ex.Message);
            await Send(ws, new { type = "login_error", message = "账号或密码错误" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "4399登录失败");
            if (Debug.Get()) Log.Information("login_4399 exception data: {Data}", DumpData(ex.Data));
            await Send(ws, new { type = "login_error", message = ex.Message ?? "登录失败" });
        }
    }

    private static (string account, string password, string? sessionId, string? captcha) Parse(JsonElement root)
    {
        var account = root.TryGetProperty("account", out var acc) ? acc.GetString() : null;
        var password = root.TryGetProperty("password", out var pwd) ? pwd.GetString() : null;
        var sessionId = root.TryGetProperty("sessionId", out var sid) ? sid.GetString() : null;
        var captcha = root.TryGetProperty("captcha", out var cap) ? cap.GetString() : null;
        return (account ?? string.Empty, password ?? string.Empty, sessionId, captcha);
    }

    private static async Task<string> LoginAsync((string account, string password, string? sessionId, string? captcha) req)
    {
        if (!string.IsNullOrWhiteSpace(req.sessionId) && !string.IsNullOrWhiteSpace(req.captcha))
        {
            return await AppState.Services!.C4399.LoginWithPasswordAsync(req.account, req.password, req.sessionId, req.captcha);
        }
        return await AppState.Services!.C4399.LoginWithPasswordAsync(req.account, req.password);
    }

    private static async Task Send(System.Net.WebSockets.WebSocket ws, object payload)
    {
        var text = JsonSerializer.Serialize(payload);
        await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(text)), System.Net.WebSockets.WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
    }

    private static string DumpData(System.Collections.IDictionary data)
    {
        try
        {
            var sb = new System.Text.StringBuilder();
            foreach (var k in data.Keys)
            {
                var v = data[k];
                sb.Append(k).Append('=').Append(v).Append(';');
            }
            return sb.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }
}