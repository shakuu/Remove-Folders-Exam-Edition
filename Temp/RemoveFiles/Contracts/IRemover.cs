namespace RemoveFolders.Utilities.Contracts
{
    using System.Collections.Generic;

    public interface IRemover
    {
        ICollection<string> ItemsFound { get; }

        ICollection<string> FindItems(string path, ICollection<string> searchForItemsContaining);

        ICollection<string> RemoveItems(IEnumerable<string> itemsToRemove);
    }
}