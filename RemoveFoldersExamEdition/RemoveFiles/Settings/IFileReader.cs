namespace RemoveFolders.Utilities.Settings
{
    using System.Collections.Generic;

    public interface IFileReader
    {
        IEnumerable<string> ReadFileContents(string fileNamePath);
    }
}