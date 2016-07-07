
namespace RemoveFiles
{
    using System.Collections.Generic;

    public class FilesToRemoveListProvider : IListToRemove
    {
        private HashSet<string> extensions;

        public FilesToRemoveListProvider()
        {
            this.BuildDefaultList();
        }

        public ICollection<string> ListOfExtensions
        {
            get
            {
                return new HashSet<string>(this.extensions);
            }
        }

        private void BuildDefaultList()
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
