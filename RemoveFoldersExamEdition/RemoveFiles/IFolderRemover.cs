
namespace RemoveFiles
{
    using System.Collections.Generic;

    public interface IFolderRemover
    {
        string Path { get; set; }

        ICollection<string> DirectoriesFound { get; }

        ICollection<string> RemoveFolders();
    }
}