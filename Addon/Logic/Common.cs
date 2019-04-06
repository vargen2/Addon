using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Addon.Core.Models;

namespace Addon.Logic
{
   public static class Common
    {
        public static Game FolderToGame(StorageFolder folder)
        {
            return new Game(folder.Path);
        }
    }
}
