namespace RemoveFolders.Utilities.Contracts
{
    using System.Collections.Generic;

    public interface IFileRemover
    {
        IEnumerable<string> RemoveFilesWithExtension(string path, ICollection<string> extensions);
    }
}