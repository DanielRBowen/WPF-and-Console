/*
 * Daniel Bowen
 * January 2017
 */


namespace Tic_Tac_Toe
{
	internal class TicTacToeGame
	{
		/// <summary>
		/// The type of token that a player uses
		/// </summary>
		public enum TicTacToeToken
		{
			XToken,
			OToken,
			Empty
		}

		/// <summary>
		/// Indicates who won
		/// </summary>
		public enum TicTacToeWin
		{
			Player1,
			Player2,
			Tie,
			Nobody
		}

		/// <summary>
		/// The current tic tac toe player
		/// </summary>
		public TicTacToeToken CurrentTicTacToePlayer { get; set; }

		/// <summary>
		/// The tic tac toe board that the game is played on.
		/// </summary>
		public TicTacToeToken[] TicTacToeBoard { get; set; }

		/// <summary>
		/// The number of times that player 1 has won
		/// </summary>
		public int Player1Wins { get; set; }

		/// <summary>
		/// The number of times that player 2 has won
		/// </summary>
		public int Player2Wins { get; set; }

		/// <summary>
		/// The number of times that the game was a tie
		/// </summary>
		public int Ties { get; set; }

		/// <summary>
		/// Sets the game to be paused if true.
		/// </summary>
		public bool Paused { get; set; }

		/// <summary>
		/// The positions on the board from a winning stratagy
		/// </summary>
		public int[] TicTacToeWinPositions { get; set; }

		/// <summary>
		/// The default tic tac toe game constructor
		/// The first turn is for the X token.
		/// </summary>
		public TicTacToeGame()
		{
			CurrentTicTacToePlayer = TicTacToeToken.XToken;
			TicTacToeBoard = new TicTacToeToken[9];
			TicTacToeWinPositions = new int[3];

			ResetBoard();

			Player1Wins = 0;
			Player2Wins = 0;
			Ties = 0;

			Paused = false;
		}

		/// <summary>
		/// Fills the board with empty tokens.
		/// Should be called on initialization or win the game results are finalized.
		/// </summary>
		private void ResetBoard()
		{
			for (int index = 0; index < TicTacToeBoard.Length; index++)
			{
				TicTacToeBoard[index] = TicTacToeToken.Empty;
			}
		}

		/// <summary>
		/// Return true if there is a horizontal win for the corresponding token
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		private bool HorizontalWin(TicTacToeToken token)
		{
			for (int row = 0; row < 7; row += 3)
			{
				if (TicTacToeBoard[row] == token && TicTacToeBoard[row + 1] == token && TicTacToeBoard[row + 2] == token)
				{
					TicTacToeWinPositions[0] = row;
					TicTacToeWinPositions[1] = row + 1;
					TicTacToeWinPositions[2] = row + 2;
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Return true if there is a vertical win for the corresponding token
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		private bool VerticalWin(TicTacToeToken token)
		{
			for (int column = 0; column < 3; column++)
			{
				if (TicTacToeBoard[column] == token && TicTacToeBoard[column + 3] == token && TicTacToeBoard[column + 6] == token)
				{
					TicTacToeWinPositions[0] = column;
					TicTacToeWinPositions[1] = column + 3;
					TicTacToeWinPositions[2] = column + 6;
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Return true if there is a left diagonal win for the corresponding token
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		private bool LeftDiagonalWin(TicTacToeToken token)
		{
			if (TicTacToeBoard[0] == token && TicTacToeBoard[4] == token && TicTacToeBoard[8] == token)
			{
				TicTacToeWinPositions[0] = 0;
				TicTacToeWinPositions[1] = 4;
				TicTacToeWinPositions[2] = 8;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Return true if there is a right diagonal win for the corresponding token
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		private bool RightDiagonalWin(TicTacToeToken token)
		{
			if (TicTacToeBoard[2] == token && TicTacToeBoard[4] == token && TicTacToeBoard[6] == token)
			{
				TicTacToeWinPositions[0] = 2;
				TicTacToeWinPositions[1] = 4;
				TicTacToeWinPositions[2] = 6;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Check if the game is a tie when the players have peen checked and the board is filled up
		/// Ties should be checked for last after checking if the players have won
		/// </summary>
		private bool CheckForTie()
		{
			for (int index = 0; index < TicTacToeBoard.Length; index++)
			{
				if (TicTacToeBoard[index] == TicTacToeToken.Empty)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Checks if there is a win for the player with the corresponding token.
		/// If there is a win it passes the token back,
		/// passes a empty token if there isn't a win.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		private TicTacToeToken TryFinalizePlayer(TicTacToeToken token)
		{
			if (HorizontalWin(token) || VerticalWin(token) || LeftDiagonalWin(token) || RightDiagonalWin(token))
			{
				return token;
			}

			return TicTacToeToken.Empty;
		}

		/// <summary>
		/// Tries to finalize the game.
		/// Should be called when ever a player makes a move.
		/// Will update the Wins or tie if any of the conditions meet.
		/// The winning stratagy positions will be stored.
		/// </summary>
		/// <returns>true if the game has been finalized.</returns>
		public bool TryFinalize(out TicTacToeWin whoWon)
		{
			if (TryFinalizePlayer(TicTacToeToken.XToken) == TicTacToeToken.XToken)
			{
				Player1Wins++;
				ResetBoard();
				whoWon = TicTacToeWin.Player1;
				return true;
			}
			else if (TryFinalizePlayer(TicTacToeToken.OToken) == TicTacToeToken.OToken)
			{
				Player2Wins++;
				ResetBoard();
				whoWon = TicTacToeWin.Player2;
				return true;
			}
			else if (CheckForTie())
			{
				Ties++;
				ResetBoard();
				whoWon = TicTacToeWin.Tie;
				return true;
			}

			whoWon = TicTacToeWin.Nobody;
			return false;
		}
	}
}
