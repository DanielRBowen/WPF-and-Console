using System;
using System.ComponentModel;
using System.Windows;

namespace Flight_Reservation_System
{
    /// <summary>
    /// Interaction logic for AddPassengerWindow.xaml
    /// </summary>
    public partial class AddPassengerWindow : Window
    {
        /// <summary>
        /// A Passenger Window
        /// </summary>
        public AddPassengerWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
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
        /// Handles the closing event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                base.OnClosing(e);
                new MainWindow().Show();
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Handles the click on the cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // It should also have a “Save” and “Cancel” button on it.  For the first part of this assignment both of these buttons will do nothing but close this form.
                Close();
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Handles the click on the cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // It should also have a “Save” and “Cancel” button on it.  For the first part of this assignment both of these buttons will do nothing but close this form.
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
