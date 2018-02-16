using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace The_Arithmetician
{
    /// <summary>
    /// The “Scores” class will be used to keep track of the top ten high scores.  
    /// This class should have the current score passed into it, and it should sort the top ten scores.
    /// </summary>
    public class Scores : IEnumerable<Score>
    {
        /// <summary>
        /// Contains the top Ten Scores
        /// </summary>
        private List<Score> Top10Scores { get; set; } = new List<Score>();

        /// <summary>
        /// Try to add a new score to the Top 10 Scores list.
        /// The high score list will be based off of the number of correct answers, then time.  
        /// So if two scores have the same number of correct answers, then the score with the lowest time will decide which one is a higher score.
        /// </summary>
        /// <param name="newScore"></param>
        /// <returns></returns>
        public bool TryAddNewScore(Score newScore)
        {
            Top10Scores.Add(newScore);

            Top10Scores.Sort((left, right) =>
            {
                var order = left.CorrectAnswerCount.CompareTo(right.CorrectAnswerCount);

                if (order != 0)
                {
                    return -order;
                }

                return left.GameDuration.CompareTo(right.GameDuration);
            });

            if (Top10Scores.Count > 10)
            {
                Top10Scores.RemoveRange(10, Top10Scores.Count - 10);
            }

            return Top10Scores.Contains(newScore);
        }

        /// <summary>
        /// Saves the High Scores at the location of the filepath.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        internal bool Save(string filePath)
        {
            try
            {
                using (var streamWriter = File.CreateText(filePath))
                {
                    new JsonSerializer().Serialize(streamWriter, Top10Scores);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Loads the High Scores at the location of the filepath.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        internal bool Load(string filePath)
        {
            try
            {
                using (var streamReader = File.OpenText(filePath))
                {
                    Top10Scores = (List<Score>)new JsonSerializer().Deserialize(streamReader, typeof(List<Score>));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Enumerator interface of scores from the Top10Scores list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Score> GetEnumerator() => Top10Scores.GetEnumerator();

        /// <summary>
        /// The matching return type of IEnumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}