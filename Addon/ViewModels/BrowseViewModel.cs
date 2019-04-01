using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Addon.Core.Helpers;
using Addon.Core.Models;
using Addon.Helpers;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace Addon.ViewModels
{
    public class BrowseViewModel : Observable
    {
        public ObservableCollection<StoreAddon> StoreAddons { get; set; }

        public BrowseViewModel()
        {
            StoreAddons = Singleton<Session>.Instance.StoreAddons;
        }






    }
}
