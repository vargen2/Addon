using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Addon.Core.Helpers;
using Addon.Core.Storage;
using static System.String;


namespace Addon.Core.Models
{
    public class Addon : INotifyPropertyChanged

    {
        public Game Game { get; }
        public string FolderName { get; }
        public string AbsolutePath { get; }
        public string Title { get; set; } = Empty;

        public Addon(Game game, string folderName, string absolutePath)
        {
            Game = game ?? throw new NullReferenceException(); ;
            FolderName = folderName ?? throw new NullReferenceException(); ;
            AbsolutePath = absolutePath ?? throw new NullReferenceException(); ;
            SetIgnored = new RelayCommand(() => IsIgnored = !IsIgnored);
            SetAlpha = new RelayCommand(() => PreferredReleaseType = "Alpha");
            SetBeta = new RelayCommand(() => PreferredReleaseType = "Beta");
            SetRelease = new RelayCommand(() => PreferredReleaseType = "Release");
        }

        private string projectUrl = Empty;

        public string ProjectUrl
        {
            get => projectUrl;
            set
            {
                if (value == null || value.Equals(projectUrl))
                    return;
                projectUrl = value;
                NotifyPropertyChanged();
            }
        }

        public List<Download> Downloads { get; set; } = new List<Download>();

        private string preferredReleaseType = "Release";

        public string PreferredReleaseType
        {
            get => preferredReleaseType;
            set
            {
                if (value.Equals(preferredReleaseType))
                    return;
                preferredReleaseType = value;
                NotifyPropertyChanged("IsAlpha");
                NotifyPropertyChanged("IsBeta");
                NotifyPropertyChanged("IsRelease");
                NotifyPropertyChanged();
            }
        }

        private string version = Empty;
        public string Version
        {
            get => version; set
            {
                if (value.Equals(version))
                    return;
                version = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("CurrentReleaseTypeAndVersion");
            }
        }

        public string CurrentReleaseTypeAndVersion => (CurrentDownload != null) ? CurrentDownload.ReleaseType + " " + CurrentDownload.Version : $"{Version}";

        //public string ReleaseType_Version => $"{ReleaseType} {Version}";
        private Download currentDownload;
        public Download CurrentDownload
        {
            get => currentDownload;
            set
            {
                if (currentDownload != null && value == currentDownload)
                    return;
                currentDownload = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("CurrentReleaseTypeAndVersion");
            }
        }


        public ICommand SetIgnored { get; set; }
        public ICommand SetAlpha { get; set; }
        public ICommand SetBeta { get; set; }
        public ICommand SetRelease { get; set; }

        private bool isIgnored;
        public bool IsIgnored
        {
            get => isIgnored;
            set
            {
                if (value == isIgnored)
                    return;
                isIgnored = value;
                NotifyPropertyChanged();

            }
        }

        public bool IsAlpha => PreferredReleaseType.ToLower().Equals("alpha");
        public bool IsBeta => PreferredReleaseType.ToLower().Equals("beta");
        public bool IsRelease => PreferredReleaseType.ToLower().Equals("release");




        private int progress;
        public int Progress
        {
            get => progress;
            set
            {
                if (value == progress)
                    return;
                progress = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsIndeterminate");
            }
        }

        public bool IsIndeterminate => progress == 0;


        public string GameVersion { get; set; } = Empty;

        public const string INITIALIZED = "Initialized";
        public const string DOWNLOADING_VERSIONS = "Downloading Versions";
        public const string UPDATEABLE = "Updateable";
        public const string UPDATING = "Updating";
        public const string UP_TO_DATE = "Up to date";
        public const string UNKNOWN = "Unknown";



        public bool IsUpdateable => status.Equals(UPDATEABLE);
        public bool IsNotUpdateable => !status.Equals(UPDATEABLE);

        public bool IsProgressing => status == UPDATING || status == DOWNLOADING_VERSIONS;

        private string status = INITIALIZED;
        public string Status
        {
            get => status;
            set
            {
                if (value.Equals(status))
                    return;
                status = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsUpdateable");
                NotifyPropertyChanged("IsNotUpdateable");
                NotifyPropertyChanged("IsProgressing");
            }
        }









        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public override string ToString()
        {
            return $"{nameof(FolderName)}: {FolderName}, {nameof(AbsolutePath)}: {AbsolutePath}, {nameof(PreferredReleaseType)}: {PreferredReleaseType}, {nameof(CurrentReleaseTypeAndVersion)}: {CurrentReleaseTypeAndVersion}, {nameof(IsIgnored)}: {IsIgnored}, {nameof(Status)}: {Status}, {nameof(GameVersion)}: {GameVersion}";
        }


        public string InfoString => $"{nameof(Game)}: {Game},\r\n{nameof(FolderName)}: {FolderName},\r\n{nameof(AbsolutePath)}: {AbsolutePath},\r\n{nameof(Title)}: {Title},\r\n{nameof(ProjectUrl)}: {ProjectUrl},\r\n{nameof(PreferredReleaseType)}: {PreferredReleaseType},\r\n{nameof(Version)}: {Version},\r\n{nameof(CurrentReleaseTypeAndVersion)}: {CurrentReleaseTypeAndVersion},\r\n{nameof(IsIgnored)}: {IsIgnored},\r\n{nameof(GameVersion)}: {GameVersion},\r\n{nameof(Status)}: {Status}";

        public SaveableAddon AsSaveableAddon()
        {
            return new SaveableAddon()
            {
                AbsolutePath = this.AbsolutePath,
                Title = this.Title,
                CurrentDownload = this.CurrentDownload,
                Downloads = this.Downloads,
                FolderName = this.FolderName,
                GameVersion = this.GameVersion,
                IsIgnored = this.IsIgnored,
                PreferredReleaseType = this.PreferredReleaseType,
                ProjectUrl = this.ProjectUrl,
                Status = this.Status,
                Version = this.Version
            };
        }

    }
}
