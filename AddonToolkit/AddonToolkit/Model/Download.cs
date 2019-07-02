using System;

// TODO add string FileSizeString and  int FileSizeBytes
namespace AddonToolkit.Model
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
            ReleaseType = releaseType ?? throw new ArgumentNullException(nameof(releaseType));
            Version = version ?? throw new ArgumentNullException(nameof(version));
            FileSize = fileSize ?? throw new ArgumentNullException(nameof(fileSize));
            DateUploaded = dateUploaded;
            GameVersion = gameVersion ?? throw new ArgumentNullException(nameof(gameVersion));
            NrOfDownloads = nrOfDownloads;
            DownloadLink = downloadLink ?? throw new ArgumentNullException(nameof(downloadLink));
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
    }
}