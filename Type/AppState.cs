using System;
using Codexus.Cipher.Protocol;

namespace OpenNEL.type;
using System.Collections.Concurrent;

internal static class AppState
{
    private static Com4399? _com4399;
    public static Com4399 Com4399 => _com4399 ??= new Com4399();

    private static G79? _g79;
    public static G79 G79 => _g79 ??= new G79();

    private static WPFLauncher? _x19;
    public static WPFLauncher X19 => _x19 ??= new WPFLauncher();
    
    public static Services? Services;
    public static ConcurrentDictionary<string, bool> WaitRestartPlugins { get; } = new();
    public static bool Debug;
    public static bool Pre = AppInfo.AppVersion.Contains("pre", StringComparison.OrdinalIgnoreCase);
}
