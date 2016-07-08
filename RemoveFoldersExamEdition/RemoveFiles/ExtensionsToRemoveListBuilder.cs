namespace RemoveFolders.Utilities
{
    using System.Collections.Generic;
    
    using Contracts;

    public class ToRemoveListProvider : IListToRemove
    {
        private HashSet<string> extensions;

        public ToRemoveListProvider()
        {
            this.BuildDefaultExtensionsList();
        }

        public ICollection<string> ListToRemove
        {
            get
            {
                return new HashSet<string>(this.extensions);
            }
        }

        private void BuildDefaultExtensionsList()
        {
            this.extensions = new HashSet<string>()
            {
                "md",
                "txt",
                "suo",
                "zip"
            };
        }
    }
}
