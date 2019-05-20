using System;
using System.Collections.Generic;

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
            return $"{ReleaseType}, {Version}, {FileSize},\r\nUploaded: {DateUploaded}";
        }

         public string ToStringThreeLines()
        {
            return $"{ReleaseType}, {Version}, {FileSize}, Downloaded: {NrOfDownloads},\r\nUploaded: {DateUploaded}, Game: {GameVersion},\r\n{DownloadLink}";
        }

        public string ToStringManyLines()
        {
            return $"{nameof(ReleaseType)}: {ReleaseType},\r\n{nameof(Version)}: {Version},\r\n{nameof(FileSize)}: {FileSize},\r\n{nameof(DateUploaded)}: {DateUploaded},\r\n{nameof(GameVersion)}: {GameVersion},\r\n{nameof(NrOfDownloads)}: {NrOfDownloads},\r\n{nameof(DownloadLink)}: {DownloadLink}";
        }

        public override bool Equals(object obj)
        {
            return obj is Download download &&
                   Version == download.Version;
        }

        public override int GetHashCode()
        {
            return -1677367089 + EqualityComparer<string>.Default.GetHashCode(Version);
        }



        //public bool Equals(Box b1, Box b2)
        //{
        //    if (b2 == null && b1 == null)
        //        return true;
        //    else if (b1 == null || b2 == null)
        //        return false;
        //    else if (b1.Height == b2.Height && b1.Length == b2.Length
        //                        && b1.Width == b2.Width)
        //        return true;
        //    else
        //        return false;
        //}

        //public int GetHashCode(Box bx)
        //{
        //    int hCode = bx.Height ^ bx.Length ^ bx.Width;
        //    return hCode.GetHashCode();
        //}

    }
}
