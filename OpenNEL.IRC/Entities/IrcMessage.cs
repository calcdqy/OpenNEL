namespace OpenNEL.IRC.Entities;

/// <summary>
/// IRC 消息实体
/// </summary>
public class IrcMessage
{
    /// <summary>
    /// 消息来源前缀 (如 nick!user@host)
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// 发送者昵称
    /// </summary>
    public string? Nick { get; set; }

    /// <summary>
    /// 发送者用户名
    /// </summary>
    public string? User { get; set; }

    /// <summary>
    /// 发送者主机
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// IRC 命令 (如 PRIVMSG, JOIN, PING 等)
    /// </summary>
    public string Command { get; set; } = string.Empty;

    /// <summary>
    /// 命令参数列表
    /// </summary>
    public List<string> Params { get; set; } = [];

    /// <summary>
    /// 尾部参数 (冒号后的内容)
    /// </summary>
    public string? Trailing { get; set; }

    /// <summary>
    /// 原始消息
    /// </summary>
    public string Raw { get; set; } = string.Empty;

    /// <summary>
    /// 解析 IRC 消息
    /// </summary>
    /// <example>
    /// :nick!user@host PRIVMSG #channel :Hello, world!
    /// PING :server.example.com
    /// </example>
    public static IrcMessage Parse(string raw)
    {
        var message = new IrcMessage { Raw = raw };
        var index = 0;

        // 解析前缀
        if (raw.StartsWith(':'))
        {
            var spaceIndex = raw.IndexOf(' ');
            if (spaceIndex == -1) return message;

            message.Prefix = raw[1..spaceIndex];
            index = spaceIndex + 1;

            // 解析 nick!user@host
            var prefix = message.Prefix;
            var exclamationIndex = prefix.IndexOf('!');
            var atIndex = prefix.IndexOf('@');

            if (exclamationIndex > 0)
            {
                message.Nick = prefix[..exclamationIndex];
                if (atIndex > exclamationIndex)
                {
                    message.User = prefix[(exclamationIndex + 1)..atIndex];
                    message.Host = prefix[(atIndex + 1)..];
                }
                else
                {
                    message.User = prefix[(exclamationIndex + 1)..];
                }
            }
            else if (atIndex > 0)
            {
                message.Nick = prefix[..atIndex];
                message.Host = prefix[(atIndex + 1)..];
            }
            else
            {
                message.Nick = prefix;
            }
        }

        // 查找尾部参数
        var trailingIndex = raw.IndexOf(" :", index);
        string mainPart;
        if (trailingIndex >= 0)
        {
            mainPart = raw[index..trailingIndex];
            message.Trailing = raw[(trailingIndex + 2)..];
        }
        else
        {
            mainPart = raw[index..];
        }

        // 解析命令和参数
        var parts = mainPart.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 0)
        {
            message.Command = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                message.Params.Add(parts[i]);
            }
        }

        return message;
    }

    public override string ToString()
    {
        return Raw;
    }
}
