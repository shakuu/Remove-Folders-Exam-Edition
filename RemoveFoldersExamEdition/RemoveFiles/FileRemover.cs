
namespace RemoveFiles
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Contracts;

    public class FileRemover : IFileRemover
    {
        public IEnumerable<string> RemoveFilesWithExtension(string path, IEnumerable<string> extensions)
        {
            var listOfDeletedFiles = new LinkedList<string>();

            var filesInCurrentDirectory = Directory.GetFiles(path);

            foreach (var file in filesInCurrentDirectory)
            {
                var filenameAsArray = file.Split('\\').Last().Split('.').Last();

                if (extensions.Contains(filenameAsArray))
                {
                    listOfDeletedFiles.AddLast(file);
                    File.Delete(file);
                }
            }

            return listOfDeletedFiles;
        }
    }
}
