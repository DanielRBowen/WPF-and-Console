/*
 * Daniel Bowen
 * January 2017
 */

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Tic_Tac_Toe
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// A computer player which will be used instead of another person when toggled on
		private ComputerPlayer computerPlayer1;

		// A second computer player so that the player can have two computers against each other.
		private ComputerPlayer computerPlayer2;

		// The TicTacToeGame which holds the logic of the game.
		private TicTacToeGame ticTacToeGame;

		/// <summary>
		/// The Default Constructor of the Main Window of the Application
		/// Sets the game up,
		/// and loads the images that are used in the game.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
			ResetGame();
		}

		/// <summary>
		/// Starts a new game or resets the game board when the Start Game button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnStartGame_Click(object sender, RoutedEventArgs e)
		{
			// Can't start a new game if the game is paused.
			if (ticTacToeGame.Paused)
			{
				// Got to reset the game and update the UI when the game is pause and both AIs are running which pause the game
				if (computerPlayer1 != null && computerPlayer2 != null)
				{
					ResetGame();

					UpdateUI();
				}

				return;
			}

			ResetGame();

			UpdateUI();
		}

		/// <summary>
		/// Resets the Tic-Tac-Toe game.
		/// Clears the board of images.
		/// Resets the labels and turn of the player.
		/// Sets the computerPlayers to null.
		/// </summary>
		private void ResetGame()
		{
			computerPlayer1 = null;
			computerPlayer2 = null;

			ticTacToeGame = new TicTacToeGame();

			ResetBoardContent();
		}

		/// <summary>
		/// Resets the Tic tac toe board view contents.
		/// </summary>
		private void ResetBoardContent()
		{
			btnx0y0.Content = "";
			btnx1y0.Content = "";
			btnx2y0.Content = "";
			btnx0y1.Content = "";
			btnx1y1.Content = "";
			btnx2y1.Content = "";
			btnx0y2.Content = "";
			btnx1y2.Content = "";
			btnx2y2.Content = "";

			btnx0y0.Background = Brushes.LightGray;
			btnx1y0.Background = Brushes.LightGray;
			btnx2y0.Background = Brushes.LightGray;
			btnx0y1.Background = Brushes.LightGray;
			btnx1y1.Background = Brushes.LightGray;
			btnx2y1.Background = Brushes.LightGray;
			btnx0y2.Background = Brushes.LightGray;
			btnx1y2.Background = Brushes.LightGray;
			btnx2y2.Background = Brushes.LightGray;
		}

		/// <summary>
		/// The click on this button will place a X or O image in the box
		/// which the button resides.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnx0y0_Click(object sender, RoutedEventArgs e)
		{
			PlaceTicTacToeToken(0);
		}

		/// <summary>
		/// The click on this button will place a X or O image in the box
		/// which the button resides.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnx1y0_Click(object sender, RoutedEventArgs e)
		{
			PlaceTicTacToeToken(1);
		}

		/// <summary>
		/// The click on this button will place a X or O image in the box
		/// which the button resides.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnx2y0_Click(object sender, RoutedEventArgs e)
		{
			PlaceTicTacToeToken(2);
		}

		/// <summary>
		/// The click on this button will place a X or O image in the box
		/// which the button resides.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnx0y1_Click(object sender, RoutedEventArgs e)
		{
			PlaceTicTacToeToken(3);
		}

		/// <summary>
		/// The click on this button will place a X or O image in the box
		/// which the button resides.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnx1y1_Click(object sender, RoutedEventArgs e)
		{
			PlaceTicTacToeToken(4);
		}

		/// <summary>
		/// The click on this button will place a X or O image in the box
		/// which the button resides.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnx2y1_Click(object sender, RoutedEventArgs e)
		{
			PlaceTicTacToeToken(5);
		}

		/// <summary>
		/// The click on this button will place a X or O image in the box
		/// which the button resides.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnx0y2_Click(object sender, RoutedEventArgs e)
		{
			PlaceTicTacToeToken(6);
		}

		/// <summary>
		/// The click on this button will place a X or O image in the box
		/// which the button resides.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnx1y2_Click(object sender, RoutedEventArgs e)
		{
			PlaceTicTacToeToken(7);
		}

		/// <summary>
		/// The click on this button will place a X or O image in the box
		/// which the button resides.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnx2y2_Click(object sender, RoutedEventArgs e)
		{
			PlaceTicTacToeToken(8);
		}

		/// <summary>
		/// Places the tic tac toe token on the place specified.
		/// The position has to be 0 through 8 inclusive.
		/// Then Updates the UI based on the results of that placement.
		/// </summary>
		/// <param name="position"></param>
		public void PlaceTicTacToeToken(int position)
		{
			string content = "";

			if (ticTacToeGame == null || ticTacToeGame.TicTacToeBoard[position] != TicTacToeGame.TicTacToeToken.Empty || ticTacToeGame.Paused)
			{
				return;
			}

			if (ticTacToeGame.CurrentTicTacToePlayer == TicTacToeGame.TicTacToeToken.XToken)
			{
				content = "X";
				ticTacToeGame.TicTacToeBoard[position] = TicTacToeGame.TicTacToeToken.XToken;
				ticTacToeGame.CurrentTicTacToePlayer = TicTacToeGame.TicTacToeToken.OToken;
			}
			else
			{
				content = "O";
				ticTacToeGame.TicTacToeBoard[position] = TicTacToeGame.TicTacToeToken.OToken;
				ticTacToeGame.CurrentTicTacToePlayer = TicTacToeGame.TicTacToeToken.XToken;
			}

			switch (position)
			{
				case 0:
					btnx0y0.Content = content;
					break;
				case 1:
					btnx1y0.Content = content;
					break;
				case 2:
					btnx2y0.Content = content;
					break;
				case 3:
					btnx0y1.Content = content;
					break;
				case 4:
					btnx1y1.Content = content;
					break;
				case 5:
					btnx2y1.Content = content;
					break;
				case 6:
					btnx0y2.Content = content;
					break;
				case 7:
					btnx1y2.Content = content;
					break;
				case 8:
					btnx2y2.Content = content;
					break;
				default:
					throw new InvalidOperationException("PlaceTicTacToeToken had an unexpected value in it's switch statement.");
			}

			UpdateUI();
		}

		/// <summary>
		/// Updates the UI with the data in the model.
		/// And starts the turn for the computer if ther is one.
		/// </summary>
		private async void UpdateUI()
		{
			TicTacToeGame.TicTacToeWin whoWon;
			if (ticTacToeGame.TryFinalize(out whoWon))
			{
				string content = "";

				// Display that somebody won for 5 seconds
				switch (whoWon)
				{
					case TicTacToeGame.TicTacToeWin.Player1:
						content = "Player 1 Wins";
						DisplayWinningMove(TicTacToeGame.TicTacToeToken.XToken);
						break;
					case TicTacToeGame.TicTacToeWin.Player2:
						DisplayWinningMove(TicTacToeGame.TicTacToeToken.OToken);
						content = "Player 2 Wins";
						break;
					case TicTacToeGame.TicTacToeWin.Tie:
						content = "The game was a Tie";
						break;
					default:
						throw new InvalidOperationException("UpdateUI had a unexpected value of whoWon");
				}

				ticTacToeGame.Paused = true;
				for (int seconds = 5; seconds > 0; seconds--)
				{
					lblGameStatus.Content = content + "\nThe next round will start in " + seconds + " seconds";
					await Task.Delay(1000);
				}
				ticTacToeGame.Paused = false;

				ResetBoardContent();
			}

			// Update the wins.
			lblPlayer1Wins.Content = "Player 1 Wins: " + ticTacToeGame.Player1Wins;
			lblPlayer2Wins.Content = "Player 2 Wins: " + ticTacToeGame.Player2Wins;
			lblTies.Content = "Ties: " + ticTacToeGame.Ties;

			if (ticTacToeGame.CurrentTicTacToePlayer == TicTacToeGame.TicTacToeToken.XToken)
			{
				lblGameStatus.Content = "Player 1's Turn";
			}
			else
			{
				lblGameStatus.Content = "Player 2's Turn";
			}

			// This is the end of processing for a turn, you can have a human or computer do a turn now.
			if (computerPlayer1 != null && computerPlayer1.ComputerToken == ticTacToeGame.CurrentTicTacToePlayer)
			{
				computerPlayer1.MakeMove();
			}
			else if (computerPlayer2 != null && computerPlayer2.ComputerToken == ticTacToeGame.CurrentTicTacToePlayer)
			{
				computerPlayer2.MakeMove();
			}
		}

		/// <summary>
		/// Highlights the winning move in yellow.
		/// </summary>
		private void DisplayWinningMove(TicTacToeGame.TicTacToeToken token)
		{
			for (int index = 0; index < ticTacToeGame.TicTacToeWinPositions.Length; index++)
			{
				int position = ticTacToeGame.TicTacToeWinPositions[index];
				switch (position)
				{
					case 0:
						btnx0y0.Background = Brushes.Yellow;
						break;
					case 1:
						btnx1y0.Background = Brushes.Yellow;
						break;
					case 2:
						btnx2y0.Background = Brushes.Yellow;
						break;
					case 3:
						btnx0y1.Background = Brushes.Yellow;
						break;
					case 4:
						btnx1y1.Background = Brushes.Yellow;
						break;
					case 5:
						btnx2y1.Background = Brushes.Yellow;
						break;
					case 6:
						btnx0y2.Background = Brushes.Yellow;
						break;
					case 7:
						btnx1y2.Background = Brushes.Yellow;
						break;
					case 8:
						btnx2y2.Background = Brushes.Yellow;
						break;
					default:
						throw new InvalidOperationException("DisplayWinningMove method had an unexpected value.");
				}
			}
		}

		/// <summary>
		/// Toggles the AI for the current player
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnToggleAIforCurrentPlayer_Click(object sender, RoutedEventArgs e)
		{
			if (ticTacToeGame.Paused)
			{
				return;
			}

			if (computerPlayer1 == null)
			{
				computerPlayer1 = new ComputerPlayer(this.ticTacToeGame, ticTacToeGame.CurrentTicTacToePlayer, this);
				computerPlayer1.MakeMove();
			}
			else if (computerPlayer2 == null && this.ticTacToeGame.CurrentTicTacToePlayer != computerPlayer1.ComputerToken)
			{
				computerPlayer2 = new ComputerPlayer(this.ticTacToeGame, ticTacToeGame.CurrentTicTacToePlayer, this);
				computerPlayer2.MakeMove();
			}
			else
			{
				return;
			}

		}
	}
}
