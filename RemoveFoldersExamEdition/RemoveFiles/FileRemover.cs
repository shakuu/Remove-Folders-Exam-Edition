namespace RemoveFolders.Utilities
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Contracts;

    public class FileRemover : IFileRemover
    {
        public IEnumerable<string> RemoveFilesWithExtension(string path, ICollection<string> extensions)
        {
            this.CheckIfPathIsValid(path);

            var listOfDeletedFiles = new LinkedList<string>();

            var filesInCurrentDirectory = Directory.GetFiles(path);

            foreach (var file in filesInCurrentDirectory)
            {
                var filenameExtension = file
                    .Split('\\').LastOrDefault()
                    .Split('.').LastOrDefault();

                if (filenameExtension != null)
                {
                    if (extensions.Contains(filenameExtension))
                    {
                        listOfDeletedFiles.AddLast(file);
                        File.Delete(file);
                    }
                }
            }

            return listOfDeletedFiles;
        }

        private void CheckIfPathIsValid(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"{path} not found");
            }
        }
    }
}
