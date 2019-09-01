using System.Collections.Generic;

namespace AddonToolkit.Model
{
    public class CurseAddon
    {
        public string AddonURL { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long Downloads { get; set; }
        public long UpdatedEpoch { get; set; }
        public long CreatedEpoch { get; set; }

        public override string ToString()
        {
            return base.ToString() + " N:" + AddonURL + " T:" + Title + " D:" + Downloads + " U:" + UpdatedEpoch + " C:" + CreatedEpoch;
        }

        public AddonData toAddonData()
        {
            return new AddonData()
            {
                ProjectName = this.AddonURL,
                Title = this.Title,
                Description = this.Description,
                NrOfDownloads = this.Downloads,
                UpdatedEpoch = this.UpdatedEpoch,
                CreatedEpoch = this.CreatedEpoch,
                Downloads = new List<Download>()
            };
        }
    }
}