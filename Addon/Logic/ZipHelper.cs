using Addon.Helpers;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Addon.Logic
{


    /// <summary> 
    /// https://code.msdn.microsoft.com/How-to-and-extract-zip-242da300/sourcecode?fileId=167934&pathId=1626668963
    /// </summary> 
    internal class ZipHelper : Observable
    {
        private float counter;

        private int nextNotify = 0;

        public float Entries { get; set; }
        public Core.Models.Addon Addon { get; set; }


        private int progress = 0;

        public int Progress
        {
            get { return progress; }
            set
            {
                Set(ref progress, value);

            }

        }



        /// <summary> 
        /// https://code.msdn.microsoft.com/How-to-and-extract-zip-242da300/sourcecode?fileId=167934&pathId=1626668963
        /// 
        /// Unzips the specified zip file to the specified destination folder. 
        /// </summary> 
        /// <param name="zipFile">The zip file</param> 
        /// <param name="destinationFolder">The destination folder</param> 
        /// <returns></returns> 
        public IAsyncAction UnZipFileAsync(StorageFile zipFile, StorageFolder destinationFolder)
        {
            return UnZipFileHelper(zipFile, destinationFolder).AsAsyncAction();
        }



        private async Task UnZipFileHelper(StorageFile zipFile, StorageFolder destinationFolder)
        {
            if (zipFile == null || destinationFolder == null ||
                !Path.GetExtension(zipFile.Name).Equals(".zip", StringComparison.OrdinalIgnoreCase)
                )
            {
                throw new ArgumentException("Invalid argument...");
            }

            Stream zipMemoryStream = await zipFile.OpenStreamForReadAsync();

            // Create zip archive to access compressed files in memory stream 
            using (ZipArchive zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Read))
            {
                // Unzip compressed file iteratively. 
                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    await UnzipZipArchiveEntryAsync(entry, entry.FullName, destinationFolder);
                }
            }
        }

        /// <summary> 
        /// It checks if the specified path contains directory. 
        /// </summary> 
        /// <param name="entryPath">The specified path</param> 
        /// <returns></returns> 
        private static bool IfPathContainDirectory(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                return false;
            }

            return entryPath.Contains("/");
        }

        /// <summary> 
        /// It checks if the specified folder exists. 
        /// </summary> 
        /// <param name="storageFolder">The container folder</param> 
        /// <param name="subFolderName">The sub folder name</param> 
        /// <returns></returns> 
        private static async Task<bool> IfFolderExistsAsync(StorageFolder storageFolder, string subFolderName)
        {
            try
            {
                IStorageItem item = await storageFolder.GetItemAsync(subFolderName);
                return (item != null);
            }
            catch
            {
                // Should never get here 
                return false;
            }
        }

        /// <summary> 
        /// Unzips ZipArchiveEntry asynchronously. 
        /// </summary> 
        /// <param name="entry">The entry which needs to be unzipped</param> 
        /// <param name="filePath">The entry's full name</param> 
        /// <param name="unzipFolder">The unzip folder</param> 
        /// <returns></returns> 
        private async Task UnzipZipArchiveEntryAsync(ZipArchiveEntry entry, string filePath, StorageFolder unzipFolder)
        {
            if (IfPathContainDirectory(filePath))
            {
                // Create sub folder 
                string subFolderName = Path.GetDirectoryName(filePath);

                bool isSubFolderExist = await IfFolderExistsAsync(unzipFolder, subFolderName);

                StorageFolder subFolder;

                if (!isSubFolderExist)
                {
                    // Create the sub folder. 
                    subFolder =
                        await unzipFolder.CreateFolderAsync(subFolderName, CreationCollisionOption.ReplaceExisting);
                }
                else
                {
                    // Just get the folder. 
                    subFolder =
                        await unzipFolder.GetFolderAsync(subFolderName);
                }

                // All sub folders have been created yet. Just pass the file name to the Unzip function. 
                string newFilePath = Path.GetFileName(filePath);

                if (!string.IsNullOrEmpty(newFilePath))
                {
                    // Unzip file iteratively. 
                    await UnzipZipArchiveEntryAsync(entry, newFilePath, subFolder);
                }
            }
            else
            {

                // Read uncompressed contents 
                using (Stream entryStream = entry.Open())
                {
                    byte[] buffer = new byte[entry.Length];
                    entryStream.Read(buffer, 0, buffer.Length);

                    // Create a file to store the contents 
                    StorageFile uncompressedFile = await unzipFolder.CreateFileAsync
                    (entry.Name, CreationCollisionOption.ReplaceExisting);

                    // Store the contents 
                    using (IRandomAccessStream uncompressedFileStream =
                    await uncompressedFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        using (Stream outstream = uncompressedFileStream.AsStreamForWrite())
                        {
                            outstream.Write(buffer, 0, buffer.Length);
                            outstream.Flush();
                        }
                    }

                }
                Increment();
            }

        }

        private void Increment()
        {
            counter++;
            var newProggress = (int)((counter / Entries) * 100f);
            //Debug.WriteLine("counter/entries " + counter + " / " + Entries + " ........newprog " + newProggress);
            if (newProggress > nextNotify)
            {
                Progress = newProggress;
                nextNotify = nextNotify + 5;
            }
        }

    }
}
