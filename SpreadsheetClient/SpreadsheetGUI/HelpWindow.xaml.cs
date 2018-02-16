using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpreadsheetGUI
{
	/// <summary>
	/// Interaction logic for HelpWindow.xaml
	/// </summary>
	public partial class HelpWindow : Window
	{
		/// <summary>
		/// Initializes the HelpWindow
        /// This window only contains a readonly textbox that
        /// tells the user how to use the application.
		/// </summary>
		public HelpWindow()
		{
			InitializeComponent();
			HelpTextBox.Text = @"To use this application, click in a cell and type a string, a number, 
or an equation that begins with a ""="" sign.
You can perform operations on cells that have formulas or
numbers by clicking on a cell and typing an equation that 
uses the Cell name of the value you want to use.
To save a spread sheet, click on 'file' then click 'Save'.
To create a new spread sheet, click on 'file' then 'New'.
To open a new spread sheet, click on 'file' then 'Open'.
To close the spread sheet, click on 'file' then 'Close'
To save the spread sheet as a different file, click 'File'
Then click on 'Save As'.";
		}

        /// <summary>
        /// This Ok button just closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
			return;
		}
	}
}
