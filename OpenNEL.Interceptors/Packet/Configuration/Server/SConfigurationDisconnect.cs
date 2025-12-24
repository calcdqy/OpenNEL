using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Entities;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Packet;
using OpenNEL.SDK.Utils;
using DotNetty.Buffers;
using Serilog;

namespace OpenNEL.Interceptors.Packet.Configuration.Server;

[RegisterPacket(EnumConnectionState.Configuration, EnumPacketDirection.ClientBound, 0x02, EnumProtocolVersion.V1206,
	false)]
public class SConfigurationDisconnect : IPacket
{
	public TextComponent Reason { get; set; } = new();

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	private const string BJDGameId = "4661334467366178884";

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		Reason = TextComponentSerializer.Deserialize(buffer);
		Log.Debug("[Disconnect] Parsed: Text={Text}, Extra.Count={Count}", Reason.Text, Reason.Extra.Count);
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		var serialized = TextComponentSerializer.Serialize(Reason);
		buffer.WriteBytes(serialized);
	}

	public bool HandlePacket(GameConnection connection)
	{
		var displayText = Reason.DisplayText;
		Log.Information("[Disconnect] GameId={GameId}, Reason={Reason}", connection.GameId, displayText);

		if (IsBanMessage(displayText))
		{
			if (Interceptor.AutoDisconnectOnBan)
			{
				var interceptorId = connection.InterceptorId;
				_ = Task.Run(async () =>
				{
					await Task.Delay(500);
					Log.Warning("正在关闭 Interceptor...");
					Interceptor.OnShutdownInterceptor?.Invoke(interceptorId);
				});
			}
		}

		if (connection.GameId == BJDGameId &&
		    displayText.Contains("invalidSession", StringComparison.OrdinalIgnoreCase))
		{
			Log.Warning("[布吉岛] 检测到协议异常");

			Reason = new TextComponent
			{
				Text = "[OpenNEL] 协议未正确工作，请检查插件是否正确安装",
				Color = "red"
			};
		}

		return false;
	}

	private static bool IsBanMessage(string message)
	{
		if (string.IsNullOrEmpty(message)) return false;

		if (message.Contains("封禁", StringComparison.OrdinalIgnoreCase))
			return true;

		return false;
	}
}