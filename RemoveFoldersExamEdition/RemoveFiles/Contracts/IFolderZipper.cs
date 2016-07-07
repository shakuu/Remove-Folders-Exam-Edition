namespace RemoveFiles.Contracts
{
    public interface IFolderZipper
    {
        bool CompressFolder(IFolderPath folderPath);

        bool CompressTempFolder(IFolderPath folderPath);

        bool DeleteTempFolder(IFolderPath folderPath);

        bool ExtractToTempFolder(IFolderPath folderPath);
    }
}