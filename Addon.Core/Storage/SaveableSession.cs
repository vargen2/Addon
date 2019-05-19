using System.Collections.Generic;

namespace Addon.Core.Storage
{
    public class SaveableSession
    {
        public SaveableGame SelectedGame { get; set; }
        public List<SaveableGame> Games { get; set; }

        public override string ToString()
        {
            return $"{nameof(SelectedGame)}: {SelectedGame}, {nameof(Games)}: {Games}";
        }
    }
}
