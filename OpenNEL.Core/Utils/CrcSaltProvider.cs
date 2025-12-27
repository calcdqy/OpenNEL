using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace OpenNEL.Core.Utils;

public static class CrcSaltProvider
{
    private const string DefaultSalt = "E520638AC4C3C93A1188664010769EEC";
    private const string CrcSaltEndpoint = "https://api.fandmc.cn/v1/crcsalt";
    
    private static string _cached = DefaultSalt;
    private static DateTime _lastFetch = DateTime.MinValue;
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1);

    public static string GetCached() => _cached;

    public static async Task<string> GetAsync()
    {
        if (DateTime.UtcNow - _lastFetch < RefreshInterval)
            return _cached;

        try
        {
            var hwid = ComputeHwid();
            using var client = new HttpClient();
            using var content = new StringContent(hwid, Encoding.UTF8, "text/plain");
            var resp = await client.PostAsync(CrcSaltEndpoint, content);
            var json = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                _lastFetch = DateTime.UtcNow;
                return _cached;
            }

            var obj = JsonSerializer.Deserialize<CrcSaltResponse>(json);
            if (obj == null || obj.success != true || string.IsNullOrWhiteSpace(obj.crcSalt))
            {
                _lastFetch = DateTime.UtcNow;
                return _cached;
            }

            _cached = obj.crcSalt;
            _lastFetch = DateTime.UtcNow;
            return _cached;
        }
        catch
        {
            _lastFetch = DateTime.UtcNow;
            return _cached;
        }
    }

    private static string ComputeHwid()
    {
        try
        {
            var os = Environment.OSVersion.VersionString;
            var cpu = Environment.ProcessorCount.ToString();
            var guid = GetMachineGuid();
            var s = string.Join("|", new[] { os, cpu, guid });
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
            return Convert.ToHexString(hash);
        }
        catch
        {
            using var sha = SHA256.Create();
            var fallbackGuid = GetMachineGuid();
            var s = string.Join("|", new[] { Environment.OSVersion.VersionString, Environment.ProcessorCount.ToString(), fallbackGuid });
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
            return Convert.ToHexString(hash);
        }
    }

    private static string GetMachineGuid()
    {
        try
        {
            using var lm64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            using var crypt64 = lm64.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography", false);
            var g64 = crypt64?.GetValue("MachineGuid") as string;
            if (!string.IsNullOrWhiteSpace(g64)) return g64!;

            using var lm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
            using var crypt = lm.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography", false);
            var g = crypt?.GetValue("MachineGuid") as string;
            if (!string.IsNullOrWhiteSpace(g)) return g!;
            return "";
        }
        catch
        {
            return "";
        }
    }

    private record CrcSaltResponse(bool success, string? crcSalt, string? gameVersion, string? error);
}
