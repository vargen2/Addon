using Addon.Core.Helpers;
using Addon.Core.Models;
using Addon.Helpers;

namespace Addon.ViewModels
{
    public class MainViewModel : Observable
    {
        public Session Session { get; }

        public MainViewModel()
        {
            Session = Singleton<Session>.Instance;
        }
    }
}
