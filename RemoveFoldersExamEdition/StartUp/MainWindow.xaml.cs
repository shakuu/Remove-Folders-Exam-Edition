namespace RemoveFolders.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Controls;
    using WinForms = System.Windows.Forms;

    using Utilities;
    using Utilities.Contracts;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string InitialDefaultPath = "D:\\GitHub";

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
            this.Search(this.folderRemover.FindItems, folderPath.TempDirectory, new List<string>());

            // Delete
            this.Delete(this.folderRemover.RemoveItems, this.folderRemover.ItemsFound);

            // Delete files .suo .txt etc
            var filesToRemove = new ToRemoveListProvider();

            Search(this.fileRemover.FindItems, folderPath.TempDirectory, filesToRemove.ListToRemove);
            Delete(this.fileRemover.RemoveItems, this.fileRemover.ItemsFound);
            
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

        private void Search(Func<string, ICollection<string>, ICollection<string>> searchMethod, string path, ICollection<string> searchParams)
        {
            var searchResult = searchMethod.Invoke(path, searchParams);

            if (searchResult.Count > 0)
            {
                DisplayDeletedFolders.Text += Environment.NewLine +
                    string.Join(Environment.NewLine, searchResult);
            }
            else
            {
                DisplayDeletedFolders.Text += Environment.NewLine +
                    "No Obj or Bin folders found.";
            }
        }

        private void Delete(Func<IEnumerable<string>, ICollection<string>> deleteMethod, IEnumerable<string> inputParams)
        {
            var userInput = WinForms.MessageBox
                .Show(string.Format("Are you sure sure you want to delete all unnecessary folders and files in {0}?",
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
