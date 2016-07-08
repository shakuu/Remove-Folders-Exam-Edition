namespace RemoveFolders.Utilities
{
    using System;
    using System.IO;
    using System.IO.Compression;

    using Contracts;
    
    public class FolderZipper : IFolderZipper
    {
        public bool CompressFolder(string folderToCompress, string archiveLocation)
        {
            this.RemoveExistingArchive(archiveLocation);
            
            try
            {
                ZipFile.CreateFromDirectory(
                    folderToCompress, 
                    archiveLocation);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public bool ExtractToTempFolder(string tempDirectory, string archiveLocation)
        {
            if (archiveLocation == null || !File.Exists(archiveLocation))
            {
                throw new FileNotFoundException($"{archiveLocation} not found");
            }

            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }

            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }

            ZipFile.ExtractToDirectory(archiveLocation, tempDirectory);

            return true;
        }

        public bool DeleteTempFolder(string tempDirectory)
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
                return true;
            }

            return false;
        }

        private void RemoveExistingArchive(string archiveLocation)
        {
            if (File.Exists(archiveLocation))
            {
                File.Delete(archiveLocation);
            }
        }
    }
}
