namespace StartUp
{
    using CompressingFiles;
    using RemoveFiles;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms;
    using WinForms = System.Windows.Forms;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string InitialDefaultPath = "D:\\GitHub";

        private string pathToDeleteFrom;

        private IFolderZipper zipper;
        private IFolderRemover remover;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.DefaultPath = InitialDefaultPath;
            DirNameTextBox.Text = this.DefaultPath;

            this.zipper = new FolderZipper(DirNameTextBox.Text);
            this.remover = new FolderRemover(DirNameTextBox.Text);
        }

        public string DefaultPath { get; private set; }

        /// <summary>
        /// Contains Validation of input path
        /// </summary>
        public string PathToDeleteFrom
        {
            get
            {
                return this.pathToDeleteFrom;
            }

            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception("Invalid input (empty).");
                }
                else if (!Directory.Exists(value))
                {
                    throw new Exception("Unable to find folder.");
                }
                else if (value.Where(chr => chr == '\\').Count() == 1
                         && value.Last() == '\\')
                {
                    throw new Exception("Drive root is not a valid folder input.");
                }
                else
                {
                    this.pathToDeleteFrom = value;
                }
            }
        }

        private void FolderBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            dialog.SelectedPath = DefaultPath; // Read from file

            var userInput = dialog.ShowDialog();

            if (userInput != WinForms.DialogResult.Cancel)
            {
                DirNameTextBox.Text = dialog.SelectedPath;
            }
            else
            {
                return;
            }
        }

        private void ArchiveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.pathToDeleteFrom = DirNameTextBox.Text;
                this.zipper.PathToCompress = this.pathToDeleteFrom;
                this.DefaultPath = PathToDeleteFrom;
            }
            catch (Exception caught)
            {
                DisplayDeletedFolders.Text = caught.Message;
                return;
            }

            var isSuccessful = zipper.CompressFolder();

            if (isSuccessful)
            {
                DisplayDeletedFolders.Text = string.Format("Successfully archived{1}Output file: {0}",
                    /*DirNameTextBox.Text + ".zip"*/
                    zipper.ZippedPath,
                    Environment.NewLine);
            }

            // Extract to temp
            this.zipper.ExtractToTempFolder();

            // Search in temp
            this.Search();

            // Delete
            this.Delete();

            // Archive
            this.zipper.CompressTempFolder();
            // Remove temp
            this.zipper.DeleteTempFolder();
        }

        private void Search()
        {
            try
            {
                this.remover.Path = this.zipper.TempPath;
            }
            catch (Exception caught)
            {
                DisplayDeletedFolders.Text = caught.Message;
                return;
            }


            if (this.remover.DirectoriesFound.Count > 0)
            {
                DisplayDeletedFolders.Text =
                    string.Join(Environment.NewLine, this.remover.DirectoriesFound);
            }
            else
            {
                DisplayDeletedFolders.Text = "No Obj or Bin folders found.";
            }
        }

        private void Delete()
        {
            var userInput = WinForms.MessageBox
                .Show(string.Format("Are you sure sure you want to delete all /obj and /bin folders in {0}?",
                this.PathToDeleteFrom),
                "Confirm", MessageBoxButtons.OKCancel);

            if (userInput == WinForms.DialogResult.OK)
            {
                ICollection<string> result = new List<string>();

                try
                {
                    result = this.remover.RemoveFolders();
                }
                catch (Exception)
                {
                    DisplayDeletedFolders.Text = "Unable to delete all files/";
                }


                if (result.Count == 0)
                {
                    DisplayDeletedFolders.Text = "Operation Complete.";
                }
                else
                {
                    DisplayDeletedFolders.Text = "Unable to delete: " + Environment.NewLine +
                        string.Join(Environment.NewLine, result);
                }
            }
            else
            {
                DirNameTextBox.Text = "Operation canceled.";
            }
        }

        private void DirNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }
    }
}
