using System;
using System.Collections.Generic;
using OpenNEL.Manager;
using OpenNEL.type;
using Serilog;
using System.Text.Json;

namespace OpenNEL_WinUI.Handlers.Game
{
    public class GetServersDetail
    {
        public object Execute(string gameId)
        {
            var last = UserManager.Instance.GetLastAvailableUser();
            if (last == null) return new { type = "notlogin" };
            if (string.IsNullOrWhiteSpace(gameId)) return new { type = "server_detail_error", message = "参数错误" };
            try
            {
                var detail = AppState.X19.QueryNetGameDetailById(last.UserId, last.AccessToken, gameId);
                var dataProp = detail?.GetType().GetProperty("Data");
                var dataVal = dataProp != null ? dataProp.GetValue(detail) : null;
                var imgs = new List<string>();
                if (dataVal != null)
                {
                    var upProp = dataVal.GetType().GetProperty("BriefImageUrls");
                    var lowProp = dataVal.GetType().GetProperty("brief_image_urls");
                    var arr = upProp != null ? upProp.GetValue(dataVal) as System.Collections.IEnumerable : null;
                    if (arr == null && lowProp != null) arr = lowProp.GetValue(dataVal) as System.Collections.IEnumerable;
                    if (arr != null)
                    {
                        foreach (var it in arr)
                        {
                            var s = it != null ? it.ToString() : string.Empty;
                            if (!string.IsNullOrWhiteSpace(s)) imgs.Add(s.Replace("`", string.Empty).Trim());
                        }
                    }
                }
                return new { type = "server_detail", images = imgs.ToArray() };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "获取服务器详情失败: {GameId}", gameId);
                return new { type = "server_detail_error", message = "获取失败" };
            }
        }
    }
}
