using System;
using System.Collections.Generic;

namespace Addon.Core.Storage
{
    public class AddonData
    {
        public string FolderName { get; }
        public string ProjectUrl { get; }
        public HashSet<string> SubFolders { get; set; }

        public AddonData(string folderName, string projectUrl)
        {            
            FolderName = folderName ?? throw new NullReferenceException();
            ProjectUrl = projectUrl ?? throw new NullReferenceException();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            return (obj is AddonData) && ((AddonData)obj).FolderName == this.FolderName;
        }

        public override int GetHashCode()
        {
            return FolderName.GetHashCode();
        }

    }
}
