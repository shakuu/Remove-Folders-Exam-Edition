
namespace RemoveFiles
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class FolderRemover : IFolderRemover
    {
        private const string BinDirectory = "bin";
        private const string ObjDirectory = "obj";

        private ICollection<string> dirsFound;

        private string path;

        public FolderRemover(string path)
        {
            this.dirsFound = new LinkedList<string>();

            this.Path = path;
        }
        
        public string Path
        {
            get
            {
                return this.path;
            }

            set
            {
                this.path = value;

                this.FindFolders();
            }
        }

        public ICollection<string> DirectoriesFound
        {
            get
            {
                return new List<string>(this.dirsFound);
            }
        }

        private ICollection<string> FindFolders()
        {
            this.ClearList();

            var output = this.FindObjBinDirectories(this.Path);

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

        public ICollection<string> RemoveFolders()
        {
            var foldersNotFound = new List<string>();

            foreach (var folder in this.DirectoriesFound)
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

        private static bool CheckDir(string path)
        {
            var folders = path
                .Split(
                    new[] { '\\' },
                    StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault();

            return (folders == FolderRemover.BinDirectory || folders == FolderRemover.ObjDirectory);
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
