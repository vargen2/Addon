using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Addon.Core.Models
{
    public class Addon : INotifyPropertyChanged
    {
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (value == _title)
                    return;
                _title = value;
                NotifyPropertyChanged();
                //OnPropertyChanged("Title");
            }
        }





        public string Version { get; set; }
        public string Status { get; set; }
        public string GameVersion { get; set; }
        public string ReleaseType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //protected void OnPropertyChanged(string name)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(name));
        //    }
        //}

        public override string ToString()
        {
            return base.ToString() + " " + Title;
        }

       
    }
}
