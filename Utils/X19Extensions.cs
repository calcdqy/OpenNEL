using System.Threading.Tasks;
using Codexus.OpenSDK;
using Codexus.OpenSDK.Entities.X19;

namespace OpenNEL.Utils;

public static class X19Extensions
{
    public static async Task<string> ApiRaw(this X19AuthenticationOtp otp, string url, string body)
    {
        var response = await X19.ApiPostAsync(url, body, otp.EntityId, otp.Token);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return json;
    }
}
