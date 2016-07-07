
namespace RemoveFiles.Contracts
{
    using System.Collections.Generic;

    public interface IFileRemover
    {
        IEnumerable<string> RemoveFilesWithExtension(string path, IEnumerable<string> extensions);
    }
}