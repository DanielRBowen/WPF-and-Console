/*
 * Original Win/Lose Music Source:
 * Win: https://www.youtube.com/watch?v=zFurcPw5ibg
 * Lose: https://www.youtube.com/watch?v=944B3iryl9Q
 */
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Media;

namespace The_Arithmetician
{
    /// <summary>
    /// Interaction logic for wndHighScores.xaml
    /// </summary>
    public partial class HighScores : Window
    {
        /// <summary>
        /// The user who is currently viewing the high scores
        /// </summary>
        private User User { get; }

        /// <summary>
        /// Contains the top ten scores.
        /// </summary>
        private Scores Top10Scores { get; } = new Scores();

        /// <summary>
        /// Plays congratulatory music when score is over 5
        /// </summary>
        private SoundPlayer FanfareSoundPlayer { get; }

        /// <summary>
        /// Plays failure music when the score is less than 6
        /// </summary>
        private SoundPlayer GameOverSoundPlayer { get; }

        /// <summary>
        /// The path where the high scores data is.
        /// </summary>
        private string HighScoresFilePath
        {
            get
            {
                var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                folderPath = Path.Combine(folderPath, "The Arithmetician");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                return Path.Combine(folderPath, "High Scores");
            }
        }

        /// <summary>
        /// Initializes the high scores
        /// </summary>
        public HighScores()
        {
            InitializeComponent();

            var top10Scores = Top10Scores;
            top10Scores.Load(HighScoresFilePath);

            HighScoresDataGrid.ItemsSource = top10Scores;
        }

        /// <summary>
        /// Initializes the high scores with a user.
        /// </summary>
        /// <param name="user"></param>
        public HighScores(User user) :
            this()
        {
            User = user;
        }

        /// <summary>
        /// Initializes the high scores. Called when the user finishes a game.
        /// </summary>
        public HighScores(User user, Score score) :
            this(user)
        {
            YourScoreTextBlock.Text = score.CorrectAnswerCount.ToString("N0", NumberFormatInfo.CurrentInfo);

            if (Top10Scores.TryAddNewScore(score))
            {
                HighScoreTextBlock.Text = "New high score!";
                HighScoreTextBlock.Foreground = Brushes.Green;
            }
            else
            {
                HighScoreTextBlock.Text = "Too bad!";
                HighScoreTextBlock.Foreground = Brushes.Red;
            }

            Top10Scores.Save(HighScoresFilePath);

            CurrentScoreGrid.Visibility = Visibility.Visible;

            // Play congratulatory music when score is over 5
            // Play failure music when the score is less than 6
            if (score.CorrectAnswerCount > 5)
            {
                var fanfareSoundPlayer = App.LoadAudio("/Resources/FFT Piano Fanfare.wav");
                FanfareSoundPlayer = fanfareSoundPlayer;
                fanfareSoundPlayer.Play();
            }
            else
            {
                var gameOverSoundPlayer = App.LoadAudio("/Resources/FFT Piano Game Over.wav");
                GameOverSoundPlayer = gameOverSoundPlayer;
                gameOverSoundPlayer.Play();
            }
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
        /// Closes the high scores window when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReturnToMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Handles the closing. Stops the music and shows the main window.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                base.OnClosing(e);

                var fanfareSoundPlayer = FanfareSoundPlayer;
                var gameOverSoundPlayer = GameOverSoundPlayer;

                if (fanfareSoundPlayer != null)
                {
                    fanfareSoundPlayer.Stop();
                    fanfareSoundPlayer.Dispose();
                }
                else if (gameOverSoundPlayer != null)
                {
                    gameOverSoundPlayer.Stop();
                    gameOverSoundPlayer.Dispose();
                }

                new MainWindow(User).Show();

                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }
    }
}
