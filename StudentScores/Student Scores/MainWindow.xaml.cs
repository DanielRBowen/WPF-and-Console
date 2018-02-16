/*
 * Daniel Bowen
 * January 2017
 */

using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Student_Scores
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		private StudentScores studentScores;

		/// <summary>
		/// The Default Constructor of the Main Window of the Application
		/// </summary>
		public MainWindow()
		{
            try
            {
                InitializeComponent();
                ResetScores();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// Handles Error messages
        /// All methods should handle exceptions.  
        /// </summary>
        /// <param name="sClass"></param>
        /// <param name="sMethod"></param>
        /// <param name="exception"></param>
        private void HandleError(string sClass, string sMethod, Exception exception)
        {
            try
            {
                MessageBox.Show(sClass + "." + sMethod + " -> " + exception.Message + "---- " + exception.StackTrace);
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\Error.txt", Environment.NewLine + "HandleError Exception: " + ex.Message);
            }
        }

        /// <summary>
        /// Resets the scores by setting it to null and then updates the UI
        /// </summary>
        private void ResetScores()
		{
            try
            {
                studentScores = null;

                DataContext = studentScores;
                UpdateUI();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

		/// <summary>
		/// Creates the Students and Assignments arrays when the submit counts button is clicked
		/// puts out error labels for invalid values
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSubmitCounts_Click(object sender, RoutedEventArgs e)
		{
            try
            {
                // Holds the length of the Students and Assignment arrays
                int numStudents = 0;
                int numAssignments = 0;

                // Get the values from the UI and print out error label if user enters unwanted values
                if
                (
                    ToggleErrorLabel(!int.TryParse(boxNumStudents.Text, out numStudents), "Couldn't Parse Number of Students to an integer.") ||
                    ToggleErrorLabel(!int.TryParse(boxNumAssignments.Text, out numAssignments), "Couldn't Parse Number of Assignments to an integer.") ||

                    ToggleErrorLabel(numStudents > 10 || numStudents < 1, "Number of students must be greater than 0 and less than 10.") ||
                    ToggleErrorLabel(numAssignments > 99 || numAssignments < 1, "Number of assignments must be greater than 0 and less than 99.")
                )
                {
                    return;
                }

                // Create the new student scores model with the inputed values
                studentScores = new StudentScores(numStudents, numAssignments);

                // Default the students and assignments
                for (int index = 0; index < numStudents; index++)
                {
                    studentScores.Students[index] = "Student #" + (index + 1);
                }

                for (int index = 0; index < studentScores.Assignments.GetLength(0); index++)
                {
                    for (int index2 = 0; index2 < studentScores.Assignments.GetLength(1); index2++)
                    {
                        studentScores.Assignments[index, index2] = 0;
                    }
                }

                UpdateUI();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

		/// <summary>
		/// Update all the xaml for the UI
		/// Updates it to the default values if null or,
		/// the current student lbl when going through each students information
		/// </summary>
		private void UpdateUI()
		{
            try
            {
                if (studentScores == null)
                {
                    lblStudentInfo.Content = "";
                    boxNumStudents.Text = "";
                    boxNumAssignments.Text = "";
                    boxANumber.Text = "";
                    boxAScore.Text = "";
                    boxScores.Text = "";
                    lblEnterANumber.Content = "Enter Assignment Number: ";
                }
                else
                {
                    lblEnterANumber.Content = "Enter Assignment Number (1-" + studentScores.Assignments.GetLength(1) + "):";
                    lblStudentInfo.Content = studentScores.Students[studentScores.CurrentStudent];
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

		/// <summary>
		/// This toggles the error label with the desired content
		/// The error lable is visible when the expression is true
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="lblContent"></param>
		/// <returns></returns>
		private bool ToggleErrorLabel(bool expression, string lblContent)
		{
            try
            {
                if (expression)
                {
                    lblError.Content = lblContent;
                    lblError.Visibility = Visibility.Visible;
                    return true;
                }
                else
                {
                    lblError.Visibility = Visibility.Hidden;
                    return false;
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);

                return false;
            }
        }

		/// <summary>
		/// Resets the scores when the Reset Scores button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnResetScores_Click(object sender, RoutedEventArgs e)
		{
            try
            {
                ResetScores();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

		/// <summary>
		/// When the studentScores exists,
		/// updates the current student to be the first student in the array
		/// and Updates the UI to reflect who the current student is
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnFirstStudent_Click(object sender, RoutedEventArgs e)
		{
            try
            {
                if (studentScores == null)
                {
                    return;
                }

                studentScores.CurrentStudent = 0;
                UpdateUI();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

		/// <summary>
		/// When the studentScores exists,
		/// updates the current student to be the previous student in the array
		/// If it is the first student it changes to the first student
		/// and Updates the UI to reflect who the current student is
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnPrevStudent_Click(object sender, RoutedEventArgs e)
		{
            try
            {
                if (studentScores == null)
                {
                    return;
                }

                if (studentScores.CurrentStudent == 0)
                {
                    studentScores.CurrentStudent = studentScores.Students.Length - 1;
                    UpdateUI();
                }
                else
                {
                    studentScores.CurrentStudent--;
                    UpdateUI();
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

		/// <summary>
		/// When the studentScores exists,
		/// updates the current student to be the next student in the array
		/// If it is the last student it changes to the first student
		/// and Updates the UI to reflect who the current student is.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnNextStudent_Click(object sender, RoutedEventArgs e)
		{
            try
            {
                if (studentScores == null)
                {
                    return;
                }

                if (studentScores.CurrentStudent == studentScores.Students.Length - 1)
                {
                    studentScores.CurrentStudent = 0;
                    UpdateUI();
                }
                else
                {
                    studentScores.CurrentStudent++;
                    UpdateUI();
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

		/// <summary>
		/// When the studentScores exists,
		/// updates the current student to be the last student in the array
		/// and Updates the UI to reflect who the current student is
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnLastStudent_Click(object sender, RoutedEventArgs e)
		{
            try
            {
                if (studentScores == null)
                {
                    return;
                }

                studentScores.CurrentStudent = studentScores.Students.Length - 1;
                UpdateUI();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

		/// <summary>
		/// Saves the name of what displayed on the lblStudentInfo to what is in the boxStudentInfo when
		/// the Save Name button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSaveName_Click(object sender, RoutedEventArgs e)
		{
            try
            {
                if (studentScores == null)
                {
                    return;
                }

                studentScores.Students[studentScores.CurrentStudent] = boxStudentInfo.Text.ToString();

                lblStudentInfo.Content = boxStudentInfo.Text.ToString();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

		/// <summary>
		/// Saves the scores of the current student selected as displayed in the lblStudentInfo
		/// boxANumber will be the Assignment number and boxAScore will be the score for the assignment
		/// Will toggle an error message for undesirable values.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSaveScore_Click(object sender, RoutedEventArgs e)
		{
            try
            {
                if (studentScores == null)
                {
                    return;
                }

                float score;
                int assignmentNumber;

                if
                (
                    ToggleErrorLabel(!float.TryParse(boxAScore.Text.ToString(), out score), "Couldn't Parse Score to a float.") ||
                    ToggleErrorLabel(!int.TryParse(boxANumber.Text.ToString(), out assignmentNumber), "Couldn't Parse Assignment Number to an integer.") ||
                    ToggleErrorLabel(assignmentNumber < 1 || assignmentNumber > studentScores.Assignments.GetLength(1), "Assignment number has to be within 1 and " + studentScores.Assignments.GetLength(1) + ".") ||
                    ToggleErrorLabel(score < 0 || score > 100, "Score has to be greater or equal to 0 and less than or equal to 100.")
                )
                {
                    return;
                }

                studentScores.Assignments[studentScores.CurrentStudent, assignmentNumber - 1] = score;
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

		/// <summary>
		/// Displays the Scores in the grid of all the students hwn btnDisplayScores is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnDisplayScores_Click(object sender, RoutedEventArgs e)
		{
            try
            {
                if (studentScores == null)
                {
                    return;
                }

                studentScores.RefreshScores();

                PrintScoresToTextBox();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

		/// <summary>
		/// Prints the scores to the boxScores text box.
		/// </summary>
		private void PrintScoresToTextBox()
		{
            try
            {
                string scoreslist = InputOutput.WriteScoresString(studentScores.ScoreItems);

                boxScores.Text = scoreslist;
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// When the “Output to file” button is pressed, instead of displaying the student’s data to the textbox, 
        /// it will be outputted to a file on a new thread.  The exact same text that is displayed in the student’s data textbox, should be saved to the file.  
        /// Since a new thread is being used the UI should not lock up when the button is pressed.
        /// 
        /// The file name should be entered by the user in a textbox next to “Output to file” button.  
        /// Make sure to check that the file does not already exist.  
        /// You may default the directory location where the file is written to, to "C:\Users\Public" just to make things easier.
        /// 
        /// When the user presses the button, the button should be disabled, and the text next to the button should say “Writing to file”.  
        /// Next the background thread (not the UI) should write the data to a file.  
        /// Since writing to the file will be fast, we need to simulate a process that will take a while.  
        /// We will do this by putting the thread to sleep for 2 seconds.  After the thread wakes up update the text next to the button to “Finished writing to file, and enable the button.
        /// 
        /// Make sure the work of writing the data to the file is done on the background thread and not the UI thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OutputToFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (studentScores == null)
                {
                    return;
                }

                WritingToFileLabel.Content = "Writing to file";
                OutputToFileButton.IsEnabled = false;
                
                var filePath = FileNameTextBox.Text;

                if (File.Exists(filePath))
                {
                    WritingToFileLabel.Content = "File already exists";
                    return;
                }

                await InputOutput.SaveScoresAsync(filePath, studentScores.ScoreItems);

                WritingToFileLabel.Content = "Finished writing to file";
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
            finally
            {
                OutputToFileButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Modify this program so that a file can be loaded which contains the student and assignment data.  
        /// The data should be loaded from the file back into the arrays used to store the student’s name and grades.  
        /// The format of the file is up to you.  A suggestion would be to use an XML file.  
        /// The assignments must be loaded on a separate thread from the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoadScoresFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                studentScores = await InputOutput.LoadStudentScoresAsync(LoadFileNameTextBox.Text);

                if (studentScores == null)
                {
                    return;
                }

                DataContext = studentScores;
                UpdateUI();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// Saves the scores a JSON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveJsonScoresFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await InputOutput.SaveStudentScoresAsync(LoadFileNameTextBox.Text, studentScores);
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }
    }
}
