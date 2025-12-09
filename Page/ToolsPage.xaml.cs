using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.Net;
using Windows.ApplicationModel.DataTransfer;
using System.Collections.ObjectModel;
using System.IO;
using OpenNEL.Utils;

namespace OpenNEL_WinUI
{
    public sealed partial class ToolsPage : Page
    {
        public static string PageTitle => "工具";
        ObservableCollection<string> _logLines = new ObservableCollection<string>();
        public ToolsPage()
        {
            this.InitializeComponent();
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                string ipv4 = string.Empty;
                string ipv6 = string.Empty;
                foreach (var a in host.AddressList)
                {
                    if (a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(a))
                    {
                        ipv4 = a.ToString();
                        break;
                    }
                }
                foreach (var a in host.AddressList)
                {
                    if (a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 && !System.Net.IPAddress.IsLoopback(a))
                    {
                        var s = a.ToString();
                        var lower = s.ToLowerInvariant();
                        if (lower.StartsWith("fe80") || lower.StartsWith("fc") || lower.StartsWith("fd")) continue;
                        if (a.IsIPv6LinkLocal || a.IsIPv6Multicast || a.IsIPv6SiteLocal) continue;
                        ipv6 = s;
                        break;
                    }
                }
                Ipv4Text.Text = ipv4;
                Ipv6Text.Text = string.IsNullOrWhiteSpace(ipv6) ? "无" : ipv6;
                LogList.ItemsSource = _logLines;
                UiLog.Logged += UiLog_Logged;
                this.Unloaded += ToolsPage_Unloaded;
                try
                {
                    var snap = UiLog.GetSnapshot();
                    foreach (var line in snap) _logLines.Add(line);
                    if (_logLines.Count > 0) LogList.ScrollIntoView(_logLines[_logLines.Count - 1]);
                }
                catch { }
            }
            catch { }
        }

        private void OpenSite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = "https://fandmc.cn/", UseShellExecute = true });
            }
            catch { }
        }

        private void OpenLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var baseDir = System.IO.Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
                var dir = System.IO.Path.Combine(baseDir, "logs");
                Process.Start(new ProcessStartInfo { FileName = dir, UseShellExecute = true });
            }
            catch { }
        }

        private void CopyIpv4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dp = new DataPackage();
                dp.SetText(Ipv4Text.Text ?? string.Empty);
                Clipboard.SetContent(dp);
                NotificationHost.ShowGlobal("已复制", ToastLevel.Success);
            }
            catch { }
        }

        private void CopyIpv6_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dp = new DataPackage();
                dp.SetText(Ipv6Text.Text ?? string.Empty);
                Clipboard.SetContent(dp);
                NotificationHost.ShowGlobal("已复制", ToastLevel.Success);
            }
            catch { }
        }

        void UiLog_Logged(string line)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line)) return;
                DispatcherQueue.TryEnqueue(() =>
                {
                    _logLines.Add(line);
                    if (_logLines.Count > 2000) _logLines.RemoveAt(0);
                    try { LogList.ScrollIntoView(line); } catch { }
                });
            }
            catch { }
        }

        void ToolsPage_Unloaded(object sender, RoutedEventArgs e)
        {
            try { UiLog.Logged -= UiLog_Logged; } catch { }
        }
    }
}
