namespace RemoveFolders.Utilities.Contracts
{
    public interface IFolderZipper
    {
        bool CompressFolder(string folderToCompress, string archiveLocation);
        
        bool DeleteTempFolder(string tempDirectory);

        bool ExtractToFolder(string tempDirectory, string archiveLocation);
    }
}