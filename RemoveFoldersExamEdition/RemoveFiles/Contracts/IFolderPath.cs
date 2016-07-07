namespace RemoveFiles.Contracts
{
    public interface IFolderPath
    {
        string Directory { get; }


        string TempDirectory { get; }
        string ArchiveDirectory { get; }
    }
}