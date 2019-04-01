using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Addon.Core.Helpers;

namespace Addon.Core.Models
{
    public class StoreAddon : INotifyPropertyChanged
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


        private static string INSTALLED = "Installed";
        private static string INSTALLING = "Installing";
        private static string NOTINSTALLED = "Not Installed";
        private static string UNKNOWN = "Unknown";

        public bool IsInstalled => status.Equals(INSTALLED);
        public bool IsInstalling => status.Equals(INSTALLING);
        public bool IsNotInstalled => status.Equals(NOTINSTALLED);
        public bool IsUnknown => status.Equals(UNKNOWN);

        private string status;
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
