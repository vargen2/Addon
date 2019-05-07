using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Addon.Core.Models
{
    public class StoreAddon : INotifyPropertyChanged, IProgressable
    {

        public string Url { get; }
        public string Title { get; }
        public string Description { get; }
        public long NrOfDownloads { get; }

        public DateTime Updated { get; }
        public DateTime Created { get; }

        public string UpdatedFormated
        {
            get { return Updated.ToString("yyyy'-'MM'-'dd"); }
        }
        public string CreatedFormated
        {
            get { return Created.ToString("yyyy'-'MM'-'dd"); }
        }

        public StoreAddon(string url, string title, string description, long nrOfDownloads, DateTime updated, DateTime created)
        {
            Url = url;
            Title = title;
            Description = description;
            NrOfDownloads = nrOfDownloads;
            Updated = updated;
            Created = created;
        }


        public const string INSTALLED = "Installed";
        public const string INSTALLING = "Installing";
        public const string NOTINSTALLED = "Not Installed";
        public const string UNKNOWN = "Unknown";

        public bool IsInstalled => status.Equals(INSTALLED);
        public bool IsInstalling => status.Equals(INSTALLING);
        public bool IsNotInstalled => status.Equals(NOTINSTALLED);
        public bool IsUnknown => status.Equals(UNKNOWN);
        //public bool IsShowTextBlock => !status.Equals(NOTINSTALLED);

        private string status=NOTINSTALLED;
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
                NotifyPropertyChanged("IsShowTextBlock");

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
            return $"{nameof(Title)}: {Title}, {nameof(Url)}: {Url}, {nameof(Description)}: {Description}, {nameof(NrOfDownloads)}: {NrOfDownloads}, {nameof(UpdatedFormated)}: {UpdatedFormated}, {nameof(CreatedFormated)}: {CreatedFormated}, {nameof(Status)}: {Status}";
        }
    }
}
