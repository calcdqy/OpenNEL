using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using OpenNEL.Entities;
using OpenNEL.Entities.Web;
using OpenNEL.Manager;
using OpenNEL.network;

namespace OpenNEL.HandleWebSocket.Connected;

public class GetAccountMessage : IWsHandler
{
    public string Type => "get_accounts";

    public async Task<object?> ProcessAsync(JsonElement root)
    {
        var mode = root.TryGetProperty("mode", out var m) ? m.GetString() : null;
        if (!string.IsNullOrEmpty(mode))
        {
            if (mode == "available-for-mobile")
            {
                var availableUsers = UserManager.Instance.GetAvailableUsers();
                return new Entity("get_accounts", JsonSerializer.Serialize(availableUsers));
            }
            else
            {
                var availableUsers = UserManager.Instance.GetAvailableUsers();
                return new Entity("get_accounts", JsonSerializer.Serialize(availableUsers));
            }
        }
        else
        {
            List<EntityUser> usersNoDetails = UserManager.Instance.GetUsersNoDetails();
            IEnumerable<EntityUser> value = usersNoDetails.AsEnumerable();
            return new Entity("get_accounts", JsonSerializer.Serialize(value));
        }
    }
}
