using System;
using System.Collections.Generic;

namespace Addon.Core.Storage
{
    public class AddonData
    {
        public string FolderName { get; set; }
        public HashSet<string> SubFolders { get; set; }
        public long Size { get; set; }
        public int Files { get; set; }

        public string ProjectName { get; set; }
        public string ProjectUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long NrOfDownloads { get; set; }
        public long UpdatedEpoch { get; set; }
        public long CreatedEpoch { get; set; }




        //public string FolderName { get; }
        //public string ProjectUrl { get; }
        //public HashSet<string> SubFolders { get; set; }

        //public AddonData(string folderName, string projectUrl)
        //{            
        //    FolderName = folderName ?? throw new NullReferenceException();
        //    ProjectUrl = projectUrl ?? throw new NullReferenceException();
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj == null || GetType() != obj.GetType())
        //        return false;
        //    return (obj is AddonData) && ((AddonData)obj).FolderName == this.FolderName;
        //}

        //public override int GetHashCode()
        //{
        //    return FolderName.GetHashCode();
        //}

    }
}
