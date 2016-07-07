namespace StartUp
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Controls;
    using WinForms = System.Windows.Forms;

    using RemoveFiles;
    using RemoveFiles.Contracts;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string InitialDefaultPath = "D:\\GitHub";

        private IFolderZipper zipper;
        private IFolderRemover remover;
        private IFileRemover fileRemover;
        private IFolderPath folderPath;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.DefaultPath = InitialDefaultPath;
            DirNameTextBox.Text = this.DefaultPath;

            this.folderPath = new FolderPath(DirNameTextBox.Text);
            this.zipper = new FolderZipper();
            this.remover = new FolderRemover();
            this.fileRemover = new FileRemover();
        }

        public string DefaultPath { get; private set; }

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
            this.DisplayDeletedFolders.Text = string.Empty;

            try
            {
                this.folderPath = new FolderPath(DirNameTextBox.Text);
                this.DefaultPath = folderPath.Directory;
            }
            catch (Exception caught)
            {
                DisplayDeletedFolders.Text = caught.Message;
                return;
            }

            var isSuccessful = zipper.CompressFolder(folderPath.Directory, folderPath.ArchiveDirectory);

            if (isSuccessful)
            {
                DisplayDeletedFolders.Text += Environment.NewLine +
                    string.Format("Successfully archived{1}Output file: {0}",
                        folderPath.ArchiveDirectory,
                        Environment.NewLine);
            }

            // Extract to temp
            this.zipper.ExtractToTempFolder(folderPath.TempDirectory, folderPath.ArchiveDirectory);

            // Search in temp
            this.Search();

            // Delete
            this.Delete();

            // Delete files .suo .txt etc
            var filesToRemove = new ExtensionsToRemoveListBuilder();

            var removedFiles = this.fileRemover.RemoveFilesWithExtension(
                 folderPath.TempDirectory,
                 filesToRemove.ListOfExtensions);

            this.DisplayOnTextBlock(DisplayDeletedFolders, removedFiles,
                 "Operation: Delete unnecessary files - complete");

            // Archive
            if (this.zipper.CompressFolder(folderPath.TempDirectory, folderPath.ArchiveDirectory))
            {
                DisplayDeletedFolders.Text += Environment.NewLine +
                   string.Format("Successfully archived{1}Output file: {0}",
                       folderPath.ArchiveDirectory,
                       Environment.NewLine);
            }

            // Remove temp
            this.zipper.DeleteTempFolder(folderPath.TempDirectory);
        }

        private void DisplayOnTextBlock(TextBlock textBlock, IEnumerable<string> linesOfText, string successMessage = null)
        {
            foreach (var line in linesOfText)
            {
                textBlock.Text += Environment.NewLine
                    + line;
            }

            if (!string.IsNullOrEmpty(successMessage))
            {
                textBlock.Text += Environment.NewLine
                    + successMessage;
            }
        }

        private void Search()
        {
            this.remover.FindFolders(folderPath);

            if (this.remover.DirectoriesFound.Count > 0)
            {
                DisplayDeletedFolders.Text += Environment.NewLine +
                    string.Join(Environment.NewLine, this.remover.DirectoriesFound);
            }
            else
            {
                DisplayDeletedFolders.Text += Environment.NewLine +
                    "No Obj or Bin folders found.";
            }
        }

        private void Delete()
        {
            var userInput = WinForms.MessageBox
                .Show(string.Format("Are you sure sure you want to delete all /obj and /bin folders in {0}?",
                folderPath.Directory),
                "Confirm", MessageBoxButtons.OKCancel);

            if (userInput == WinForms.DialogResult.OK)
            {
                ICollection<string> result = new Collection<string>();

                try
                {
                    result = this.remover.RemoveFolders(remover.DirectoriesFound);
                }
                catch (Exception)
                {
                    DisplayDeletedFolders.Text += Environment.NewLine +
                        "Unable to delete all files/";
                }


                if (result.Count == 0)
                {
                    DisplayDeletedFolders.Text += Environment.NewLine +
                        "Operation: Delete Obj/ Bin folders - complete.";
                }
                else
                {
                    DisplayDeletedFolders.Text += Environment.NewLine +
                        "Unable to delete: " + Environment.NewLine +
                        string.Join(Environment.NewLine, result);
                }
            }
            else
            {
                DirNameTextBox.Text += Environment.NewLine +
                    "Operation canceled.";
            }
        }

        private void DirNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
