namespace RemoveFolders.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Contracts;

    public class FolderRemover : IRemover
    {
        private const string BinDirectory = "bin";
        private const string ObjDirectory = "obj";
        private const string DotVSDirectory = ".vs";

        private ICollection<string> dirsFound = new List<string>();
        private ICollection<string> listOfFolderNamesToMatch;

        public ICollection<string> ItemsFound
        {
            get
            {
                return new List<string>(this.dirsFound);
            }
        }

        public ICollection<string> FindItems(string path, ICollection<string> searchForItemsContaining)
        {
            this.listOfFolderNamesToMatch = searchForItemsContaining;

            this.ClearList();

            var output = this.FindObjBinDirectories(path);

            this.listOfFolderNamesToMatch = null;

            return output;
        }

        private ICollection<string> FindObjBinDirectories(string path)
        {
            var dirs = Directory.GetDirectories(path);

            if (dirs == null)
            {
                return dirsFound;
            }

            foreach (var dir in dirs)
            {
                if (CheckDir(dir))
                {
                    dirsFound.Add(dir);
                }
                else
                {
                    FindObjBinDirectories(dir);
                }
            }

            return dirsFound;
        }

        public ICollection<string> RemoveItems(IEnumerable<string> itemsToRemove)
        {
            var foldersNotFound = new List<string>();

            foreach (var folder in itemsToRemove)
            {
                if (Directory.Exists(folder))
                {
                    try
                    {
                        Directory.Delete(folder, true);
                    }
                    catch (Exception)
                    {

                        foldersNotFound.Add(folder);
                    }
                }
                else
                {
                    foldersNotFound.Add(folder);
                }
            }

            return foldersNotFound;
        }

        private bool CheckDir(string path)
        {
            var folder = path
                .Split(
                    new[] { '\\' },
                    StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault();

            var result = 
                this.listOfFolderNamesToMatch.Contains(folder);

            return result;
        }

        private bool ClearList()
        {
            try
            {
                dirsFound.Clear();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
