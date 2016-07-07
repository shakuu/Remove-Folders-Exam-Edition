namespace RemoveFiles.Contracts
{
    public interface IFolderZipper
    {
        bool CompressFolder(string folderToCompress, string archiveLocation);
        
        bool DeleteTempFolder(string tempDirectory);

        bool ExtractToTempFolder(string tempDirectory, string archiveLocation);
    }
}