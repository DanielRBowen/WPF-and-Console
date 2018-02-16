// Original Background image source: https://www.kotaku.com.au/2013/01/why-everybody-loves-final-fantasy-tactics/
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace The_Arithmetician
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// This is the user data of the one to play the game.
        /// </summary>
        private User User { get; }

        /// <summary>
        /// Constructor to build the main window of the main menu
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Add the background which contains lore of the game - nice feature to have.
            BitmapImage ArithmeticianBackground = LoadImage("/Resources/Arithmetician.png");
            this.Background = new ImageBrush(ArithmeticianBackground);
        }

        /// <summary>
        /// Constructor of the main window which get the user passed to it.
        /// </summary>
        /// <param name="user"></param>
        public MainWindow(User user) :
            this()
        {
            User = user;
        }

        /// <summary>
        /// Shows an error message of the exception. Used to catch event errors.
        /// </summary>
        /// <param name="ex"></param>
        private void ShowErrorMessage(Exception ex)
        {
            MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows an error message with an error message passed to it.
        /// </summary>
        /// <param name="errorMessage"></param>
        private void ShowErrorMessage(string errorMessage)
        {
            MessageBox.Show(this, errorMessage, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        /// <summary>
        /// Hides this window and shows the game window.
        /// Only opens if there is user data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (User == null)
                {
                    ShowErrorMessage("Need user data.");
                    return;
                }

                MenuGrid.Visibility = Visibility.Collapsed;
                GameTypeGrid.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Goes to the Edit Player data window when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditPlayerInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new EnterUserData(User).Show();
                Close();
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Hides this window and shows the high scores window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HighScores_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new HighScores(User).Show();
                Close();
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Opens up a new Game window and closes this one.
        /// </summary>
        /// <param name="gameType"></param>
        private void NewGame(GameType gameType)
        {
            try
            {
                new Game(User, gameType).Show();
                Close();
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Changes the game type to addition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdditionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewGame(GameType.Addition);
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Changes the game type to subtraction.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubtractionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewGame(GameType.Subtraction);
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Changes the game type to Multiplication
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MultiplicationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewGame(GameType.Multiplication);
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Changes the game type to Division.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DivisionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewGame(GameType.Division);
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Shows the main menu items instead of the game type items whicn clicked on cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GameTypeGrid.Visibility = Visibility.Collapsed;
                MenuGrid.Visibility = Visibility.Visible;
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
		/// Loads an image resource
		/// </summary>
		/// <param name="relativeUrl"></param>
		/// <returns></returns>
		private static BitmapImage LoadImage(string relativeUrl)
        {
            var streamInfo = Application.GetResourceStream(new Uri(relativeUrl, UriKind.Relative));
            var image = new BitmapImage();

            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = streamInfo.Stream;
            image.EndInit();
            image.Freeze();

            return image;
        }
    }
}
