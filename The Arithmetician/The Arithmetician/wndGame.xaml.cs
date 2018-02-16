// Original Background Music Source: https://www.youtube.com/watch?v=944B3iryl9Q

using System;
using System.ComponentModel;
using System.Globalization;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace The_Arithmetician
{
    /// <summary>
    /// Interaction logic for wndGame.xaml
    /// </summary>
    public partial class Game : Window
    {
        /// <summary>
        /// This holds the data and logic for the game.
        /// </summary>
        internal ArithmeticianGame ArithmeticianGame { get; set; }

        /// <summary>
        /// The game user
        /// </summary>
        private User CurrentUser { get; }

        /// <summary>
        /// This is the current game type of the game.
        /// </summary>
        private GameType GameType { get; }

        /// <summary>
        /// This plays the background music of the game.
        /// </summary>
        private SoundPlayer BackgroundMusicSoundPlayer { get; }

        /// <summary>
        /// To show the main menu or not to show the main menu. That is the question.
        /// </summary>
        private bool ShowMainWindow { get; set; } = true;

        /// <summary>
        /// Initializes the game window
        /// </summary>
        public Game()
        {
            InitializeComponent();

            var backgroundMusicSoundPlayer = App.LoadAudio("/Resources/FFT Piano Antipyretic.wav");
            BackgroundMusicSoundPlayer = backgroundMusicSoundPlayer;
            backgroundMusicSoundPlayer.PlayLooping();
        }

        /// <summary>
        /// Constructor of the game window initializes it with a user and gametype information.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="gameType"></param>
        public Game(User user, GameType gameType) :
            this()
        {
            CurrentUser = user;
            GameType = gameType;
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
        /// Returns to the menu when clicked
        /// Also, closes the current game by setting it to null
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReturnToMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FinishGame();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Closes the game window and closes the game
        /// Goes to the High scores if the ten questions are completed.
        /// Else goes to the main menu.
        /// </summary>
        private void FinishGame()
        {
            btnStartGame.Visibility = Visibility.Visible;
            btnSubmit.Visibility = Visibility.Hidden;
            boxAnswer.Visibility = Visibility.Hidden;

            if (ArithmeticianGame != null && ArithmeticianGame.CurrentQuestion >= 10)
            {
                ShowMainWindow = false;
                new HighScores(CurrentUser, ArithmeticianGame.GameScore).Show();
            }

            Close();

            return;
        }

        /// <summary>
        /// Starts the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Make the game inputs visible
                btnStartGame.Visibility = Visibility.Hidden;
                btnSubmit.Visibility = Visibility.Visible;
                boxAnswer.Visibility = Visibility.Visible;

                // Focus on the Answer box
                Dispatcher.BeginInvoke(new Action(() => boxAnswer.Focus()));

                // Create the game
                ArithmeticianGame = new ArithmeticianGame(CurrentUser, GameType);
                lblCurrentQuestion.Content = ArithmeticianGame.CurrentQuestion;

                //Tell it which method will handle the click event
                ArithmeticianGame.GameTimer.Tick += new EventHandler(GameTimer_Tick);
                ArithmeticianGame.GameTimer.Start();

                // Display the question
                lblQuestion.Content = ArithmeticianGame.GenerateQuestion();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Goes off when the timer ticks
        /// Display the time which the game has ran.
        /// Ends the game after 60 minutes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (ArithmeticianGame == null)
                {
                    return;
                }

                var elapsed = ArithmeticianGame.Stopwatch.Elapsed;
                ArithmeticianGame.GameScore.GameDuration = elapsed;

                if (elapsed.TotalMinutes >= 60)
                {
                    FinishGame();
                }

                if (elapsed >= TimeSpan.FromMinutes(1))
                {
                    lblGameTimer.Content = $"Game Duration:\r\n{elapsed.TotalMinutes.ToString("N0", NumberFormatInfo.CurrentInfo)} minutes {elapsed.Seconds.ToString("N0", NumberFormatInfo.CurrentInfo)} seconds";
                }
                else
                {
                    lblGameTimer.Content = $"Game Duration:\r\n{elapsed.TotalSeconds.ToString("N0", NumberFormatInfo.CurrentInfo)} seconds";
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Submits the current answer to the game,
        /// Checks if it is right and show if it is
        /// Then clears the answer and shows the next
        /// If the ten question is answered, then go to the high score screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SubmitAnswer();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Restrict user input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Answer_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                //Only allow numbers to be entered
                if (!((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                  e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                {
                    //Allow the user to use the backspace and delete keys
                    if (!(e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Enter))
                    {
                        //No other keys allowed besides numbers, backspace, and delete
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Submits the Answer to the game
        /// </summary>
        private async Task SubmitAnswer()
        {
            // Check the answer
            int.TryParse(boxAnswer.Text.ToString(), out int answer);
            if (ArithmeticianGame.CheckAnswer(lblQuestion.Content.ToString(), answer))
            {
                lblPreviousCorrect.Content = "O";
                ArithmeticianGame.GameScore.CorrectAnswerCount++;
                lblPreviousCorrect.Foreground = Brushes.Green;
            }
            else
            {
                lblPreviousCorrect.Content = "X";
                ArithmeticianGame.GameScore.IncorrectAnswerCount--;
                lblPreviousCorrect.Foreground = Brushes.Red;
            }

            // Finish the game if it was the tenth question
            if (ArithmeticianGame.CurrentQuestion >= 10)
            {
                FinishGame();
                return;
            }

            ArithmeticianGame.CurrentQuestion++;

            // Change the labels then clear and focus the answer box
            lblCurrentQuestion.Content = ArithmeticianGame.CurrentQuestion;
            lblQuestion.Content = ArithmeticianGame.GenerateQuestion();
            boxAnswer.Text = null;
            boxAnswer.Focus();

            // Show the if the previous answer is correct or not for a second
            await Task.Delay(1000);

            var previousCorrect = lblPreviousCorrect.Content as string;

            if (!string.IsNullOrWhiteSpace(previousCorrect))
            {
                lblPreviousCorrect.Content = null;
                lblPreviousCorrect.Background = Brushes.Transparent;
            }
        }

        /// <summary>
        /// Closes the window and shuts down the music. Then shows the main menu.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                base.OnClosing(e);

                var backgroundMusicSoundPlayer = BackgroundMusicSoundPlayer;

                if (ShowMainWindow)
                {
                    backgroundMusicSoundPlayer.Stop();
                    new MainWindow(CurrentUser).Show();
                }

                backgroundMusicSoundPlayer.Dispose();

                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }
    }
}