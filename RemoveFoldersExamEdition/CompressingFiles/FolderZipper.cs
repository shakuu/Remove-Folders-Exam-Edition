
//namespace CompressingFiles
//{
//    using System;
//    using System.IO;
//    using System.IO.Compression;

//    public class FolderZipper : IFolderZipper
//    {
//        private const string FileExtension = ".zip";
//        private const string TempPathFolder = "Temp";
        
//        private string path;

//        public FolderZipper(string path)
//        {
//            this.PathToCompress = path;
//        }

//        public string PathToCompress
//        {
//            get
//            {
//                return this.path;
//            }

//            set
//            {
//                // TODO: Validate
//                this.path = value;
//            }
//        }

//        public string ZippedPath
//        {
//            get
//            {
//                return this.PathToCompress + FolderZipper.FileExtension;
//            }
//        }

//        public string TempPath
//        {
//            get
//            {
//                var pathAsArray = PathToCompress.Split('\\');
//                pathAsArray[pathAsArray.Length - 1] = FolderZipper.TempPathFolder;


//                return string.Join("\\", pathAsArray);
//            }
//        }

//        public bool CompressFolder()
//        {
//            if (File.Exists(this.ZippedPath))
//            {
//                File.Delete(this.ZippedPath);
//            }

//            try
//            {
//                ZipFile.CreateFromDirectory(this.PathToCompress, this.ZippedPath);
//                return true;
//            }
//            catch (Exception)
//            {
//                return false;
//            }
//        }

//        public bool CompressTempFolder()
//        {
//            if (File.Exists(this.ZippedPath))
//            {
//                File.Delete(this.ZippedPath);
//            }

//            try
//            {
//                ZipFile.CreateFromDirectory(this.TempPath, this.ZippedPath);
//                return true;
//            }
//            catch (Exception)
//            {
//                return false;
//            }
//        }

//        public bool ExtractToTempFolder()
//        {
//            if (this.ZippedPath == null || !File.Exists(this.ZippedPath))
//            {
//                throw new FileNotFoundException($"{this.ZippedPath} not found");
//            }

//            if (Directory.Exists(this.TempPath))
//            {
//                Directory.Delete(this.TempPath, true);
//            }

//            if (!Directory.Exists(this.TempPath))
//            {
//                Directory.CreateDirectory(this.TempPath);
//            }

//            ZipFile.ExtractToDirectory(this.ZippedPath, this.TempPath);

//            return true;
//        }

//        public bool DeleteTempFolder()
//        {
//            if (Directory.Exists(this.TempPath))
//            {
//                Directory.Delete(this.TempPath, true);
//                return true;
//            }

//            return false;
//        }
//    }
//}
