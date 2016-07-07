
namespace RemoveFiles
{
    using System;
    using System.IO;
    using System.Linq;

    using Contracts;

    public class FolderPath : IFolderPath
    {
        private const string FileExtension = ".zip";
        private const string TempPathFolder = "Temp";

        private DirectoryInfo directory;

        public FolderPath(string directoryAsString)
        {
            if (!string.IsNullOrEmpty(directoryAsString))
            {
                this.Directory = directoryAsString;
            }
        }

        public FolderPath(DirectoryInfo directory)
        {
            this.Directory = directory.FullName;
        }

        public string Directory
        {
            get
            {
                return this.directory.FullName;
            }

            private set
            {
                var inputDirectory = new DirectoryInfo(value);

                if (!inputDirectory.Exists)
                {
                    throw new Exception("Unable to find folder.");
                }
                else if (inputDirectory.FullName.Where(chr => chr == '\\').Count() == 1
                         && inputDirectory.FullName.Last() == '\\')
                {
                    throw new Exception("Drive root is not a valid folder input.");
                }
                else
                {
                    this.directory = inputDirectory;
                }
            }
        }

        public string TempDirectory
        {
            get
            {
                var pathAsArray = this.Directory.Split('\\');
                pathAsArray[pathAsArray.Length - 1] = FolderPath.TempPathFolder;

                return string.Join("\\", pathAsArray);
            }
        }

        public string ArchiveDirectory
        {
            get
            {
                return this.Directory + FolderPath.FileExtension;
            }
        }
    }
}
