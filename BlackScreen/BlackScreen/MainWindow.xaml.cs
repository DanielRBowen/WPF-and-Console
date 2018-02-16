using System;
using System.Windows;
using System.Windows.Input;

namespace BlackScreen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        Close();
                        break;
                    }
            }
            return;
        }

        /// <summary>
        /// Exits the program.
        /// </summary>
        public static void Exit()
        {
            Console.ReadKey();
            Environment.Exit(1);
        }

        public void ShowMessageBox()
        {
            MessageBox.Show("Black Screen");
        }
    }
}
