using Addon.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
