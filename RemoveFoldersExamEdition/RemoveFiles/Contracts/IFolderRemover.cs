namespace RemoveFolders.Utilities.Contracts
{
    using System.Collections.Generic;

    public interface IFolderRemover
    {
        ICollection<string> DirectoriesFound { get; }

        ICollection<string> FindFolders(IFolderPath folderPath);

        ICollection<string> RemoveFolders(IEnumerable<string> foldersToRemove);
    }
}