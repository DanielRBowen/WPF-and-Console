using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Die_Guess_Game
{
	/// <summary>
	/// A DieGuessGame hold all the data for a game in which
	/// the player guesses what the die will be when it is rolled
	/// a couple of times
	/// </summary>
	internal class DieGuessGame : INotifyPropertyChanged
    {
		/// <summary>
		/// These are the GameHistoryItems which attributes will be binded to the data grid.
		/// The class uses the INotifyPropertyChanged interface so that xaml elements can be updated when bound to 
		/// the class when ever the property is changed
		/// </summary>
		public GameHistoryItem Face1History { get; set; }

		public GameHistoryItem Face2History { get; set; }

		public GameHistoryItem Face3History { get; set; }

		public GameHistoryItem Face4History { get; set; }

		public GameHistoryItem Face5History { get; set; }

		public GameHistoryItem Face6History { get; set; }

		/// <summary>
		/// This ObservableCollection holds all the items that can be displayed a data grid
		/// It holds the game history.
		/// </summary>
		public ObservableCollection<GameHistoryItem> GameHistoryItems { get; } = new ObservableCollection<GameHistoryItem>();

		/// <summary>
		/// Handles the PropertyChanged event on the class.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// The Random Number Generator of the Die Guess Game
		/// </summary>
		public Random GameRandom { get; set; }

		/// <summary>
		/// This is so I can convert the Current Guess that the user chooses to an integer
		/// </summary>
		public int CurrentGuessNumber
		{
			get
			{
				int value;
				int.TryParse(CurrentGuess, out value);
				return value;
			}
		}

		/// <summary>
		/// This is the Current Guess Number that the user chooses
		/// </summary>
		private string currentGuess;
		public string CurrentGuess
		{
			get { return currentGuess; }
			set
			{
				if (value != currentGuess)
				{
					currentGuess = value;
					NotifyPropertyChanged();
				}
			}
		}

		/// <summary>
		/// This is how many times the die has been rolled
		/// </summary>
		private int timesPlayed;
		public int TimesPlayed
		{
			get { return timesPlayed; }
			set
			{
				if (value != timesPlayed)
				{
					timesPlayed = value;
					NotifyPropertyChanged();
				}
			}
		}

		/// <summary>
		/// This is how many times the game has been won
		/// </summary>
		private int timesWon;
		public int TimesWon
		{
			get { return timesWon; }
			set
			{
				if (value != timesWon)
				{
					timesWon = value;
					NotifyPropertyChanged();
				}
			}
		}

		/// <summary>
		/// This is how many times the game has been lost.
		/// </summary>
		private int timesLost;
		public int TimesLost
		{
			get { return timesLost; }
			set
			{
				if (value != timesLost)
				{
					timesLost = value;
					NotifyPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Default Constructor for a die guess game.
		/// </summary>
		public DieGuessGame()
        {
            this.GameRandom = new Random();

			this.Face1History = new GameHistoryItem(1);
			this.Face2History = new GameHistoryItem(2);
			this.Face3History = new GameHistoryItem(3);
			this.Face4History = new GameHistoryItem(4);
			this.Face5History = new GameHistoryItem(5);
			this.Face6History = new GameHistoryItem(6);

			RefreshGameHistoryItems();
		}

		/// <summary>
		/// The PropertyChanged handler is invoked
		/// when the CurrentGuess, TimesPlayed, TimesWon, and TimesLost are set
		/// </summary>
		/// <param name="propertyName"></param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

		/// <summary>
		/// To change the GameHistory Data grid, the items has to be refreshed:
		/// Cleared and add the items again because it is an Observable Collection
		/// </summary>
		public void RefreshGameHistoryItems()
		{
			GameHistoryItems.Clear();

			this.GameHistoryItems.Add(Face1History);
			this.GameHistoryItems.Add(Face2History);
			this.GameHistoryItems.Add(Face3History);
			this.GameHistoryItems.Add(Face4History);
			this.GameHistoryItems.Add(Face5History);
			this.GameHistoryItems.Add(Face6History);
		}

		/// <summary>
		/// This method was created so it would be easier to get a game history
		/// item property to modify.
		/// </summary>
		/// <param name="face"></param>
		/// <returns></returns>
		public GameHistoryItem UseGameHistoryItem(int face)
		{
			switch (face)
			{
				case 1:
					return this.Face1History;
				case 2:
					return this.Face2History;
				case 3:
					return this.Face3History;
				case 4:
					return this.Face4History;
				case 5:
					return this.Face5History;
				case 6:
					return this.Face6History;
			}

			return null;
		}

		/// <summary>
		/// Updates the percent of each game history item
		/// Make sure you update the frequency first.
		/// </summary>
		public void UpdatePercent()
		{
			// Count each frequency of dieGuessGame history items
			float totalFrequency = 0;
			for (int index = 1; index <= 6; index++)
			{
				totalFrequency += UseGameHistoryItem(index).Frequency;
			}

			//  update each dieGuessGame percent as its frequency over the total frequency of the items.
			for (int index = 1; index <= 6; index++)
			{
				float percent = ((float)UseGameHistoryItem(index).Frequency / totalFrequency) * 100;
				UseGameHistoryItem(index).Percent = $"{percent.ToString("N2", NumberFormatInfo.CurrentInfo)}%";
			}
		}
	}
}
