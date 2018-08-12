using System;
using System.Windows;

namespace TagFolderNameToAlbum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IncludeSubdirectories { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ChangeAlbumTagsButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeAlbumTagsButton.IsEnabled = false;

            try
            {
                var folderPath = FolderPathTextBox.Text;
                await ChangeAlbumTags.ChangeAlbumTagsOfFolderAsync(folderPath, IncludeSubdirectories);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message.ToString());
            }

            ChangeAlbumTagsButton.IsEnabled = true;
        }

        private void SubdirectoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (SubdirectoriesCheckBox.IsChecked.Value)
            {
                IncludeSubdirectories = true;
            }
        }

        private void SubdirectoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (SubdirectoriesCheckBox.IsChecked.Value == false)
            {
                IncludeSubdirectories = false;
            }
        }
    }
}
