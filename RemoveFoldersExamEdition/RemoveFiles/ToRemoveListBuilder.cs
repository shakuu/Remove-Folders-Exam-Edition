namespace RemoveFolders.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;
    using Settings;

    public class ToRemoveListProvider : IListToRemove
    {
        private HashSet<string> items = new HashSet<string>();

        private IFileReader fileReader;

        public ToRemoveListProvider()
        {
            this.BuildDefaultExtensionsList();
        }

        public ToRemoveListProvider(string fileNamePath)
        {
            this.BuildFromFile(fileNamePath);
        }

        public ICollection<string> ListToRemove
        {
            get
            {
                return new HashSet<string>(this.items);
            }
        }

        private void BuildDefaultExtensionsList()
        {
            this.items = new HashSet<string>()
            {
                "md",
                "txt",
                "suo",
                "zip"
            };
        }

        private void BuildFromFile(string fileNamePath)
        {
            IEnumerable<string> dataFromFile = null;

            try
            {
                dataFromFile = ReadDataFromFile(fileNamePath);
                ValidateDataFromFile(dataFromFile);
            }
            catch (Exception)
            {
                BuildDefaultExtensionsList();
            }
            
            foreach (var line in dataFromFile)
            {
                this.items.Add(line);
            }
        }

        private void ValidateDataFromFile(IEnumerable<string> dataFromFile)
        {
            if (dataFromFile == null)
            {
                throw new ArgumentNullException("No input data, collection is null");
            }

            foreach (var line in dataFromFile)
            {
                if (line.Any(chr => !char.IsLetter(chr) && chr != '.'))
                {
                    throw new ArgumentException("Allowed symbols are : letters and . (dot)");
                }
            }
        }

        private IEnumerable<string> ReadDataFromFile(string fileNamePath)
        {
            this.fileReader = new FileReader();

            var dataFromFile = this.fileReader.ReadFileContents(fileNamePath);

            this.fileReader = null;

            return dataFromFile;
        }
    }
}
