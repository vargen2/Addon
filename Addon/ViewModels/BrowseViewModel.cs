using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using Addon.Core.Helpers;
using Addon.Core.Models;
using Addon.Helpers;

namespace Addon.ViewModels
{
    public class BrowseViewModel : Observable
    {
        public Session Session { get; }

        public BrowseViewModel()
        {
            Session = Singleton<Session>.Instance;
            
        }
    }
}
