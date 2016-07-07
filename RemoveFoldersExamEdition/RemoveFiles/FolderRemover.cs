
namespace RemoveFiles
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Contracts;

    public class FolderRemover : IFolderRemover
    {
        private const string BinDirectory = "bin";
        private const string ObjDirectory = "obj";
        private const string DotVSDirectory = ".vs";

        private ICollection<string> dirsFound = new List<string>();

        public ICollection<string> DirectoriesFound
        {
            get
            {
                return new List<string>(this.dirsFound);
            }
        }

        public ICollection<string> FindFolders(IFolderPath folderPath)
        {
            this.ClearList();

            var output = this.FindObjBinDirectories(folderPath.TempDirectory);

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

        public ICollection<string> RemoveFolders(IEnumerable<string> foldersToRemove)
        {
            var foldersNotFound = new List<string>();

            foreach (var folder in foldersToRemove)
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

            return (folders == FolderRemover.BinDirectory 
                || folders == FolderRemover.ObjDirectory
                || folders == FolderRemover.DotVSDirectory);
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
