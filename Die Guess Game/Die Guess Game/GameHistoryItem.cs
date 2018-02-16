using System.Globalization;

namespace Die_Guess_Game
{
	/// <summary>
	/// A game history item holds the history the game
	/// </summary>
    internal class GameHistoryItem
    {
		/// <summary>
		/// The face that was rolled in the game
		/// </summary>
		public int Face { get; set; }

		/// <summary>
		/// The frequency that the face was rolled throughout the game
		/// </summary>
		public int Frequency { get; set; }

		/// <summary>
		/// The percent that the face was rolled throughout the game
		/// </summary>
		public string Percent { get; set; }

		/// <summary>
		/// The number of times the user has guessed that roll.
		/// </summary>
		public int NumberTimesGuessed { get; set; }

		/// <summary>
		/// A method that I used when I found out that I was doing the game history wrong
		/// and made this funky implementation.
		/// </summary>
		/// <param name="face"></param>
		/// <param name="frequency"></param>
		/// <param name="percent"></param>
		/// <param name="timesGuessed"></param>
		public void SetAllHistoryItems(int face, int frequency, float percent, int timesGuessed)
		{
			this.Face = face;
			this.Frequency = frequency;
			this.Percent = $"{percent.ToString("N2", NumberFormatInfo.CurrentInfo)}%";
			this.NumberTimesGuessed = timesGuessed;
		}

		/// <summary>
		/// The default constructor for a game history item.
		/// </summary>
		/// <param name="face"></param>
		/// <param name="frequency"></param>
		/// <param name="percent"></param>
		/// <param name="timesGuessed"></param>
		public GameHistoryItem(int face = 0, int frequency = 0, float percent = 0, int timesGuessed = 0)
		{
			this.Face = face;
			this.Frequency = frequency;
			this.Percent = $"{percent.ToString("N2", NumberFormatInfo.CurrentInfo)}%";
			this.NumberTimesGuessed = timesGuessed;
		}
    }
}
