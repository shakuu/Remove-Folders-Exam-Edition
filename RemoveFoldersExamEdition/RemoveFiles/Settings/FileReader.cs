namespace RemoveFolders.Utilities.Settings
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class FileReader : IFileReader
    {
        public IEnumerable<string> ReadFileContents(string fileNamePath)
        {
            var output = new LinkedList<string>();

            using (var reader = new StreamReader(fileNamePath))
            {
                while (!reader.EndOfStream)
                {
                    output.AddLast(reader.ReadLine());
                }
            }

            return output;
        }

        private void ValidateFileNamePath(string fileNamePath)
        {
            if (string.IsNullOrEmpty(fileNamePath))
            {
                throw new ArgumentNullException("File name is null or empty");
            }

            if (!File.Exists(fileNamePath))
            {
                throw new FileNotFoundException($"{fileNamePath}");
            }
        }
    }
}
