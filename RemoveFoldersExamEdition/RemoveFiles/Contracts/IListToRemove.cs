namespace RemoveFolders.Utilities.Contracts
{
    using System.Collections.Generic;

    public interface IListToRemove
    {
        ICollection<string> ListToRemove { get; }
    }
}