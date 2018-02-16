using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Student_Scores
{
    /// <summary>
    /// Used To input and output student scores
    /// </summary>
    public class InputOutput
    {
        /// <summary>
        /// Writes a string of all the student scores
        /// </summary>
        /// <param name="scoreItems"></param>
        /// <returns></returns>
        internal static string WriteScoresString(IEnumerable<ScoreItem> scoreItems)
        {
            try
            {
                // String for the header
                var scoreslist = new StringBuilder("STUDENT \t");

                for (int index = 0; index < scoreItems.First().Scores.Count; index++)
                {
                    int scoreIndex = index + 1;
                    scoreslist.Append( "#" + scoreIndex + " \t");
                }

                scoreslist.AppendLine("AVG \t GRADE");

                // String for the scores
                foreach (var scoreItem in scoreItems)
                {
                    scoreslist.Append(scoreItem.StudentName + " \t");

                    foreach (var score in scoreItem.Scores)
                    {
                        scoreslist.Append(score + " \t");
                    }

                    scoreslist.Append($"{scoreItem.Average.ToString("N2", NumberFormatInfo.CurrentInfo)}% \t");
                    scoreslist.AppendLine(scoreItem.Grade);
                }

                return scoreslist.ToString();
            }
            catch (Exception ex)
            {
                Error.HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);

                return null;
            }
        }

        /// <summary>
        /// Saves scores Asynchronously to a string for a txt file
        /// 
        /// Don't need a try catch because the async will marshal the exception to the UI thread, which already has a try-catch
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="scoreItems"></param>
        /// <returns></returns>
        internal static Task SaveScoresAsync(string filePath, IEnumerable<ScoreItem> scoreItems)
        {
            return Task.Run(() => SaveScores(filePath, scoreItems));
        }

        
        /// <summary>
        /// Saves the scores to a text file
        /// 
        /// Don't need a try catch because the async will marshal the exception to the UI thread, which already has a try-catch
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="scoreItems"></param>
        private static void SaveScores(string filePath, IEnumerable<ScoreItem> scoreItems)
        {
            var text = InputOutput.WriteScoresString(scoreItems);
            File.WriteAllText(filePath, text);

            //Since writing to the file will be fast, we need to simulate a process that will take a while.  
            //We will do this by putting the thread to sleep for 2 seconds.
            Thread.Sleep(2000);
        }

        /// <summary>
        /// Saves scores Asynchronously in JSON format
        /// 
        /// Don't need a try catch because the async will marshal the exception to the UI thread, which already has a try-catch
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="studentScores"></param>
        /// <returns></returns>
        internal static Task SaveStudentScoresAsync(string filePath, StudentScores studentScores)
        {
            return Task.Run(() => SaveStudentScores(filePath, studentScores));
        }

        /// <summary>
        /// Saves scores in JSON format
        /// 
        /// Don't need a try catch because the async will marshal the exception to the UI thread, which already has a try-catch
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="studentScores"></param>
        private static void SaveStudentScores(string filePath, StudentScores studentScores)
        {
            var jsonSerializer = new JsonSerializer();

            using (var streamWriter = File.CreateText(filePath))
            {
                jsonSerializer.Serialize(streamWriter, studentScores);
            }
        }

        /// <summary>
        /// Loads the scores Asynchronously from a JSON file from the filePath
        /// 
        /// Don't need a try catch because the async will marshal the exception to the UI thread, which already has a try-catch
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        internal static Task<StudentScores> LoadStudentScoresAsync(string filePath)
        {
            return Task.Run(() => LoadStudentScores(filePath));
        }

        /// <summary>
        /// Loads the scores from a JSON file from the filePath
        /// 
        /// Don't need a try catch because the async will marshal the exception to the UI thread, which already has a try-catch
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static StudentScores LoadStudentScores(string filePath)
        {
            var jsonSerializer = new JsonSerializer();

            using (var streamReader = File.OpenText(filePath))
            {
                var jsonReader = new JsonTextReader(streamReader);
                return jsonSerializer.Deserialize<StudentScores>(jsonReader);
            }
        }
    }
}
