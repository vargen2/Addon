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
    }
}
