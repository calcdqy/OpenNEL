namespace OpenNEL.IRC.Entities;

/// <summary>
/// IRC 频道
/// </summary>
public class IrcChannel
{
    /// <summary>
    /// 频道名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 频道主题
    /// </summary>
    public string? Topic { get; set; }

    /// <summary>
    /// 频道用户列表
    /// </summary>
    public HashSet<string> Users { get; } = new(StringComparer.OrdinalIgnoreCase);

    public IrcChannel(string name)
    {
        Name = name;
    }
}
