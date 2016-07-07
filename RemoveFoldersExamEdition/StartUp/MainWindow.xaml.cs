namespace StartUp
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Forms;
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

            var isSuccessful = zipper.CompressFolder(folderPath);

            if (isSuccessful)
            {
                DisplayDeletedFolders.Text += Environment.NewLine +
                    string.Format("Successfully archived{1}Output file: {0}",
                        folderPath.ArchiveDirectory,
                        Environment.NewLine);
            }

            // Extract to temp
            this.zipper.ExtractToTempFolder(folderPath);

            // Search in temp
            this.Search();

            // Delete
            this.Delete();

            // Archive
            if (this.zipper.CompressTempFolder(folderPath))
            {
                DisplayDeletedFolders.Text += Environment.NewLine +
                   string.Format("Successfully archived{1}Output file: {0}",
                       folderPath.ArchiveDirectory,
                       Environment.NewLine);
            }

            // Remove temp
            this.zipper.DeleteTempFolder(folderPath);
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
                        "Operation Complete.";
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

        private void DirNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }
    }
}
