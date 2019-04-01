using System;
using System.Collections.Generic;
using System.Text;

namespace Addon.Core.Models
{
    public class CurseAddon
    {
        public string addonURL { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public long downloads { get; set; }
        public string updatedEpoch { get; set; }
        public string createdEpoch { get; set; }


    }
}
