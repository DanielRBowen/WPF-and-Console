/*
 * Daniel Bowen
 * January 2017
 */

using System.Collections.ObjectModel;

namespace Student_Scores
{
    internal class StudentScores
	{
		/// <summary>
		/// The students array which contain the names of the students.
		/// </summary>
		public string[] Students { get; set; }

		/// <summary>
		/// Contains the assignment scores
		/// </summary>
		public float[,] Assignments { get; set; }

		/// <summary>
		/// The current student that the user can modify
		/// </summary>
		public int CurrentStudent { get; set; }

		/// <summary>
		/// It holds the game history.
		/// </summary>
		public ObservableCollection<ScoreItem> ScoreItems { get; } = new ObservableCollection<ScoreItem>();

		/// <summary>
		/// Default constructer initializes the Students, Assignments, and CurrentStudent
		/// </summary>
		/// <param name="studentCount"></param>
		/// <param name="assignmentNumberCount"></param>
		public StudentScores(int studentCount, int assignmentNumberCount)
		{
			Students = new string[studentCount];

			Assignments = new float[studentCount, assignmentNumberCount];

			CurrentStudent = 0;

			RefreshScores();
		}

		/// <summary>
		/// To change the Scores Data grid, the items has to be refreshed:
		/// Cleared and add the items again because it is an Observable Collection
		/// </summary>
		public void RefreshScores()
		{
			ScoreItems.Clear();

			for (int index = 0; index < Students.Length; index++)
			{
				ScoreItem scoreItem = new ScoreItem();

				scoreItem.StudentName = Students[index];

				for (int assignmentsIndex = 0; assignmentsIndex < Assignments.GetLength(1); assignmentsIndex++)
				{
					scoreItem.Scores.Add(Assignments[index, assignmentsIndex]);
				}

				ScoreItems.Add(scoreItem);
			}
		}
	}
}
