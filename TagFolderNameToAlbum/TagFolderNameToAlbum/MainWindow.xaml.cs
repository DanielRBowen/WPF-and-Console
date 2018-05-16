using System.Windows;

namespace TagFolderNameToAlbum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeAlbumTagsButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPath = FolderPathTextBox.Text;
            ChangeAlbumTags.ChangeAlbumTagsOfFolder(folderPath);
        }
    }
}
