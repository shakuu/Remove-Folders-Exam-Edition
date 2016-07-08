namespace RemoveFolders.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Contracts;

    public class FileRemover : IRemover
    {
        private ICollection<string> filesFound = new LinkedList<string>();

        public ICollection<string> ItemsFound
        {
            get
            {
                return new List<string>(this.filesFound);
            }
        }

        public ICollection<string> FindItems(string path, ICollection<string> searchForItemsContaining)
        {
            this.CheckIfPathIsValid(path);

            var filesInCurrentDirectory = Directory.GetFiles(path);

            foreach (var file in filesInCurrentDirectory)
            {
                var filenameExtension = file
                    .Split('\\').LastOrDefault()
                    .Split('.').LastOrDefault();

                if (filenameExtension != null)
                {
                    if (searchForItemsContaining.Contains(filenameExtension))
                    {
                        this.filesFound.Add(file);
                        //File.Delete(file);
                    }
                }
            }

            this.filesFound.Add($"Number of files found: {this.filesFound.Count}");

            return this.ItemsFound;
        }

        public ICollection<string> RemoveItems(IEnumerable<string> itemsToRemove)
        {
            var filesNotFound = new List<string>();

            foreach (var file in itemsToRemove)
            {
                if (File.Exists(file))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception)
                    {

                        filesNotFound.Add(file);
                    }
                }
                else
                {
                    filesNotFound.Add(file);
                }
            }

            return filesNotFound;
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
