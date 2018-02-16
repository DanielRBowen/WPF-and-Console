using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace The_Arithmetician
{
    /// <summary>
    /// Game logic for a Arithmetician game.
    /// </summary>
    public class ArithmeticianGame
    {
        /// <summary>
        /// The current question
        /// </summary>
        public int CurrentQuestion { get; set; }

        /// <summary>
        /// The time when the game was played
        /// </summary>
        public Stopwatch Stopwatch { get; set; }

        /// <summary>
        /// The timer which times how long the game when on
        /// </summary>
        public DispatcherTimer GameTimer { get; set; }

        /// <summary>
        /// The current user who is playing the game
        /// </summary>
        public User CurrentUser { get; set; }

        /// <summary>
        /// The game type of the current game.
        /// </summary>
        public GameType GameType { get; set; }

        /// <summary>
        /// Contains the final score of the game
        /// </summary>
        public Score GameScore { get; set; }

        /// <summary>
        /// The rng that the game uses
        /// </summary>
        private Random GameRandom;

        /// <summary>
        /// Constructor which can initialize the current user.
        /// </summary>
        public ArithmeticianGame(User currentUser, GameType gameType)
        {
            CurrentUser = currentUser;
            GameType = gameType;

            CurrentQuestion = 1;
            GameRandom = new Random();
            Stopwatch = Stopwatch.StartNew();

            GameScore = new Score
            {
                User = CurrentUser
            };

            GameTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
        }

        /// <summary>
        /// Generates a new question based
        /// </summary>
        /// <returns></returns>
        internal string GenerateQuestion()
        {
            switch (GameType)
            {
                case GameType.Addition:
                    return GenerateAdditionQuestion();
                case GameType.Subtraction:
                    return GenerateSubtractionQuestion();
                case GameType.Multiplication:
                    return GenerateMultiplicationQuestion();
                case GameType.Division:
                    return GenerateDivisionQuestion();
                default:
                    throw new InvalidOperationException("Current User is null or GameType is invalid");
            }
        }

        /// <summary>
        /// Generates a Division Question which isn't difficult 
        /// </summary>
        /// <returns></returns>
        private string GenerateDivisionQuestion()
        {
            float firstOperand = GameRandom.Next(0, 101);
            float secondOperand = GameRandom.Next(0, (int)firstOperand);
            float answer = firstOperand / secondOperand;

            // If the answer isn't a whole number then keep generating the question until it is.
            while (!((answer % 1) == 0))
            {
                firstOperand = GameRandom.Next(0, 101);
                secondOperand = GameRandom.Next(0, (int)firstOperand);
                answer = firstOperand / secondOperand;
            }

            return firstOperand + " / " + secondOperand;
        }

        /// <summary>
        /// Generates a Multiplication Question which isn't difficult 
        /// </summary>
        /// <returns></returns>
        private string GenerateMultiplicationQuestion()
        {
            return GameRandom.Next(0, 13) + " * " + GameRandom.Next(0, 13); // Each opperand is a number 0 through 12
        }

        /// <summary>
        /// Generates a Addition Question which isn't difficult 
        /// </summary>
        /// <returns></returns>
        private string GenerateAdditionQuestion()
        {
            return GameRandom.Next(0, 101) + " + " + GameRandom.Next(0, 101);
        }

        /// <summary>
        /// Generates a Subtraction Question which isn't difficult 
        /// </summary>
        /// <returns></returns>
        private string GenerateSubtractionQuestion()
        {
            int firstOperand = GameRandom.Next(0, 101);
            int secondOperand = GameRandom.Next(0, firstOperand); // The second operand should be smaller than the first so that there is no negative numbers
            return firstOperand + " - " + secondOperand;
        }

        /// <summary>
        /// Returns true if the answer gets the question right.
        /// </summary>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public bool CheckAnswer(string question, int answer)
        {
            string[] questionElements = question.Split(' ');
            int correctAnswer = 0;

            int.TryParse(questionElements[0], out int firstOperand);
            int.TryParse(questionElements[2], out int secondOperand);

            switch (questionElements[1])
            {
                case "+":
                    correctAnswer = firstOperand + secondOperand;
                    break;
                case "-":
                    correctAnswer = firstOperand - secondOperand;
                    break;
                case "*":
                    correctAnswer = firstOperand * secondOperand;
                    break;
                case "/":
                    correctAnswer = firstOperand / secondOperand;
                    break;
                default:
                    throw new InvalidOperationException("Invalid question or the CheckAnswer method has a bug.");
            }

            return answer == correctAnswer ? true : false;
        }
    }
}
