using System.Windows;
using System.Windows.Media;

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

        private void ChangeAlbumTagsButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeAlbumTagsButton.Background = Brushes.Red;
            var folderPath = FolderPathTextBox.Text;
            ChangeAlbumTags.ChangeAlbumTagsOfFolder(folderPath, IncludeSubdirectories);
            ChangeAlbumTagsButton.Background = Brushes.Green;
        }

        private void SubdirectoriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (SubdirectoriesButton.Background == Brushes.Red)
            {
                SubdirectoriesButton.Background = Brushes.Green;
                IncludeSubdirectories = true;
            }
            else
            {
                SubdirectoriesButton.Background = Brushes.Red;
                IncludeSubdirectories = false;
            }
        }
    }
}
