/*
<OpenNEL>
Copyright (C) <2025>  <OpenNEL>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using OpenNEL.MPay.Entities;
using OpenNEL.MPay.Exceptions;
using OpenNEL.WPFLauncher.Entities;
using OpenNEL.Pc4399;
using OpenNEL_WinUI.Entities;
using OpenNEL_WinUI.Entities.Web;
using OpenNEL_WinUI.Entities.Web.NEL;
using OpenNEL_WinUI.Enums;
using OpenNEL_WinUI.Manager;
using OpenNEL_WinUI.type;
using Serilog;

namespace OpenNEL_WinUI.Handlers.Login
{
    public static class LoginHandler
    {
        private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static void LoginWithChannelAndType(string channel, string type, string details, Platform platform, string token)
        {
            Log.Information("[LoginHandler] LoginWithChannelAndType: channel={Channel}, type={Type}", channel, type);
            LoginWith4399Password(details, platform, token);
        }
        
        private static void LoginWith4399Password(string details, Platform platform, string token)
        {
            Log.Debug("[LoginHandler] LoginWith4399Password: details长度={Len}", details?.Length ?? 0);
            EntityPasswordRequest? entityPasswordRequest = JsonSerializer.Deserialize<EntityPasswordRequest>(details);
            if (entityPasswordRequest == null)
            {
                Log.Error("[LoginHandler] 无法解析EntityPasswordRequest");
                throw new ArgumentException("Invalid password login details");
            }
            Log.Information("[LoginHandler] 解析成功: Account={Account}, 有密码={HasPwd}, CaptchaId={CaptchaId}", 
                entityPasswordRequest.Account, 
                !string.IsNullOrEmpty(entityPasswordRequest.Password),
                entityPasswordRequest.CaptchaIdentifier);
            using Pc4399Client pc = new Pc4399Client();
            string result2;
            try
            {
                Log.Information("[LoginHandler] 开始调用Pc4399Client.LoginWithPasswordAsync");
                result2 = pc.LoginWithPasswordAsync(entityPasswordRequest.Account, entityPasswordRequest.Password, entityPasswordRequest.CaptchaIdentifier, entityPasswordRequest.Captcha).GetAwaiter().GetResult();
                Log.Debug("[LoginHandler] Pc4399登录返回: 长度={Len}", result2?.Length ?? 0);
            }
            catch (CaptchaException ex)
            {
                Log.Warning("[LoginHandler] 需要验证码: {Msg}", ex.Message);
                throw new CaptchaException("captcha required");
            }
            if (string.IsNullOrWhiteSpace(result2))
            {
                Log.Error("[LoginHandler] cookie为空");
                throw new Exception("cookie empty");
            }
            Log.Debug("[LoginHandler] 开始解析cookie: {Cookie}", result2);
            EntityX19CookieRequest cookieReq;
            try
            {
                cookieReq = JsonSerializer.Deserialize<EntityX19CookieRequest>(result2) ?? new EntityX19CookieRequest { Json = result2 };
                Log.Debug("[LoginHandler] 解析为EntityX19CookieRequest成功");
            }
            catch (Exception ex)
            {
                Log.Debug("[LoginHandler] 解析EntityX19CookieRequest失败，使用原始Json: {Err}", ex.Message);
                cookieReq = new EntityX19CookieRequest { Json = result2 };
            }
            Log.Information("[LoginHandler] 开始调用X19.LoginWithCookie");
            var (entityAuthenticationOtp2, text) = AppState.X19.LoginWithCookie(cookieReq);
            Log.Information("[LoginHandler] X19登录成功: EntityId={UserId}, Channel={LoginChannel}", entityAuthenticationOtp2.EntityId, text);
            Log.Debug("User details: {UserId},{Token}", entityAuthenticationOtp2.EntityId, entityAuthenticationOtp2.Token);
            UserManager.Instance.AddUserToMaintain(entityAuthenticationOtp2);
            UserManager.Instance.AddUser(new EntityUser
            {
                UserId = entityAuthenticationOtp2.EntityId,
                Authorized = true,
                AutoLogin = false,
                Channel = text,
                Type = "password",
                Details = JsonSerializer.Serialize(new EntityPasswordRequest
                {
                    Account = entityPasswordRequest.Account,
                    Password = entityPasswordRequest.Password
                }, DefaultOptions)
            });
        }
    }
}
