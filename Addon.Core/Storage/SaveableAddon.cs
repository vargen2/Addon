using Addon.Core.Models;
using System.Collections.Generic;

namespace Addon.Core.Storage
{
    public class SaveableAddon
    {
        public string FolderName { get; set; }
        public string AbsolutePath { get; set; }
        public string Title { get; set; }
        public string ProjectUrl { get; set; }
        public List<Download> Downloads { get; set; }
        public string PreferredReleaseType { get; set; }
        public string Version { get; set; }
        public Download CurrentDownload { get; set; }
        public bool IsIgnored { get; set; }
        public string GameVersion { get; set; }
        public string Status { get; set; }
        public HashSet<string> SubFolders { get; set; }


        public override string ToString()
        {
            return $"{nameof(FolderName)}: {FolderName}, {nameof(AbsolutePath)}: {AbsolutePath}, " +
                $"{nameof(Title)}: {Title}, {nameof(ProjectUrl)}: {ProjectUrl}, " +
                $"{nameof(Downloads)}: {Downloads}, {nameof(PreferredReleaseType)}: {PreferredReleaseType}, " +
                $"{nameof(Version)}: {Version}, {nameof(CurrentDownload)}: {CurrentDownload}, " +
                $"{nameof(IsIgnored)}: {IsIgnored}, {nameof(GameVersion)}: {GameVersion}, " +
                $"{nameof(Status)}: {Status}";
        }

        public Models.Addon AsAddon(Game game)
        {
            return new Models.Addon(game, this.FolderName, this.AbsolutePath)
            {
                Title = this.Title,
                ProjectUrl = this.ProjectUrl,
                Downloads = this.Downloads,
                PreferredReleaseType = this.PreferredReleaseType,
                Version = this.Version,
                CurrentDownload = this.CurrentDownload,
                IsIgnored = this.IsIgnored,
                GameVersion = this.GameVersion,
                Status = this.Status,
                SubFolders = this.SubFolders ?? new HashSet<string>()
            };
        }

     
    }
}
