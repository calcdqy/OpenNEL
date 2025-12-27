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
using System.Linq;
using OpenNEL_WinUI.type;
using OpenNEL_WinUI.Manager;
using Serilog;

namespace OpenNEL_WinUI.Handlers.Game.RentalServer;

public class CreateRentalRole
{
    public object Execute(string serverId, string roleName)
    {
        Log.Debug("[RentalServer] CreateRentalRole: serverId={ServerId}, roleName={RoleName}", serverId, roleName);
        var last = UserManager.Instance.GetLastAvailableUser();
        if (last == null)
        {
            Log.Debug("[RentalServer] CreateRentalRole: 用户未登录");
            return new { type = "notlogin" };
        }
        if (string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(roleName))
        {
            return new { type = "create_role_error", message = "参数错误" };
        }
        try
        {
            Log.Debug("[RentalServer] CreateRentalRole 调用参数: userId={UserId}, serverId={ServerId}, roleName={RoleName}", 
                last.UserId, serverId, roleName);
            
            var result = AppState.X19.AddRentalGameRole(last.UserId, last.AccessToken, serverId, roleName);
            Log.Debug("[RentalServer] AddRentalGameRole result: Code={Code}, Message={Message}", result.Code, result.Message);
            
            if (result.Code != 0)
            {
                Log.Error("[RentalServer] 创建角色失败: {Message}", result.Message);
                return new { type = "create_role_error", message = result.Message ?? "创建失败" };
            }

            var entities = AppState.X19.GetRentalGameRolesList(last.UserId, last.AccessToken, serverId);
            var items = entities.Data.Select(r => new { id = r.Name, name = r.Name }).ToArray();
            Log.Information("[RentalServer] 角色创建成功: serverId={ServerId}, name={RoleName}", serverId, roleName);
            return new { type = "server_roles", items, serverId };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[RentalServer] 创建租赁服角色失败: serverId={ServerId}", serverId);
            return new { type = "create_role_error", message = "创建失败" };
        }
    }
}
