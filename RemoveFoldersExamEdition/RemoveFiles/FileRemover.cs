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
            this.ClearList();

            searchForItemsContaining = 
                RemoveDotsFromListOfItemsToRemove(searchForItemsContaining);

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
            
            return this.ItemsFound;
        }

        private ICollection<string> RemoveDotsFromListOfItemsToRemove(IEnumerable<string> inputItems)
        {
            ICollection<string> output = new HashSet<string>();

            foreach (var item in inputItems)
            {
                output.Add(item.Replace(".", ""));
            }

            return output;
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

        private bool ClearList()
        {
            try
            {
                filesFound.Clear();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
