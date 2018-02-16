using System;

namespace The_Arithmetician
{
    /// <summary>
    /// The score of an arithmetician game.
    /// Contains the User data, correct answer count, incorrect answer count, and the game duration.
    /// </summary>
    public class Score
    {
        /// <summary>
        /// The user who has the score
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Number of Right Answers
        /// </summary>
        public int CorrectAnswerCount { get; set; }

        /// <summary>
        /// Number of Wrong Answers
        /// </summary>
        public int IncorrectAnswerCount { get; set; }

        /// <summary>
        /// How long the game took
        /// </summary>
        public TimeSpan GameDuration { get; set; }
    }
}