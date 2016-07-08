namespace RemoveFolders.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Forms;
    using WinControls = System.Windows.Controls;
    using WinForms = System.Windows.Forms;

    using Utilities;
    using Utilities.Contracts;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string InitialDefaultPath = "D:\\GitHub";

        private const string FileExtentionsListFilePath = "./settings/file.settings.removefolders";
        private const string FolderExtentionsListFilePath = "./settings/folder.settings.removefolders";

        private IFolderZipper zipper;
        private IRemover folderRemover;
        private IRemover fileRemover;
        private IFolderPath folderPath;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.DefaultPath = InitialDefaultPath;
            DirNameTextBox.Text = this.DefaultPath;

            this.folderPath = new FolderPath(DirNameTextBox.Text);
            this.zipper = new FolderZipper();
            this.folderRemover = new FolderRemover();
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
                this.folderPath = GetCurrentFolderPath(this.DirNameTextBox);
            }
            catch (Exception)
            {
                return;
            }

            var isSuccessful = this.zipper.CompressFolder(folderPath.Directory, folderPath.ArchiveDirectory);
            DisplayZipperCompressOutcomeMessage(isSuccessful);

            // Extract to temp
            this.zipper.ExtractToTempFolder(folderPath.TempDirectory, folderPath.ArchiveDirectory);

            // Search and Destray Folders.
            var foldersToRemove = new ToRemoveListProvider(MainWindow.FolderExtentionsListFilePath);
            this.Search(this.folderRemover.FindItems, folderPath.TempDirectory, foldersToRemove.ListToRemove);
            this.Delete(this.folderRemover.RemoveItems, this.folderRemover.ItemsFound);

            // Search and Destroy Files.
            var filesToRemove = new ToRemoveListProvider(MainWindow.FileExtentionsListFilePath);
            Search(this.fileRemover.FindItems, folderPath.TempDirectory, filesToRemove.ListToRemove);
            Delete(this.fileRemover.RemoveItems, this.fileRemover.ItemsFound);

            // Archive
            isSuccessful = this.zipper.CompressFolder(folderPath.TempDirectory, folderPath.ArchiveDirectory);
            DisplayZipperCompressOutcomeMessage(isSuccessful);

            // Remove temp
            this.zipper.DeleteTempFolder(folderPath.TempDirectory);
        }

        private IFolderPath GetCurrentFolderPath(WinControls.TextBox inputTextBlock)
        {
            IFolderPath result;

            try
            {
                result = new FolderPath(inputTextBlock.Text);
                this.DefaultPath = folderPath.Directory;
                return result;
            }
            catch (Exception caught)
            {
                inputTextBlock.Text = caught.Message;
                throw;
            }
        }

        private void DisplayZipperCompressOutcomeMessage(bool isSuccessful)
        {
            if (isSuccessful)
            {
                DisplayOnTextBlock(
                    this.DisplayDeletedFolders,
                    new Collection<string>() {
                        string.Format("Successfully archived{1}Output file: {0}",
                        folderPath.ArchiveDirectory,
                        Environment.NewLine)});
            }
        }

        private void DisplayOnTextBlock(WinControls.TextBlock textBlock, IEnumerable<string> linesOfText, string successMessage = null)
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

        private void Search(Func<string, ICollection<string>, ICollection<string>> searchMethod, string path, ICollection<string> searchParams)
        {
            var searchResult = searchMethod.Invoke(path, searchParams);

            if (searchResult.Count > 0)
            {
                DisplayDeletedFolders.Text += Environment.NewLine +
                    string.Join(Environment.NewLine, searchResult);
            }
        }

        private void Delete(Func<IEnumerable<string>, ICollection<string>> deleteMethod, IEnumerable<string> inputParams)
        {
            var userInput = WinForms.MessageBox
                .Show(string.Format("Are you sure sure you want to delete all unnecessary files and folders in {0}?",
                folderPath.Directory),
                "Confirm deletion", MessageBoxButtons.OKCancel);

            if (userInput == WinForms.DialogResult.OK)
            {
                ICollection<string> result = new Collection<string>();

                try
                {
                    result = deleteMethod.Invoke(inputParams);
                }
                catch (Exception)
                {
                    DisplayDeletedFolders.Text += Environment.NewLine +
                        "Unable to delete all files/ folders";
                }


                if (result.Count == 0)
                {
                    DisplayDeletedFolders.Text += Environment.NewLine +
                        "Operation: Delete - complete.";
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

        private void DirNameTextBox_TextChanged(object sender, WinControls.TextChangedEventArgs e)
        {
        }
    }
}
