using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Addon.Core.Helpers;
using Addon.Core.Models;
using Addon.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace Addon.ViewModels
{
    public class MasterDetailViewModel : Observable
    {
        public Session Session { get; }

        private Addon.Core.Models.Addon _selected;

        public Addon.Core.Models.Addon Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

       // public ObservableCollection<Addon.Core.Models.Addon> SampleItems { get; private set; } = new ObservableCollection<Addon.Core.Models.Addon>();

        public MasterDetailViewModel()
        {
             Session = Singleton<Session>.Instance;
        }

        //public async Task LoadDataAsync(MasterDetailsViewState viewState)
        //{
        //    SampleItems.Clear();

        //    var data = Singleton<Session>.Instance.SelectedGame.Addons;

        //    foreach (var item in data)
        //    {
        //        SampleItems.Add(item);
        //    }

        //    if (viewState == MasterDetailsViewState.Both)
        //    {
        //        Selected = SampleItems.First();
        //    }
        //}
    }
}
