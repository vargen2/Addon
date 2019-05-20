using AddonManager.Core.Storage;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AddonManager.Core.Models
{
    public class StoreAddon : INotifyPropertyChanged, IProgressable
    {
        public AddonData AddonData { get; }
        public string Title { get => AddonData.Title; }
        public string Description { get => AddonData.Description; }
        public long NrOfDownloads { get => AddonData.NrOfDownloads; }
        public DateTime Updated { get; }
        public DateTime Created { get; }
        public string UpdatedFormated { get; }
        public string CreatedFormated { get; }

        public StoreAddon(AddonData addonData)
        {
            AddonData = addonData;
            Updated = DateTimeOffset.FromUnixTimeSeconds(addonData.UpdatedEpoch).UtcDateTime;
            Created = DateTimeOffset.FromUnixTimeSeconds(addonData.CreatedEpoch).UtcDateTime;
            UpdatedFormated = Updated.ToString("yyyy'-'MM'-'dd");
            CreatedFormated = Created.ToString("yyyy'-'MM'-'dd");

        }


        public const string INSTALLED = "Installed";
        public const string INSTALLING = "Installing";
        public const string NOTINSTALLED = "Not Installed";
        public const string UNKNOWN = "Unknown";

        public bool IsInstalled => status.Equals(INSTALLED);
        public bool IsInstalling => status.Equals(INSTALLING);
        public bool IsNotInstalled => status.Equals(NOTINSTALLED);
        public bool IsUnknown => status.Equals(UNKNOWN);

        private string status = NOTINSTALLED;
        public string Status
        {
            get => status;
            set
            {
                if (value.Equals(status))
                    return;
                status = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsInstalled");
                NotifyPropertyChanged("IsInstalling");
                NotifyPropertyChanged("IsNotInstalled");
                NotifyPropertyChanged("IsUnknown");


            }
        }

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
        public bool ShowStatus => !status.Equals(NOTINSTALLED) && string.IsNullOrEmpty(Message);

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{nameof(AddonData.Title)}: {AddonData.Title}, {nameof(AddonData.ProjectName)}: {AddonData.ProjectName}";
        }
    }
}
