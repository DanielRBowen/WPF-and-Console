using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Die_Guess_Game
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// This is the model of the die guess game view
		private DieGuessGame dieGuessGame;

		// These are the images of each die stored as BitmapImages
		private BitmapImage die1;
		private BitmapImage die2;
		private BitmapImage die3;
		private BitmapImage die4;
		private BitmapImage die5;
		private BitmapImage die6;

		/// <summary>
		/// The Default Constructor of the Main Window of the Application
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
			LoadImages();
			ResetGame();
			Dispatcher.BeginInvoke(new Action(() => guess.Focus()));

		}

		/// <summary>
		/// Clears the game and resets the elements in the window
		/// </summary>
		public void ResetGame()
		{
			// Create a new dieGuessGame that overwrites the previous one if there is one
			dieGuessGame = new DieGuessGame();

			DataContext = dieGuessGame;

		}

		/// <summary>
		/// Will roll the dice and record the results when the roll button is clicked
		/// Method has the async keyword that labels it as asynchoronous
		/// Doesn't waist cpu cycles
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void rollBtn_Click(object sender, RoutedEventArgs e)
		{
			// Check that the number is 1 through 6
			if (dieGuessGame.CurrentGuessNumber > 6 || dieGuessGame.CurrentGuessNumber < 1)
			{
				lblInvalidError.Visibility = Visibility.Visible;
				return;
			}
			else
			{
				lblInvalidError.Visibility = Visibility.Hidden;
			}

			// Roll the die between 3 to 10 times and record the face
			int face = 0;
			for (int dieChangeCount = dieGuessGame.GameRandom.Next(3, 11), index = 0; index <= dieChangeCount; index++)
			{
				/* 
				* Instead of using Thread.Sleep and causing the thread to stop for 100 milliseconds
				* Make the method asynchronous by labeling it with the keyword async and this next line
				* will delay the task and the thread will return to the thread pool to do work while the method is delayed
				*/
				await Task.Delay(100);
				//Thread.Sleep(100);

				int imageIndex = dieGuessGame.GameRandom.Next(1, 7);
				face = imageIndex;

				switch (imageIndex)
				{
					case 1:
						imgDieImage.Source = die1;
						break;
					case 2:
						imgDieImage.Source = die2;
						break;
					case 3:
						imgDieImage.Source = die3;
						break;
					case 4:
						imgDieImage.Source = die4;
						break;
					case 5:
						imgDieImage.Source = die5;
						break;
					case 6:
						imgDieImage.Source = die6;
						break;
					default:
						throw new InvalidOperationException("imageIndex had a wrong value");
				}
			}

			// Update the times played, times won, and times lost stats
			dieGuessGame.TimesPlayed++;
			if (dieGuessGame.CurrentGuessNumber == face)
			{
				dieGuessGame.TimesWon++;
			}
			else
			{
				dieGuessGame.TimesLost++;
			}

			//Find the statistics of current play for the data grid
			dieGuessGame.UseGameHistoryItem(dieGuessGame.CurrentGuessNumber).NumberTimesGuessed++;
			dieGuessGame.UseGameHistoryItem(face).Frequency++;
			dieGuessGame.UpdatePercent();

			// Update the game history items in the data grid. You have to refresh the items of the observable collection.
			dieGuessGame.RefreshGameHistoryItems();

			// Clear the CurrentGuess text box and focus on the box after the roll
			dieGuessGame.CurrentGuess = null;
			guess.Focus();
		}

		/// <summary>
		/// Resets the game, clears the CurrentGuess textbox and focuses it,
		/// when the reset button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void resetBtn_Click(object sender, RoutedEventArgs e)
		{
			ResetGame();

			dieGuessGame.CurrentGuess = null;
			guess.Focus();
		}

		/// <summary>
		/// Loads all the Images used in the Main window.
		/// </summary>
		private void LoadImages()
		{
			//die1 = new BitmapImage(new Uri("pack://application:,,,/Die Guess Game;component/Resources/die1.gif", UriKind.Absolute));
			die1 = LoadImage("/Resources/die1.gif");
			die2 = LoadImage("/Resources/die2.gif");
			die3 = LoadImage("/Resources/die3.gif");
			die4 = LoadImage("/Resources/die4.gif");
			die5 = LoadImage("/Resources/die5.gif");
			die6 = LoadImage("/Resources/die6.gif");
		}

		/// <summary>
		/// Loads an image resource
		/// Graphics are tedious
		/// The dies bulid action in Properties have to be a Resource
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
			image.Freeze(); //readonly

			return image;
		}
	}
}
