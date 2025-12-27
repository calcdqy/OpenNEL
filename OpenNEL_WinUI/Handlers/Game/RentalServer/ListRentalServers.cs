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
using System.Linq;
using OpenNEL_WinUI.Manager;
using OpenNEL_WinUI.type;
using Serilog;

namespace OpenNEL_WinUI.Handlers.Game.RentalServer;

public class ListRentalServers
{
    public object Execute(int offset, int limit)
    {
        Log.Debug("[RentalServer] ListRentalServers.Execute: offset={Offset}, limit={Limit}", offset, limit);
        
        var user = UserManager.Instance.GetLastAvailableUser();
        if (user == null)
        {
            Log.Debug("[RentalServer] ListRentalServers: 用户未登录");
            return new { type = "notlogin" };
        }
        
        Log.Debug("[RentalServer] ListRentalServers: userId={UserId}", user.UserId);

        var result = AppState.X19.GetRentalGameList(user.UserId, user.AccessToken, offset);
        Log.Debug("[RentalServer] GetRentalGameList result: Code={Code}, Count={Count}", result.Code, result.Data?.Count() ?? 0);
        
        var items = new System.Collections.Generic.List<object>();
        var hasMore = false;
        
        if (result.Data != null)
        {
            var count = 0;
            foreach (var item in result.Data)
            {
                count++;
                items.Add(new
                {
                    entityId = item.EntityId,
                    name = string.IsNullOrEmpty(item.ServerName) ? item.Name : item.ServerName,
                    serverName = item.ServerName,
                    playerCount = (int)item.PlayerCount,
                    hasPassword = item.HasPassword == "1",
                    mcVersion = item.McVersion
                });
            }
            hasMore = count >= limit;
        }
        
        Log.Debug("[RentalServer] ListRentalServers: 找到 {Count} 个服务器, hasMore={HasMore}", items.Count, hasMore);
        return new { type = "rental_servers", items, hasMore };
    }
}
