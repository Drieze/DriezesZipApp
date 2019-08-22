using System.Windows;
using System.IO;
using System.Windows.Forms;
using System.IO.Compression;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string selectedPath = "";
        private delegate void ProgressBarDelegate();
        private delegate void LogListBoxDelegate(string source, string target);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnOpenDirectoryPathSource_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    selectedPath = fbd.SelectedPath;
                    tbPathSource.Text = selectedPath;
                    lbLog.Items.Insert(0, $"Selected folder: {fbd.SelectedPath}");
                }
            }
        }

        private void BtnStartZipping_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(selectedPath))
            {
                if (rbCreateZipsInSourceDirectory.IsChecked == true)
                {
                    string[] folders = Directory.GetDirectories(selectedPath);
                    lbLog.Items.Insert(0, $"Found {folders.Length} folders in {selectedPath}");

                    pbProgress.Minimum = 0;
                    pbProgress.Maximum = folders.Length;
                    pbProgress.Value = 0;

                    for (var i = 0; i < folders.Length; i++)
                    {
                        var sourcePath = folders[i];
                        var zipPath = selectedPath + "\\" + new DirectoryInfo(sourcePath).Name + ".zip";

                        if (File.Exists(zipPath))
                        {
                            if (rbFileExistsDeleteFile.IsChecked == true)
                            {
                                File.Delete(zipPath);
                            }
                            if (rbFileExistsShowWarning.IsChecked == true)
                            {
                                System.Windows.MessageBox.Show("Zip-file already exists!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
                                continue;
                            }
                        }

                        ZipFile.CreateFromDirectory(sourcePath, zipPath);
                        pbProgress.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgressBar), System.Windows.Threading.DispatcherPriority.Background);

                        LogListBoxDelegate llb = UpdateLogListBox;
                        llb.Invoke(sourcePath, zipPath);
                    }
                }
            }
        }

        private void UpdateProgressBar()
        {
            pbProgress.Value += 1;
        }

        private void UpdateLogListBox(string source, string target)
        {
            lbLog.Items.Insert(0, $"Zipped folder {source} to {target}");
        }

        private void TbPathSource_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            selectedPath = tbPathSource.Text;
            lbLog.Items.Insert(0, $"Selected folder: {selectedPath}");
        }
    }
}
