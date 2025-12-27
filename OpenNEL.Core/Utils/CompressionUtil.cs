using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;

namespace OpenNEL.Core.Utils;

public static class CompressionUtil
{
    public static bool Extract7Z(string filePath, string outPath, Action<int> progressAction)
    {
        try
        {
            using IArchive archive = ArchiveFactory.Open(filePath);
            int totalEntries = archive.Entries.Count(e => !e.IsDirectory);
            int processed = 0;
            foreach (IArchiveEntry entry in archive.Entries)
            {
                if (entry != null && !entry.IsDirectory && entry.Key != null)
                {
                    string path = Path.Combine(outPath, entry.Key);
                    string? directoryName = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    using Stream stream = entry.OpenEntryStream();
                    using FileStream destination = File.Create(path);
                    stream.CopyTo(destination);
                    processed++;
                    progressAction((int)((double)processed / totalEntries * 100.0));
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Failed to extract archive {FilePath}", filePath);
            return false;
        }
    }

    private static bool ExtractZip(string filePath, string outPath, Action<int> progressAction)
    {
        try
        {
            using ZipArchive archive = ZipArchive.Open(filePath);
            IArchiveExtensions.ExtractToDirectory(archive, outPath, dp =>
            {
                progressAction((int)(dp * 100.0));
            });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task Extract7ZAsync(string archivePath, string outputDir, Action<int>? progress = null)
    {
        await Task.Run(() =>
        {
            using IArchive archive = ArchiveFactory.Open(archivePath);
            int totalEntries = archive.Entries.Count();
            int processed = 0;
            foreach (IArchiveEntry entry in archive.Entries)
            {
                if (entry != null && !entry.IsDirectory && entry.Key != null)
                {
                    string path = Path.Combine(outputDir, entry.Key);
                    string? directoryName = Path.GetDirectoryName(path);
                    if (directoryName == null)
                    {
                        throw new ArgumentException("Invalid directory name");
                    }
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    using Stream stream = entry.OpenEntryStream();
                    using FileStream destination = File.Create(path);
                    stream.CopyTo(destination);
                }
                processed++;
                progress?.Invoke((int)((double)processed / totalEntries * 100.0));
            }
        });
    }
}
