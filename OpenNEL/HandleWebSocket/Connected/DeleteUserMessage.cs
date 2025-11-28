using System.Text.Json;
using OpenNEL.Entities;
using OpenNEL.Manager;
using OpenNEL.network;

namespace OpenNEL.HandleWebSocket.Connected;

internal class DeleteUserMessage : IWsHandler
{
    public string Type => "delete_user";

    public async Task<object?> ProcessAsync(JsonElement root)
    {
        var id = root.TryGetProperty("id", out var v) ? v.ToString() : string.Empty;
        if (string.IsNullOrWhiteSpace(id)) return null;
        UserManager.Instance.RemoveUser(id);
        UserManager.Instance.RemoveAvailableUser(id);
        var accounts = UserManager.Instance.GetAvailableUsers();
        return new Entity("get_accounts", System.Text.Json.JsonSerializer.Serialize(accounts));
    }
}
