using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace The_Arithmetician
{
    /// <summary>
    /// Interaction logic for wndEnterUserData.xaml
    /// </summary>
    public partial class EnterUserData : Window
    {
        private User User { get; set; }

        /// <summary>
		/// The Initialization of the EnterInfo Window
		/// </summary>
        public EnterUserData()
        {
            InitializeComponent();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                boxName.Focus();
                boxName.SelectAll();
            }));
        }

        /// <summary>
        /// Constructor for the screen gets the user data passed to it if there already is one.
        /// </summary>
        /// <param name="user"></param>
        public EnterUserData(User user) :
            this()
        {
            if (user != null)
            {
                User = user;
                boxName.Text = user.Name;
                boxAge.Text = user.Age.ToString(NumberFormatInfo.InvariantInfo);
            }
        }

        /// <summary>
        /// Shows an error message of the exception. Used to catch event errors.
        /// </summary>
        /// <param name="ex"></param>
        private void ShowErrorMessage(Exception ex)
        {
            MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows an Error Message with a specified message and should focus the text box that had undesirable input
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="errorMessage"></param>
        private void ShowErrorMessage(TextBox textBox, string errorMessage)
        {
            MessageBox.Show(this, errorMessage, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);

            textBox.Focus();
            textBox.SelectAll();

            return;
        }

        /// <summary>
        /// Handles the closing event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                base.OnClosing(e);
                new MainWindow(User).Show();
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
		/// Checks if the user data is valid and creates the user with the information if it is.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = boxName.Text;

                if (!Regex.IsMatch(name, @"(^[a-zA-Z]+$)"))
                {
                    ShowErrorMessage(boxName, "Name has to be characters within a-z, A-Z and one word.");
                    return;
                }

                if (!int.TryParse(boxAge.Text, out int age))
                {
                    ShowErrorMessage(boxAge, "Age has to be a number.");
                    return;
                }

                if (age < 5)
                {
                    ShowErrorMessage(boxAge, "Your age has to be 5 or older to play this game.");
                    return;
                }

                User = new User
                {
                    Name = name,
                    Age = age
                };

                Close();

                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Closes the window when the cancel button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }
    }
}