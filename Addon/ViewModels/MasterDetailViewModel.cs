using Addon.Core.Helpers;
using Addon.Core.Models;
using Addon.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Addon.ViewModels
{
    public class MasterDetailViewModel : Observable
    {
        public Session Session { get; }

        private Addon.Core.Models.Addon _selected;

        public Addon.Core.Models.Addon Selected
        {
            get { return _selected; }
            set
            {
                Set(ref _selected, value);
            }
        }

        // public ObservableCollection<Addon.Core.Models.Addon> SampleItems { get; private set; } = new ObservableCollection<Addon.Core.Models.Addon>();

        public MasterDetailViewModel()
        {
            Session = Singleton<Session>.Instance;
        }

        public async Task LoadDataAsync(MasterDetailsViewState viewState)
        {


            if (viewState == MasterDetailsViewState.Both && Session.SelectedGame != null && Session.SelectedGame.Addons.Count > 0)
            {
                Selected = Session.SelectedGame.Addons.First();
            }
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
