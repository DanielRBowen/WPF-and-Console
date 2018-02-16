/*
 * Daniel Bowen
 * January 2017
 */

using System.Collections.Generic;

namespace Student_Scores
{
	internal class ScoreItem
	{
		/// <summary>
		/// The student Name of the scores
		/// </summary>
		public string StudentName { get; set; }

		/// <summary>
		/// The Scores of each student
		/// </summary>
		public List<float> Scores { get; set; }

		/// <summary>
		/// The average of each student
		/// </summary>
		public float Average
		{
			get
			{
				float total = 0;
				for (int index = 0; index < Scores.Count; index++)
				{
					total += Scores[index];
				}

				return total / Scores.Count;
			}
		}

		/// <summary>
		/// The grade in the class of the student
		/// </summary>
		public string Grade
		{
			get
			{
				if (Average >= 93)
				{
					return "A";
				}
				else if (Average <= 92.9 && Average >= 90)
				{						
					return "A-";		
				}						
				else if (Average <= 89.9 && Average >= 87)
				{						
					return "B+";		
				}						
				else if (Average <= 86.9 && Average >= 83)
				{						
					return "B";			
				}						
				else if (Average <= 82.9 && Average >= 80)
				{						
					return "B-";		
				}						
				else if (Average <= 79.9 && Average >= 77)
				{						
					return "C+";		
				}						
				else if (Average <= 76.9 && Average >= 73)
				{						
					return "C";			
				}						
				else if (Average <= 72.9 && Average >= 70)
				{						
					return "C-";		
				}						
				else if (Average <= 69.9 && Average >= 67)
				{						
					return "D+";		
				}						
				else if (Average <= 66.9 && Average >= 63)
				{						
					return "D";			
				}						
				else if (Average <= 62.9 && Average >= 60)
				{
					return "D-";
				}
				else if (Average <= 60)
				{
					return "E";
				}
				else
				{
					return "Invalid Grade";
				}
			}
		}

		/// <summary>
		/// The constructor has to initialize the Scores list
		/// </summary>
		public ScoreItem()
		{
			Scores = new List<float>();
		}
	}
}
