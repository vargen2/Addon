using System.Collections.Generic;

namespace AddonToolkit.Model
{
    public class AddonData
    {
        public string FolderName { get; set; }
        public HashSet<string> SubFolders { get; set; }
        public long Size { get; set; }
        public int Files { get; set; }
        public string ProjectName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long NrOfDownloads { get; set; }
        public long UpdatedEpoch { get; set; }
        public long CreatedEpoch { get; set; }
        public bool HasRetail { get; set; }
        public bool HasClassic { get; set; }
        public List<Download> Downloads { get; set; }
    }
}