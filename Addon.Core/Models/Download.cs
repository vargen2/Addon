using System;
using System.Collections.Generic;
using System.Text;

namespace Addon.Core.Models
{
    public class Download
    {
        public string ReleaseType { get; }
        public string Version { get; }
        public string FileSize { get; }
        public DateTime DateUploaded { get; }
        public string GameVersion { get; }
        public long NrOfDownloads { get; }
        public string DownloadLink { get; }

        public Download(string releaseType, string version, string fileSize, DateTime dateUploaded, string gameVersion, long nrOfDownloads, string downloadLink)
        {
            ReleaseType = releaseType;
            Version = version;
            FileSize = fileSize;
            DateUploaded = dateUploaded;
            GameVersion = gameVersion;
            NrOfDownloads = nrOfDownloads;
            DownloadLink = downloadLink;
        }

        public override string ToString()
        {
            return $"{nameof(ReleaseType)}: {ReleaseType},\r\n{nameof(Version)}: {Version},\r\n{nameof(FileSize)}: {FileSize},\r\n{nameof(DateUploaded)}: {DateUploaded},\r\n{nameof(GameVersion)}: {GameVersion},\r\n{nameof(NrOfDownloads)}: {NrOfDownloads},\r\n{nameof(DownloadLink)}: {DownloadLink}";
        }
    }
}
