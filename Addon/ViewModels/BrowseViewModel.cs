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
