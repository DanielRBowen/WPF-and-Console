using System;
using System.Windows;
using System.Windows.Controls;

namespace SpreadSheetClient
{
    /// <summary>
    /// Interaction logic for SessionWindow.xaml
    /// </summary>
    public partial class SessionWindow : Window
    {
        private sealed class Parameters
        {
            public string SpreadsheetName { get; set; }

            public string Password { get; set; }

            public string Server { get; set; }

            public Parameters()
            {
                Server = "localhost";
            }
        }

        private readonly Parameters myParameters;

        public SessionWindow()
        {
            InitializeComponent();

            var parameters = new Parameters();
            myParameters = parameters;
            DataContext = parameters;

            ServerTextBox.Focus();
        }

        private void EnableUI(bool enabled)
        {
            ServerTextBox.IsEnabled = enabled;
            SpreadsheetNameTextBox.IsEnabled = enabled;
            MainPasswordBox.IsEnabled = enabled;
            CreateButton.IsEnabled = enabled;
            JoinButton.IsEnabled = enabled;
            return;
        }

        private bool IsValid(TextBox textBox, string propertyDisplayName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show(propertyDisplayName + " can't be empty.", Title);
                textBox.Focus();
                textBox.SelectAll();
                return false;
            }
            return true;
        }

        private bool IsValid(PasswordBox passwordBox, string propertyDisplayName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show(propertyDisplayName + " can't be empty.", Title);
                passwordBox.Focus();
                passwordBox.SelectAll();
                return false;
            }
            return true;
        }

        private bool IsValid(Parameters parameters)
        {
            if (!IsValid(ServerTextBox, "Server", parameters.Server))
            {
                return false;
            }

            if (!IsValid(SpreadsheetNameTextBox, "Spreadsheet Name", parameters.SpreadsheetName))
            {
                return false;
            }

            if (!IsValid(MainPasswordBox, "Password", parameters.Password))
            {
                return false;
            }

            return true;
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var parameters = myParameters;
            parameters.Password = MainPasswordBox.Password;

            if (!IsValid(parameters))
            {
                return;
            }

            EnableUI(false);

            CreateSessionResponse response;
            string errorMessage;

            try
            {
                using (var spreadsheetServiceClient = new SpreadsheetServiceClient())
                {
                    await spreadsheetServiceClient.Connect(parameters.Server);
                    response = await spreadsheetServiceClient.CreateSession(parameters.SpreadsheetName, parameters.Password);
                }
                errorMessage = response.ErrorMessage;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            if (errorMessage == null)
            {
                MessageBox.Show(this, "Session created.", Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(this, "Unable to create a new session:\r\n" + errorMessage, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            EnableUI(true);

            return;
        }

        private async void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            var parameters = myParameters;
            parameters.Password = MainPasswordBox.Password;

            if (!IsValid(parameters))
            {
                return;
            }

            EnableUI(false);

            SpreadsheetServiceClient spreadsheetServiceClient = null;
            JoinSessionResponse response = null;
            string errorMessage;

            try
            {
                spreadsheetServiceClient = new SpreadsheetServiceClient();
                await spreadsheetServiceClient.Connect(parameters.Server);
                response = await spreadsheetServiceClient.JoinSession(parameters.SpreadsheetName, parameters.Password);
                errorMessage = response.ErrorMessage;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            if (errorMessage == null)
            {
                new MainWindow(spreadsheetServiceClient, response).Show();
                Close();
            }
            else
            {
                spreadsheetServiceClient.Dispose();
                MessageBox.Show(this, "Unable to join the session:\r\n" + errorMessage, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                EnableUI(true);
            }

            return;
        }
    }
}
