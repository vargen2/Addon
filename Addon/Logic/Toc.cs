using Addon.Core.Helpers;
using Addon.Core.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Addon.Logic
{
    public class TocFile
    {
        public StorageFolder StorageFolder { get; }
        public string Version { get; }

        public string GameVersion { get; }

        public string Title { get; }

        public bool IsKnownSubFolder { get; }

        public TocFile(StorageFolder storageFolder, string version, string gameVersion, string title, bool isKnownSubFolder)
        {
            StorageFolder = storageFolder;
            Version = version;
            GameVersion = gameVersion;
            Title = title;
            IsKnownSubFolder = isKnownSubFolder;
        }
    }

    internal static class Toc
    {
        internal static async Task<TocFile> FolderToTocFile(StorageFolder folder)
        {
            var version = String.Empty;
            var gameVersion = String.Empty;
            var title = String.Empty;
            var files = await folder.GetFilesAsync(CommonFileQuery.DefaultQuery);
            var file = files.FirstOrDefault(f => f.FileType.Equals(".toc"));

            if(file==null)
            {
                return null;
            }

            var lines = await FileIO.ReadLinesAsync(file);
            foreach (var line in lines)
            {
                if (line.Contains("Interface:"))
                {
                    gameVersion = line.Substring(line.IndexOf("Interface:") + 10).Trim();
                }

                if (line.Contains("Version:"))
                {
                    version = line.Substring(line.IndexOf("Version:") + 8).Trim();
                }

                if (line.Contains("Title:"))
                {
                    var temp = line.Substring(line.IndexOf("Title:") + 6).Trim();
                    var temp2 = Regex.Replace(temp, "\\|c[a-zA-Z_0-9]{8}", "");
                    title = Regex.Replace(temp2, "\\|r", "");
                }
            }

            return new TocFile(folder, version, gameVersion, title, Singleton<Session>.Instance.KnownSubFolders.Contains(folder.Name));
        }
    }
}
