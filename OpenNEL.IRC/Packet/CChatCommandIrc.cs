using DotNetty.Buffers;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Packet;
using Serilog;

namespace OpenNEL.IRC.Packet;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ServerBound, 4, EnumProtocolVersion.V1206, false)]
public class CChatCommandIrc : IPacket
{
    public EnumProtocolVersion ClientProtocolVersion { get; set; }

    private byte[]? _rawBytes;
    private string _command = string.Empty;
    private bool _isIrcCommand;

    public void ReadFromBuffer(IByteBuffer buffer)
    {
        _rawBytes = new byte[buffer.ReadableBytes];
        buffer.GetBytes(buffer.ReaderIndex, _rawBytes);

        _command = buffer.ReadStringFromBuffer(32767);
        buffer.SkipBytes(buffer.ReadableBytes);

        _isIrcCommand = _command.StartsWith("irc ", StringComparison.OrdinalIgnoreCase)
                     || _command.Equals("irc", StringComparison.OrdinalIgnoreCase);
    }

    public void WriteToBuffer(IByteBuffer buffer)
    {
        if (_isIrcCommand) return;

        if (_rawBytes != null)
            buffer.WriteBytes(_rawBytes);
    }

    public bool HandlePacket(GameConnection connection)
    {
        if (!_isIrcCommand) return false;

        var content = _command.Length > 4 ? _command.Substring(4).Trim() : string.Empty;

        if (string.IsNullOrWhiteSpace(content))
        {
            SendLocalMessage(connection, "§e[IRC] 用法: /irc <消息>");
            return true;
        }

        var playerName = connection.NickName;
        if (string.IsNullOrEmpty(playerName))
        {
            SendLocalMessage(connection, "§c[IRC] 未登录");
            return true;
        }

        var ircClient = IrcManager.Get(connection);
        if (ircClient == null)
        {
            SendLocalMessage(connection, "§c[IRC] IRC 未连接");
            return true;
        }
        ircClient.SendChat(playerName, content);
        return true;
    }

    private static void SendLocalMessage(GameConnection connection, string message)
    {
        try
        {
            var buffer = Unpooled.Buffer();
            buffer.WriteVarInt(108); 
            
            var textBytes = System.Text.Encoding.UTF8.GetBytes(message);
            buffer.WriteByte(0x08);
            buffer.WriteShort(textBytes.Length);
            buffer.WriteBytes(textBytes);
            buffer.WriteBoolean(false);
            
            connection.ClientChannel?.WriteAndFlushAsync(buffer);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[IRC] 发送本地消息失败");
        }
    }
}
