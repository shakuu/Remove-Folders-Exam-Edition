
namespace RemoveFiles
{
    using System.Collections.Generic;

    public class ExtensionsToRemoveListBuilder
    {
        private HashSet<string> extensions;

        public ExtensionsToRemoveListBuilder()
        {
            this.BuildDefaultList();
        }

        public IEnumerable<string> ListOfExtensions
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
