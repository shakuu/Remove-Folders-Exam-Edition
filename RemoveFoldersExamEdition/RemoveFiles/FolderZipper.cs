
namespace RemoveFiles
{
    using System;
    using System.IO;
    using System.IO.Compression;

    using RemoveFiles.Contracts;
    
    public class FolderZipper : IFolderZipper
    {
        public bool CompressFolder(IFolderPath folderPath)
        {
            this.RemoveExistingArchive(folderPath);
            
            try
            {
                ZipFile.CreateFromDirectory(
                    folderPath.Directory, 
                    folderPath.ArchiveDirectory);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CompressTempFolder(IFolderPath folderPath)
        {
            this.RemoveExistingArchive(folderPath);

            try
            {
                ZipFile.CreateFromDirectory(
                    folderPath.TempDirectory,
                    folderPath.ArchiveDirectory);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ExtractToTempFolder(IFolderPath folderPath)
        {
            if (folderPath.ArchiveDirectory == null || !File.Exists(folderPath.ArchiveDirectory))
            {
                throw new FileNotFoundException($"{folderPath.ArchiveDirectory} not found");
            }

            if (Directory.Exists(folderPath.TempDirectory))
            {
                Directory.Delete(folderPath.TempDirectory, true);
            }

            if (!Directory.Exists(folderPath.TempDirectory))
            {
                Directory.CreateDirectory(folderPath.TempDirectory);
            }

            ZipFile.ExtractToDirectory(folderPath.ArchiveDirectory, folderPath.TempDirectory);

            return true;
        }

        public bool DeleteTempFolder(IFolderPath folderPath)
        {
            if (Directory.Exists(folderPath.TempDirectory))
            {
                Directory.Delete(folderPath.TempDirectory, true);
                return true;
            }

            return false;
        }

        private void RemoveExistingArchive(IFolderPath folderPath)
        {
            if (File.Exists(folderPath.ArchiveDirectory))
            {
                File.Delete(folderPath.ArchiveDirectory);
            }
        }
    }
}
