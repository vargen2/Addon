using AddonManager.Core.Models;
using AddonToolkit.Model;
using System.Collections.Generic;
using static AddonToolkit.Model.Enums;

namespace AddonManager.Core.Storage
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
        public bool IsAutoUpdate { get; set; }
        public string GameVersion { get; set; }
        public GAME_TYPE GameType { get; set; }
        public string Status { get; set; }
        public HashSet<string> SubFolders { get; set; }

        public override string ToString()
        {
            return $"{nameof(FolderName)}: {FolderName}, {nameof(AbsolutePath)}: {AbsolutePath}, " +
                $"{nameof(Title)}: {Title}, {nameof(ProjectUrl)}: {ProjectUrl}, " +
                $"{nameof(Downloads)}: {Downloads}, {nameof(PreferredReleaseType)}: {PreferredReleaseType}, " +
                $"{nameof(Version)}: {Version}, {nameof(CurrentDownload)}: {CurrentDownload}, " +
                $"{nameof(IsIgnored)}: {IsIgnored},{ nameof(IsAutoUpdate)}: { IsAutoUpdate}, " +
                $"{ nameof(GameVersion)}: { GameVersion}, { nameof(Status)}: { Status}, { nameof(GameType)}: { GameType}";
        }

        public Addon AsAddon(Game game)
        {
            return new Addon(game, FolderName, AbsolutePath)
            {
                Title = this.Title,
                ProjectUrl = this.ProjectUrl,
                Downloads = this.Downloads,
                PreferredReleaseType = this.PreferredReleaseType,
                Version = this.Version,
                CurrentDownload = this.CurrentDownload,
                IsIgnored = this.IsIgnored,
                IsAutoUpdate = this.IsAutoUpdate,
                GameVersion = this.GameVersion,
                Status = this.Status,
                SubFolders = this.SubFolders ?? new HashSet<string>()
            };
        }
    }
}