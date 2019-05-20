using AddonManager.Core.Helpers;
using AddonManager.Core.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using static System.String;


namespace AddonManager.Core.Models
{
    public class Addon : INotifyPropertyChanged, IProgressable

    {
        public Game Game { get; }
        public string FolderName { get; }
        public string AbsolutePath { get; }
        public string Title { get; set; } = Empty;

        public HashSet<string> SubFolders { get; set; } = new HashSet<string>();


        public Addon(Game game, string folderName, string absolutePath)
        {
            Game = game ?? throw new NullReferenceException();
            FolderName = folderName ?? throw new NullReferenceException();
            AbsolutePath = absolutePath ?? throw new NullReferenceException();
            SetIgnored = new RelayCommand(() => IsIgnored = !IsIgnored);
            SetAutoUpdate = new RelayCommand(() => IsAutoUpdate = !IsAutoUpdate);
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

        private List<Download> downloads = new List<Download>();

        public List<Download> Downloads
        {
            get => downloads;
            set
            {
                downloads = value;
                updateStatus();
                NotifyPropertyChanged();
                NotifyPropertyChanged("InfoString");
                NotifyPropertyChanged("SuggestedDownload");
            }
        }

        public void InsertNewDownloads(List<Download> newDownloads)
        {
            downloads.InsertRange(0, newDownloads);
            updateStatus();
            NotifyPropertyChanged("Downloads");
            NotifyPropertyChanged("InfoString");
            NotifyPropertyChanged("SuggestedDownload");
        }

        private string preferredReleaseType = "Release";

        public string PreferredReleaseType
        {
            get => preferredReleaseType;
            set
            {
                if (value.Equals(preferredReleaseType))
                    return;
                preferredReleaseType = value;
                updateStatus();
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsAlpha");
                NotifyPropertyChanged("IsBeta");
                NotifyPropertyChanged("IsRelease");
                NotifyPropertyChanged("InfoString");
                NotifyPropertyChanged("SuggestedDownload");

            }
        }

        private string version = Empty;

        public string Version
        {
            get => version;
            set
            {
                if (value.Equals(version))
                    return;
                version = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("CurrentReleaseTypeAndVersion");
            }
        }

        public string CurrentReleaseTypeAndVersion => (CurrentDownload != null)
            ? CurrentDownload.ReleaseType + " " + CurrentDownload.Version
            : $"{Version}";

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
                updateStatus();
                NotifyPropertyChanged();
                NotifyPropertyChanged("CurrentReleaseTypeAndVersion");
                NotifyPropertyChanged("InfoString");
                NotifyPropertyChanged("SuggestedDownload");
            }
        }


        public ICommand SetIgnored { get; set; }
        public ICommand SetAutoUpdate { get; set; }
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
                NotifyPropertyChanged("NotIgnoredOpacity");
                NotifyPropertyChanged("InfoString");

            }
        }

        private bool isAutoUpdate;

        public bool IsAutoUpdate
        {
            get => isAutoUpdate;
            set
            {
                if (value == isAutoUpdate)
                    return;
                isAutoUpdate = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("InfoString");

            }
        }

        public double NotIgnoredOpacity => IsIgnored ? 0.3 : 1;
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
                NotifyPropertyChanged("InfoString");
                NotifyPropertyChanged("ShowMessage");
                NotifyPropertyChanged("ShowStatus");

            }
        }





        public Download SuggestedDownload
        {
            get
            {
                if (Downloads.Count == 0)
                {
                    return null;
                }

                return downloads.FirstOrDefault(dl => dl.ReleaseType.ToLower().Equals(preferredReleaseType.ToLower()));
            }
        }


        private void updateStatus()
        {
            if (downloads.Count == 0)
            {
                Status = UNKNOWN;
                return;
            }

            var suggestedDownload = SuggestedDownload;

            if (suggestedDownload == null)
            {
                Status = UNKNOWN;
                return;
            }

            if (currentDownload == null)
            {
                Status = UPDATEABLE;
                return;
            }

            if (suggestedDownload.DateUploaded > currentDownload.DateUploaded)
            {
                Status = UPDATEABLE;
            }
            else
            {
                Status = UP_TO_DATE;
            }


        }


        private string message = "";

        public string Message
        {
            get => message;
            set
            {
                if (value.Equals(message))
                    return;
                message = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("ShowMessage");
                NotifyPropertyChanged("ShowStatus");
            }
        }

        public bool ShowMessage => !string.IsNullOrEmpty(Message);
        public bool ShowStatus => !status.Equals(UPDATEABLE) && string.IsNullOrEmpty(Message);



        private string changeLog;

        public string ChangeLog
        {
            get => changeLog;
            set
            {
                changeLog = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsChangeLogEmpty");
            }
        }

        public bool IsChangeLogEmpty => string.IsNullOrEmpty(ChangeLog);



        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public override string ToString()
        {
            return $"{nameof(FolderName)}: {FolderName}, {nameof(AbsolutePath)}: {AbsolutePath}, {nameof(PreferredReleaseType)}: {PreferredReleaseType}, {nameof(CurrentReleaseTypeAndVersion)}: {CurrentReleaseTypeAndVersion}, {nameof(IsIgnored)}: {IsIgnored}, {nameof(Status)}: {Status}, {nameof(GameVersion)}: {GameVersion}";
        }


        public string InfoString => $"{nameof(Game)}: {Game},\r\n{nameof(FolderName)}: {FolderName}," +
            $"\r\n{nameof(AbsolutePath)}: {AbsolutePath},\r\n{nameof(Title)}: {Title}," +
            $"\r\n{nameof(ProjectUrl)}: {ProjectUrl},\r\n{nameof(PreferredReleaseType)}: {PreferredReleaseType}," +
            $"\r\n{nameof(Version)}: {Version},\r\n{nameof(CurrentReleaseTypeAndVersion)}: {CurrentReleaseTypeAndVersion}," +
            $"\r\n{nameof(IsIgnored)}: {IsIgnored},\r\n{nameof(GameVersion)}: {GameVersion}," +
            $"\r\n{nameof(Status)}: {Status},\r\n{nameof(CurrentDownload)}: {CurrentDownload}," +
            $"\r\n{nameof(SuggestedDownload)}: {SuggestedDownload},\r\n{ nameof(IsAutoUpdate)}: { IsAutoUpdate}" +
            $"\r\n{nameof(SubFolders)}: {SubFolderContents()}";

        private string SubFolderContents()
        {

            return (SubFolders != null) ? string.Join(", ", SubFolders) : string.Empty;
        }

        public SaveableAddon AsSaveableAddon()
        {
            return new SaveableAddon()
            {
                AbsolutePath = AbsolutePath,
                Title = Title,
                CurrentDownload = CurrentDownload,
                Downloads = Downloads,
                FolderName = FolderName,
                GameVersion = GameVersion,
                IsIgnored = IsIgnored,
                IsAutoUpdate = IsAutoUpdate,
                PreferredReleaseType = PreferredReleaseType,
                ProjectUrl = ProjectUrl,

                Status = (this.Status.Equals(DOWNLOADING_VERSIONS) || this.Status.Equals(UPDATING)) ? UNKNOWN : this.Status,
                Version = Version,
                SubFolders = SubFolders
            };
        }

    }
}
