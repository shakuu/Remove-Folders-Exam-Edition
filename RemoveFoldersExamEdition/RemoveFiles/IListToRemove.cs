using System.Collections.Generic;

namespace RemoveFiles
{
    public interface IListToRemove
    {
        ICollection<string> ListOfExtensions { get; }
    }
}