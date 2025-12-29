using OpenNEL.IRC.Entities;

namespace OpenNEL.IRC.Events;

/// <summary>
/// 断开连接事件参数
/// </summary>
public class DisconnectedEventArgs(string reason) : EventArgs
{
    public string Reason { get; } = reason;
}

/// <summary>
/// 原始消息事件参数
/// </summary>
public class RawMessageEventArgs(string raw, IrcMessage message) : EventArgs
{
    public string Raw { get; } = raw;
    public IrcMessage Message { get; } = message;
}

/// <summary>
/// 频道消息事件参数
/// </summary>
public class ChannelMessageEventArgs(string channel, string sender, string message) : EventArgs
{
    public string Channel { get; } = channel;
    public string Sender { get; } = sender;
    public string Message { get; } = message;
}

/// <summary>
/// 私聊消息事件参数
/// </summary>
public class PrivateMessageEventArgs(string sender, string message) : EventArgs
{
    public string Sender { get; } = sender;
    public string Message { get; } = message;
}

/// <summary>
/// 加入频道事件参数
/// </summary>
public class JoinEventArgs(string channel, string nick) : EventArgs
{
    public string Channel { get; } = channel;
    public string Nick { get; } = nick;
}

/// <summary>
/// 离开频道事件参数
/// </summary>
public class PartEventArgs(string channel, string nick, string? reason) : EventArgs
{
    public string Channel { get; } = channel;
    public string Nick { get; } = nick;
    public string? Reason { get; } = reason;
}

/// <summary>
/// 用户退出事件参数
/// </summary>
public class QuitEventArgs(string nick, string? reason) : EventArgs
{
    public string Nick { get; } = nick;
    public string? Reason { get; } = reason;
}

/// <summary>
/// 昵称更改事件参数
/// </summary>
public class NickChangeEventArgs(string oldNick, string newNick) : EventArgs
{
    public string OldNick { get; } = oldNick;
    public string NewNick { get; } = newNick;
}

/// <summary>
/// MOTD 事件参数
/// </summary>
public class MotdEventArgs(string text) : EventArgs
{
    public string Text { get; } = text;
}
