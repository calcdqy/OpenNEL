using System;
using System.Text.Json;
using Codexus.Cipher.Entities.G79;
using Codexus.Cipher.Entities.MPay;
using Codexus.Cipher.Entities.WPFLauncher;
using Codexus.Cipher.Protocol;
using Codexus.Cipher.Utils.Exception;
using OpenNEL.Entities;
using OpenNEL.Entities.Web;
using OpenNEL.Entities.Web.NEL;
using OpenNEL.Enums;
using OpenNEL.Manager;
using OpenNEL.network;
using OpenNEL.type;
using Serilog;

namespace OpenNEL.HandleWebSocket.Login;

public class LoginMessage : IWsHandler
{
    public string Type => "login";

    public async Task<object?> ProcessAsync(JsonElement root)
    {
        EntityLoginRequest? entity = JsonSerializer.Deserialize<EntityLoginRequest>(root.GetRawText());
        if (entity == null)
        {
            return new Entity("login", "Success");
        }
        try
        {
            switch (entity.Channel)
            {
                case "send_code":
                    return HandleSendCode(entity);
                case "active":
                    return HandleActive(entity);
                case "active_with_captcha":
                    return HandleActiveWithCaptcha(entity);
                default:
                    return HandleDefaultLogin(entity);
            }
        }
        catch (VerifyException exception)
        {
            return new Entity("login", EntityResponse.Error(1002, exception));
        }
        catch (CaptchaException exception2)
        {
            string append = entity?.Details ?? string.Empty;
            return new Entity("login", EntityResponse.Error(1001, CaptchaException.Clone(exception2, append)));
        }
        catch (Exception exception3)
        {
            Log.Error(exception3, "Login failed");
            return new Entity("login", EntityResponse.Error(1001, exception3));
        }
    }

    private static object HandleSendCode(EntityLoginRequest entity)
    {
        if (!AppState.X19.MPay.SendSmsCodeAsync(entity.Details).GetAwaiter().GetResult())
        {
            throw new Exception("Failed to send SMS code");
        }
        return new Entity("login", EntityResponse.Success(36, string.Empty));
    }

    private static object HandleActive(EntityLoginRequest entity)
    {
        EntityUser? entityUser = UserManager.Instance.GetUserByEntityId(entity.Details);
        if (entityUser == null)
        {
            throw new Exception("User not found");
        }
        if (!entityUser.Authorized)
        {
            LoginWithChannelAndType(entityUser.Channel, entityUser.Type, entityUser.Details, entity.Platform, entity.Token);
        }
        var accounts = UserManager.Instance.GetAvailableUsers();
        return new object[]
        {
            new Entity("login", EntityResponse.Success(string.Empty)),
            new Entity("get_accounts", JsonSerializer.Serialize(accounts))
        };
    }

    private static object HandleActiveWithCaptcha(EntityLoginRequest entity)
    {
        EntityActiveWithCaptcha? entityActiveWithCaptcha = JsonSerializer.Deserialize<EntityActiveWithCaptcha>(entity.Details);
        if (entityActiveWithCaptcha == null)
        {
            throw new Exception("Invalid captcha");
        }
        EntityUser? userByEntityId = UserManager.Instance.GetUserByEntityId(entityActiveWithCaptcha.UserId);
        if (userByEntityId == null)
        {
            throw new Exception("User not found");
        }
        if (!userByEntityId.Authorized)
        {
            EntityPasswordRequest? entityPasswordRequest = JsonSerializer.Deserialize<EntityPasswordRequest>(userByEntityId.Details);
            if (entityPasswordRequest == null)
            {
                throw new Exception("Invalid user");
            }
            string details = JsonSerializer.Serialize(new EntityPasswordRequest
            {
                Account = entityPasswordRequest.Account,
                Password = entityPasswordRequest.Password,
                Captcha = entityActiveWithCaptcha.Captcha,
                CaptchaIdentifier = entityActiveWithCaptcha.Identifier
            });
            LoginWithChannelAndType(userByEntityId.Channel, userByEntityId.Type, details, entity.Platform, entity.Token);
        }
        var accounts = UserManager.Instance.GetAvailableUsers();
        return new object[]
        {
            new Entity("login", EntityResponse.Success(string.Empty)),
            new Entity("get_accounts", JsonSerializer.Serialize(accounts))
        };
    }

    private static object HandleDefaultLogin(EntityLoginRequest entity)
    {
        LoginWithChannelAndType(entity.Channel, entity.Type, entity.Details, entity.Platform, entity.Token);
        var accounts = UserManager.Instance.GetAvailableUsers();
        return new object[]
        {
            new Entity("login", EntityResponse.Success(string.Empty)),
            new Entity("get_accounts", JsonSerializer.Serialize(accounts))
        };
    }

    private static void LoginWithChannelAndType(string channel, string type, string details, Platform platform, string token)
    {
        if (channel != "netease")
        {
            if (channel == "4399pc" && type == "password")
            {
                LoginWith4399Password(details, platform, token);
            }
            return;
        }
        switch (type)
        {
            case "sms":
                LoginWithPhone(channel, details, platform, token);
                break;
            case "cookie":
                {
                    var (authOtpCookie, loginChannelCookie) = AppState.X19.LoginWithCookie(details);
                    Log.Information("Login with cookie: {UserId} Channel: {LoginChannel}", authOtpCookie.EntityId, loginChannelCookie);
                    Log.Debug("User details: {UserId},{Token}", authOtpCookie.Token, authOtpCookie.Token);
                    UserManager.Instance.AddUserToMaintain(authOtpCookie);
                    UserManager.Instance.AddUser(new EntityUser
                    {
                        UserId = authOtpCookie.EntityId,
                        Authorized = true,
                        AutoLogin = false,
                        Channel = loginChannelCookie,
                        Type = type,
                        Details = details
                    }, loginChannelCookie == "netease");
                }
                break;
            case "password":
                LoginWithEmail(details, platform, token);
                break;
        }
    }

    private static void LoginWithPhone(string channel, string details, Platform platform, string token)
    {
        EntityCodeRequest? entityCodeRequest = JsonSerializer.Deserialize<EntityCodeRequest>(details);
        if (entityCodeRequest == null)
        {
            throw new ArgumentException("Invalid phone login details");
        }
        WPFLauncher x = AppState.X19;
        EntitySmsTicket result = x.MPay.VerifySmsCodeAsync(entityCodeRequest.Phone, entityCodeRequest.Code).GetAwaiter().GetResult();
        if (result == null)
        {
            throw new Exception("Failed to verify SMS code");
        }
        EntityX19CookieRequest value = WPFLauncher.GenerateCookie(x.MPay.FinishSmsCodeAsync(entityCodeRequest.Phone, result.Ticket).GetAwaiter().GetResult() ?? throw new Exception("Failed to finish SMS code"), x.MPay.GetDevice());
        LoginWithChannelAndType(channel, "cookie", JsonSerializer.Serialize(value), platform, token);
    }

    private static void LoginWith4399Password(string details, Platform platform, string token)
    {
        EntityPasswordRequest? entityPasswordRequest = JsonSerializer.Deserialize<EntityPasswordRequest>(details);
        if (entityPasswordRequest == null)
        {
            throw new ArgumentException("Invalid password login details");
        }
        if (platform == Platform.Mobile)
        {
            Log.Information("Waiting for 4399 login...");
            string result = AppState.Com4399.LoginAndAuthorize(entityPasswordRequest.Account, entityPasswordRequest.Password).GetAwaiter().GetResult();
            var (authOtp4399Mobile, loginChannel4399Mobile) = AppState.X19.LoginWithCookie(result);
            Log.Information("Login with 4399 password on mobile: {UserId}", authOtp4399Mobile.EntityId);
            Log.Debug("User details: {UserId},{Token}", authOtp4399Mobile.EntityId, authOtp4399Mobile.Token);
            UserManager.Instance.AddUserToMaintain(authOtp4399Mobile);
            UserManager.Instance.AddUser(new EntityUser
            {
                UserId = authOtp4399Mobile.EntityId,
                Authorized = true,
                AutoLogin = false,
                Channel = loginChannel4399Mobile,
                Type = "password",
                Details = JsonSerializer.Serialize(new EntityPasswordRequest
                {
                    Account = entityPasswordRequest.Account,
                    Password = entityPasswordRequest.Password
                })
            });
            return;
        }
        using Pc4399 pc = new Pc4399();
        string result2 = pc.LoginWithPasswordAsync(entityPasswordRequest.Account, entityPasswordRequest.Password, entityPasswordRequest.CaptchaIdentifier, entityPasswordRequest.Captcha).GetAwaiter().GetResult();
        var (authOtp4399Pc, loginChannel4399Pc) = AppState.X19.LoginWithCookie(result2);
        Log.Information("Login with password: {UserId} Channel: {LoginChannel}", authOtp4399Pc.EntityId, loginChannel4399Pc);
        Log.Debug("User details: {UserId},{Token}", authOtp4399Pc.EntityId, authOtp4399Pc.Token);
        UserManager.Instance.AddUserToMaintain(authOtp4399Pc);
        UserManager.Instance.AddUser(new EntityUser
        {
            UserId = authOtp4399Pc.EntityId,
            Authorized = true,
            AutoLogin = false,
            Channel = loginChannel4399Pc,
            Type = "password",
            Details = JsonSerializer.Serialize(new EntityPasswordRequest
            {
                Account = entityPasswordRequest.Account,
                Password = entityPasswordRequest.Password
            })
        });
    }

    private static void LoginWithEmail(string details, Platform platform, string token)
    {
        EntityPasswordRequest? entityPasswordRequest = JsonSerializer.Deserialize<EntityPasswordRequest>(details);
        if (entityPasswordRequest == null)
        {
            throw new ArgumentException("Invalid email login details");
        }
        WPFLauncher x = AppState.X19;
        EntityX19CookieRequest entityX19CookieRequest = WPFLauncher.GenerateCookie(x.LoginWithEmailAsync(entityPasswordRequest.Account, entityPasswordRequest.Password).GetAwaiter().GetResult(), x.MPay.GetDevice());
        var (authOtpEmail, loginChannelEmail) = x.LoginWithCookie(entityX19CookieRequest);
        Log.Information("Login with email: {UserId} Channel: {LoginChannel}", authOtpEmail.EntityId, loginChannelEmail);
        Log.Debug("User details: {UserId},{Token}", authOtpEmail.EntityId, authOtpEmail.Token);
        UserManager.Instance.AddUserToMaintain(authOtpEmail);
        UserManager.Instance.AddUser(new EntityUser
        {
            UserId = authOtpEmail.EntityId,
            Authorized = true,
            AutoLogin = false,
            Channel = loginChannelEmail,
            Type = "cookie",
            Details = JsonSerializer.Serialize(entityX19CookieRequest)
        });
    }
}
