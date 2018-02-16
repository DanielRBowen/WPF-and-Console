/*
 * Daniel Bowen
 * January 2017
 */

using System;
using System.Threading.Tasks;

namespace Tic_Tac_Toe
{
	internal class ComputerPlayer
	{
		private MainWindow ComputerMainWindow { get; set; }

		/// <summary>
		/// The Tic-Tac-Toe game which the computer plays on.
		/// </summary>
		private TicTacToeGame ComputerTicTacToeGame { get; set; }

		/// <summary>
		/// The TicTacToeToken which the computer plays
		/// </summary>
		public TicTacToeGame.TicTacToeToken ComputerToken { get; set; }

		/// <summary>
		/// The default constructer needs the game that the computer plays on and the token that it plays
		/// </summary>
		/// <param name="ticTacToeGame"></param>
		/// <param name="ticTacToeToken"></param>
		public ComputerPlayer(TicTacToeGame ticTacToeGame, TicTacToeGame.TicTacToeToken ticTacToeToken, MainWindow mainWindow)
		{
			ComputerTicTacToeGame = ticTacToeGame;
			ComputerToken = ticTacToeToken;
			ComputerMainWindow = mainWindow;
		}
		/// <summary>
		/// Makes a move for the current player
		/// Should be called after the last player has made a move when the computer exists
		/// Also is called when the AI is created.
		/// </summary>
		public async void MakeMove()
		{
			// The computer takes a second to make it's move.
			ComputerTicTacToeGame.Paused = true;
			await Task.Delay(1000);
			ComputerTicTacToeGame.Paused = false;

			if (TryMakeWinningMove())
			{
				return;
			}
			else if (TryBlockWinningMove())
			{
				return;
			}
			else if (TryMiddleSpot())
			{
				return;
			}
			else if (TrySpotAdjacentToOpponent())
			{
				return;
			}
			else if (TryFirstEmptySpot())
			{
				return;
			}
			else
			{
				throw new InvalidOperationException("MakeMove method of ComputerPlayer didn't make a move.");
			}
		}

		/// <summary>
		/// The computer will look for winning moves and make it if there is one.
		/// </summary>
		/// <returns></returns>
		private bool TryMakeWinningMove()
		{
			if (TryWinningMove(ComputerToken))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// The computer will look for winning moves and block it if there is one.
		/// Can only block one winning move seen. If there are two then the player can win.
		/// </summary>
		/// <returns></returns>
		private bool TryBlockWinningMove()
		{
			TicTacToeGame.TicTacToeToken opponentToken = FindOpponentToken();

			if(TryWinningMove(opponentToken))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// The computer will either make a winning move or block on depending on the token passed
		/// Can only block one winning move seen. If there are two then the player can win.
		/// </summary>
		/// <returns></returns>
		private bool TryWinningMove(TicTacToeGame.TicTacToeToken token)
		{
			// Check top to bottom
			if (ComputerTicTacToeGame.TicTacToeBoard[0] == token && ComputerTicTacToeGame.TicTacToeBoard[3] == token && ComputerTicTacToeGame.TicTacToeBoard[6] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(6);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[1] == token && ComputerTicTacToeGame.TicTacToeBoard[4] == token && ComputerTicTacToeGame.TicTacToeBoard[7] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(7);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[2] == token && ComputerTicTacToeGame.TicTacToeBoard[5] == token && ComputerTicTacToeGame.TicTacToeBoard[8] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(8);
				return true;
			}

			// Check bottom to top
			if (ComputerTicTacToeGame.TicTacToeBoard[6] == token && ComputerTicTacToeGame.TicTacToeBoard[3] == token && ComputerTicTacToeGame.TicTacToeBoard[0] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(0);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[7] == token && ComputerTicTacToeGame.TicTacToeBoard[4] == token && ComputerTicTacToeGame.TicTacToeBoard[1] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(1);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[8] == token && ComputerTicTacToeGame.TicTacToeBoard[5] == token && ComputerTicTacToeGame.TicTacToeBoard[2] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(2);
				return true;
			}

			// Check right to left
			if (ComputerTicTacToeGame.TicTacToeBoard[0] == token && ComputerTicTacToeGame.TicTacToeBoard[1] == token && ComputerTicTacToeGame.TicTacToeBoard[2] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(2);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[3] == token && ComputerTicTacToeGame.TicTacToeBoard[4] == token && ComputerTicTacToeGame.TicTacToeBoard[5] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(5);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[6] == token && ComputerTicTacToeGame.TicTacToeBoard[7] == token && ComputerTicTacToeGame.TicTacToeBoard[8] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(8);
				return true;
			}

			// Check left to right
			if (ComputerTicTacToeGame.TicTacToeBoard[2] == token && ComputerTicTacToeGame.TicTacToeBoard[1] == token && ComputerTicTacToeGame.TicTacToeBoard[0] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(0);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[5] == token && ComputerTicTacToeGame.TicTacToeBoard[4] == token && ComputerTicTacToeGame.TicTacToeBoard[3] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(3);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[8] == token && ComputerTicTacToeGame.TicTacToeBoard[7] == token && ComputerTicTacToeGame.TicTacToeBoard[6] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(6);
				return true;
			}

			// Check diagonal starting from left
			if (ComputerTicTacToeGame.TicTacToeBoard[0] == token && ComputerTicTacToeGame.TicTacToeBoard[4] == token && ComputerTicTacToeGame.TicTacToeBoard[8] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(8);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[8] == token && ComputerTicTacToeGame.TicTacToeBoard[4] == token && ComputerTicTacToeGame.TicTacToeBoard[0] == TicTacToeGame.TicTacToeToken.Empty) // from the opposite direction
			{
				ComputerMainWindow.PlaceTicTacToeToken(0);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[8] == token && ComputerTicTacToeGame.TicTacToeBoard[0] == token && ComputerTicTacToeGame.TicTacToeBoard[4] == TicTacToeGame.TicTacToeToken.Empty) // from the opposite direction
			{
				ComputerMainWindow.PlaceTicTacToeToken(4);
				return true;
			}

			// Check diagonal starting from right
			if (ComputerTicTacToeGame.TicTacToeBoard[2] == token && ComputerTicTacToeGame.TicTacToeBoard[4] == token && ComputerTicTacToeGame.TicTacToeBoard[6] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(6);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[6] == token && ComputerTicTacToeGame.TicTacToeBoard[4] == token && ComputerTicTacToeGame.TicTacToeBoard[2] == TicTacToeGame.TicTacToeToken.Empty) // from the opposite direction
			{
				ComputerMainWindow.PlaceTicTacToeToken(2);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[6] == token && ComputerTicTacToeGame.TicTacToeBoard[2] == token && ComputerTicTacToeGame.TicTacToeBoard[4] == TicTacToeGame.TicTacToeToken.Empty) // from the opposite direction
			{
				ComputerMainWindow.PlaceTicTacToeToken(4);
				return true;
			}

			// Check the middle sides
			if (ComputerTicTacToeGame.TicTacToeBoard[0] == token && ComputerTicTacToeGame.TicTacToeBoard[6] == token && ComputerTicTacToeGame.TicTacToeBoard[3] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(3);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[0] == token && ComputerTicTacToeGame.TicTacToeBoard[2] == token && ComputerTicTacToeGame.TicTacToeBoard[1] == TicTacToeGame.TicTacToeToken.Empty) // from the opposite direction
			{
				ComputerMainWindow.PlaceTicTacToeToken(1);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[2] == token && ComputerTicTacToeGame.TicTacToeBoard[8] == token && ComputerTicTacToeGame.TicTacToeBoard[5] == TicTacToeGame.TicTacToeToken.Empty) // from the opposite direction
			{
				ComputerMainWindow.PlaceTicTacToeToken(5);
				return true;
			}
			else if (ComputerTicTacToeGame.TicTacToeBoard[6] == token && ComputerTicTacToeGame.TicTacToeBoard[8] == token && ComputerTicTacToeGame.TicTacToeBoard[5] == TicTacToeGame.TicTacToeToken.Empty) // from the opposite direction
			{
				ComputerMainWindow.PlaceTicTacToeToken(7);
				return true;
			}


			return false;
		}

		/// <summary>
		/// Computer tries the Middle spot of the tic-tac-toe board
		/// </summary>
		/// <returns>True if successful</returns>
		private bool TryMiddleSpot()
		{
			if (ComputerTicTacToeGame.TicTacToeBoard[4] == TicTacToeGame.TicTacToeToken.Empty)
			{
				ComputerMainWindow.PlaceTicTacToeToken(4);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Computer tries the spot which is adjacent to the player
		/// </summary>
		/// <returns>True if successful</returns>
		private bool TrySpotAdjacentToOpponent()
		{
			TicTacToeGame.TicTacToeToken opponentToken = FindOpponentToken();

			for (int boardPosition = 0; boardPosition < ComputerTicTacToeGame.TicTacToeBoard.Length; boardPosition++)
			{
				if (ComputerTicTacToeGame.TicTacToeBoard[boardPosition] == opponentToken)
				{
					// Check to the right of the opponent
					if (boardPosition != 8 && ComputerTicTacToeGame.TicTacToeBoard[boardPosition + 1] == TicTacToeGame.TicTacToeToken.Empty)
					{
						ComputerMainWindow.PlaceTicTacToeToken(boardPosition + 1);
						return true;
					}

					// Check the left of the opponent
					if (boardPosition != 0 && ComputerTicTacToeGame.TicTacToeBoard[boardPosition - 1] == TicTacToeGame.TicTacToeToken.Empty)
					{
						ComputerMainWindow.PlaceTicTacToeToken(boardPosition - 1);
						return true;
					}

					// Check the below the opponent
					if (boardPosition != 6 && boardPosition != 7 && boardPosition != 8 && ComputerTicTacToeGame.TicTacToeBoard[boardPosition + 3] == TicTacToeGame.TicTacToeToken.Empty)
					{
						ComputerMainWindow.PlaceTicTacToeToken(boardPosition + 3);
						return true;
					}

					// Check the above the opponent
					if (boardPosition != 0 && boardPosition != 1 && boardPosition != 2 && ComputerTicTacToeGame.TicTacToeBoard[boardPosition - 3] == TicTacToeGame.TicTacToeToken.Empty)
					{
						ComputerMainWindow.PlaceTicTacToeToken(boardPosition - 3);
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Find what the opponents token is
		/// </summary>
		/// <returns></returns>
		private TicTacToeGame.TicTacToeToken FindOpponentToken()
		{
			TicTacToeGame.TicTacToeToken opponentToken;
			if (ComputerToken == TicTacToeGame.TicTacToeToken.XToken)
			{
				opponentToken = TicTacToeGame.TicTacToeToken.OToken;
			}
			else
			{
				opponentToken = TicTacToeGame.TicTacToeToken.XToken;
			}

			return opponentToken;
		}

		/// <summary>
		/// Computer tries a random empty spot
		/// </summary>
		/// <returns>True if successful</returns>
		private bool TryFirstEmptySpot()
		{
			for (int boardPosition = 0; boardPosition < ComputerTicTacToeGame.TicTacToeBoard.Length; boardPosition++)
			{
				if (ComputerTicTacToeGame.TicTacToeBoard[boardPosition] == TicTacToeGame.TicTacToeToken.Empty)
				{
					ComputerMainWindow.PlaceTicTacToeToken(boardPosition);
					return true;
				}
			}
			return false;
		}
	}
}
