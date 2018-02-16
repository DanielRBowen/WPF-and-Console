/// By Daniel Bowen, October 2012
/// returns are used to show the end of a method.

using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace SpreadsheetGUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string AppName = "Spread Sheet"; // Displays at the top of the screen along with the file name.
		private const string SpreadsheetXmlVersion = "ps6"; // The current app version.
		private const int MinColumnCount = 26; // This is how many columns there are in the spreadsheet.
		private const int MinRowCount = 99; // This is how many rows there are in the spreadsheet.

		private SpreadsheetServiceClient mySpreadsheetServiceClient;

		private Spreadsheet mySpreadsheet;  // This is the spread sheet that contains the values and contents of every cell
		private readonly SortedSet<string> myValidCellNames = new SortedSet<string>(); // These are all the Cell names that contain a value in them (Not empty cells).
		private readonly ViewModel myViewModel;  // The view model which uses Component model and Collections.ObjectModel
		private string mySpreadsheetName; // The name of the spread sheet which is 'Untitled' until the user saves the spread sheet
		private string mySpreadsheetVersion;

		private DataGridColumn myCurrentColumn; // The current column that the user has selected.
		private RowViewModel myCurrentRow; // The current row that the user has selected.

		private RowViewModel myUpdateRow; // used to update which current row is selected.
		private readonly Timer myUpdateRowTimer; // the timer used to update the row.

		/// <summary>
		/// Updates the row.
		/// </summary>
		/// <param name="parameters"></param>
		private void UpdateRow(object parameters)
		{
			Dispatcher.BeginInvoke(new Action(() => myUpdateRow.NotifyDataChanged()));
			return;
		}

		/// <summary>
		/// Creates a row and is used in Create Cells
		/// </summary>
		/// <param name="rowNumber"></param>
		/// <returns></returns>
		private RowViewModel CreateRow(int rowNumber)
		{
			var row = new RowViewModel
			{
				Number = rowNumber
			};
			return row;
		}

		/// <summary>
		/// Creates cells in the Data Grid
		/// </summary>
		/// <param name="columnCount"></param>
		/// <param name="rowCount"></param>
		private void CreateCells(int columnCount = MinColumnCount, int rowCount = MinRowCount)
		{
			var columns = MainDataGrid.Columns;
			columns.Clear();

			// create the columns 
			for (var index = 0; index != columnCount; ++index)
			{
				var columnName = ((char)('A' + index)).ToString(); // This is the name of the column
				var column = new DataGridTextColumn
				{
					Header = columnName, // The name of the column is displayed at the top of the column
					Binding = new Binding("Data")
					{
						Converter = new CellConverter(), // The converter is custom logic to the data binding
						ConverterParameter = Tuple.Create(this, columnName)
					}
				};
				columns.Add(column);  // Add the column to the data grid 
			}

			// We need to load the row headers seperatly.
			var rows = myViewModel.Rows; // use the view model for the rows because a MainDataGrid only has columns
			rows.Clear();
			++rowCount;  // Add one to the rows so that the correct rows count are displayed

			// create the rows
			for (var rowNumber = 1; rowNumber != rowCount; ++rowNumber)
			{
				rows.Add(CreateRow(rowNumber)); // Add the row to the data grid.
			}

			return;
		}

		/// <summary>
		/// When the user click on 'New' in the menu then a new spread sheet will pop up.
		/// </summary>
		private void New()
		{
			myValidCellNames.Clear();  // clear the valid cell names

			// crate a new representation of a spread sheet
			var spreadsheet = new Spreadsheet(IsValidCellName, NormalizeCellName, SpreadsheetXmlVersion);
			mySpreadsheet = spreadsheet;

			SetSpreadsheetName(null, "0"); // There will not be a file path for the new spreadsheet
			CreateCells(); // Create cells for the new spread sheet

			return; // The end of this method.
		}

		/// <summary>
		/// Crates a NewWindow when New is executed.
		/// </summary>
		private void NewWindow()
		{
			new SessionWindow().Show();
			return;
		}

		/// <summary>
		/// This highlights what cell(s) are selected.
		/// </summary>
		private void SetFocusOnCurrentCell()
		{
			var mainDataGrid = MainDataGrid;
			var currentRow = myCurrentRow;
			var currentCell = new DataGridCellInfo(currentRow, myCurrentColumn);

			mainDataGrid.Focus();
			mainDataGrid.CurrentCell = currentCell;
			mainDataGrid.ScrollIntoView(currentRow);

			return;
		}

		private async void SpreadsheetServiceClient_CellUpdated(object sender, CellUpdatedEventArgs e)
		{
			await SetCellContents(e.Cell, e.Content, e.Version);
			return;
		}

		/// <summary>
		/// initializes the MainWindow
		/// </summary>
		internal MainWindow(SpreadsheetServiceClient spreadsheetServiceClient, JoinSessionResponse joinSessionResponse)
		{
			mySpreadsheetServiceClient = spreadsheetServiceClient;
			spreadsheetServiceClient.CellUpdated += SpreadsheetServiceClient_CellUpdated;

			InitializeComponent();

			myUpdateRowTimer = new Timer(UpdateRow);

			var viewModel = new ViewModel(this);
			myViewModel = viewModel;

			New(); // Create a new spread sheet with the data grid cleared and the name set to 'Untitled'.

			DataContext = viewModel;

			myCurrentColumn = MainDataGrid.Columns[0];
			myCurrentRow = viewModel.Rows[0];
			SetFocusOnCurrentCell();

			Dispatcher.BeginInvoke(new Action<JoinSessionResponse>(joinSessionResponse0 => Load(joinSessionResponse0)), joinSessionResponse);
		}

		/// <summary>
		/// Returns true if the cell name is valid.
		/// </summary>
		/// <param name="cellName"></param>
		/// <returns></returns>
		private bool IsValidCellName(string cellName)
		{
			return true;
		}

		/// <summary>
		/// Cell names cannot have null or white spaces.
		/// Also, all cell names will be capitalized.
		/// </summary>
		/// <param name="cellName"></param>
		/// <returns></returns>
		private string NormalizeCellName(string cellName)
		{
			if (string.IsNullOrWhiteSpace(cellName))
			{
				return null;
			}

			return cellName.ToUpperInvariant();
		}

		/// <summary>
		/// This is used to set the file path when the spread sheet is saved.
		/// </summary>
		private void SetSpreadsheetName(string spreadsheetName, string spreadsheetVersion)
		{
			if (string.IsNullOrWhiteSpace(spreadsheetName))
			{
				spreadsheetName = "Untitled";
			}

			mySpreadsheetName = spreadsheetName;
			mySpreadsheetVersion = spreadsheetVersion;

			Title = spreadsheetName + " – " + AppName; // puts the name of the file with " - Spread Sheet" at the top of the window.

			return;
		}

		/// <summary>
		/// Gets the cell numbers associated to the cell Name (The position on the grid)
		/// Is used in when opening a spread sheet and seting the cell contents of a spread sheet
		/// </summary>
		/// <param name="cellName"></param>
		/// <returns></returns>
		private static Tuple<int, int> GetCellNumbers(string cellName)
		{
			var index = 0; // We need to use this index, we find it with the loop

			for (var count = cellName.Length; index != count; ++index)
			{
				if (char.IsDigit(cellName, index))
				{
					break;
				}
			}

			if (index == 0)
			{
				return Tuple.Create(1, 1);
			}

			var columnName = cellName.Substring(0, index);
			var rowName = cellName.Substring(index);
			int columnNumber, rowNumber;

			var character = columnName[0];
			columnNumber = (int)character - 'A' + 1;

			if (!int.TryParse(rowName, out rowNumber))
			{
				rowNumber = 1;
			}

			return Tuple.Create(columnNumber, rowNumber);
		}

		/// <summary>
		/// This method opens up a saved spread sheet from the specified file path
		/// </summary>
		private bool Load(JoinSessionResponse joinSessionResponse)
		{
			Spreadsheet spreadsheet;

			// Use the Spreadsheet constructor that will create a spreadsheet with all the data the .ss file contains.
			try
			{
				var spreadsheetElement = XElement.Parse(joinSessionResponse.Xml);
                spreadsheet = new Spreadsheet();
				//spreadsheet = new Spreadsheet(spreadsheetElement, IsValidCellName, NormalizeCellName, SpreadsheetXmlVersion);
			}
			catch (Exception ex)
			{
				ShowErrorMessage("Unable to open:\r\n" + ex.Message);
				return false;
			}

			mySpreadsheet = spreadsheet;
			SetSpreadsheetName(joinSessionResponse.Name, joinSessionResponse.Version);

			CreateCells();

			// You need the names of all empty cells in order to display the non empty cells onto the data grid.
			var namesOfAllNonemptyCells = spreadsheet.GetNamesOfAllNonemptyCells();
			var rows = MainDataGrid.Items;  // The items in the MainDataGrid
			var rowCount = rows.Count;
			var validCellNames = myValidCellNames;

			foreach (var cellName in namesOfAllNonemptyCells)
			{
				validCellNames.Add(cellName);

				var cellNumbers = GetCellNumbers(cellName); // Get the position of the cell
				var rowIndex = cellNumbers.Item2 - 1; // The row index

				// Add the data from the spreadsheet to the MainDataGrid UI
				while (rowCount <= rowIndex)
				{
					rows.Add(CreateRow(rowCount++));
				}

				var row = (RowViewModel)rows[rowIndex];
				row.NotifyDataChanged();
			}

			// If no exceptions occured then tell that the file has been opened
			return true;
		}

		/// <summary>
		/// This function will be called when the user clicks 'Save' from the menu.
		/// It will call Save As if no there is no file path.
		/// </summary>
		/// <returns></returns>
		private async Task<bool> Save()
		{
			string errorMessage;

			try
			{
				var response = await mySpreadsheetServiceClient.Save(mySpreadsheetName);
				errorMessage = response.ErrorMessage;

				if (errorMessage == null)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			ShowErrorMessage("Unable to save:\r\n" + errorMessage);

			return false;
		}

		/// <summary>
		/// Allows the user to save the spread sheet if the spread sheet was changed but not saved.        /// 
		/// </summary>
		/// <returns></returns>
		private async Task<bool> SaveIfNeeded()
		{
			// show a message box if the spread sheet was changed and allow the user to save.
			if (mySpreadsheet.Changed)
			{
				var result = MessageBox.Show(this, "Do you want to save changes to " + mySpreadsheetName + "?", Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel, MessageBoxOptions.None);

				switch (result)
				{
					case MessageBoxResult.Yes:
						return await Save();
					case MessageBoxResult.No:
						break;
					default:
						return false;
				}
			}
			return true;
		}

		private void ShowErrorMessage(string errorMessage)
		{
			MessageBox.Show(this, errorMessage, Title, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.None);
			return;
		}

		private async Task<bool> Undo()
		{
			var response = await mySpreadsheetServiceClient.UndoLastChange(mySpreadsheetName, mySpreadsheetVersion);
			var errorMessage = response.ErrorMessage;

			if (response.VersionOutOfDate)
			{
				errorMessage = "the spreadsheet is out of date.";
			}

			if (errorMessage != null)
			{
				ShowErrorMessage("Unable to undo:\r\n" + errorMessage);
				return false;
			}

			if (response.NoUnsavedChanges)
			{
				return false;
			}

			return await SetCellContents(response.Cell, response.Content, response.Version);
		}

		/// <summary>
		/// Opens up the help window when you click on help.
		/// </summary>
		private void Help()
		{
			new HelpWindow
			{
				Owner = this
			}.ShowDialog();  // use Show Dialog to return to the Main Window when the help window closes.
			return;
		}

		/// <summary>
		/// Will call SaveIfNeeded when closing the window.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Window_Closing(object sender, CancelEventArgs e)
		{
			var ok = await SaveIfNeeded();

			if (!ok)
			{
				e.Cancel = true;
			}
			else
			{
				var spreadsheetServiceClient = mySpreadsheetServiceClient;

				if (spreadsheetServiceClient != null)
				{
					spreadsheetServiceClient.CellUpdated -= SpreadsheetServiceClient_CellUpdated;
					mySpreadsheetServiceClient = null;

					await spreadsheetServiceClient.LeaveSession(mySpreadsheetName);

					spreadsheetServiceClient.Dispose();
				}
			}

			return;
		}

		/// <summary>
		/// This makes the UI element so that it can always execute.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Always_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = true;
			return;
		}

		/// <summary>
		/// The event is handled and a new window will be opened
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void New_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			NewWindow();
			return;
		}

		/// <summary>
		/// When the spreadsheet is changed then the user can use save from the menu.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = mySpreadsheet.Changed;
			return;
		}

		/// <summary>
		/// Run the save method
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Save_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			await Save();
			return;
		}

		/// <summary>
		/// Run the close method and close the darn window.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			Close();
			return;
		}

		private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = mySpreadsheet.Changed && mySpreadsheetVersion != "0";
			return;
		}

		private async void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			await Undo();
			return;
		}

		/// <summary>
		/// Run the help method to open up the help window.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Help_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			Help();
			return;
		}

		/// <summary>
		/// Load the row headers to the MainDataGrid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
		{
			var row = e.Row;
			var rowViewModel = row.Item as RowViewModel; //set the data for each row

			if (rowViewModel != null)
			{
				row.Header = rowViewModel.Number;
			}
			else
			{
				row.Header = (row.GetIndex() + 1).ToString(NumberFormatInfo.InvariantInfo);
			}

			return;
		}

		/// <summary>
		/// Gets the name of a cell
		/// </summary>
		/// <param name="column"></param>
		/// <param name="row"></param>
		/// <returns></returns>
		private static string GetCellName(DataGridColumn column, RowViewModel row)
		{
			return (string)column.Header + row.Number.ToString(NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Change the current cell in the MainDataGrid UI
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainDataGrid_CurrentCellChanged(object sender, EventArgs e)
		{
			var currentCell = MainDataGrid.CurrentCell;
			var column = currentCell.Column;

			if (column == null)
			{
				return;
			}

			var row = (RowViewModel)currentCell.Item;

			myCurrentColumn = column;
			myCurrentRow = row;
			myViewModel.CurrentCellName = GetCellName(column, row);

			return;
		}

		/// <summary>
		/// Sets the Cell contents and updates the MainDataGrid UI
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void MainDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			var row = (RowViewModel)e.Row.Item;  // When you edit a cell this with be the row that will be updated

			if (e.EditAction == DataGridEditAction.Commit)
			{
				var cellName = GetCellName(e.Column, row);
				await SetCellContents(cellName, ((TextBox)e.EditingElement).Text);
			}

			myUpdateRow = row;
			myUpdateRowTimer.Change(25, Timeout.Infinite);

			return;
		}

		/// <summary>
		/// Gets the cell value and returns that value as a string to be displayed in the UI.
		/// </summary>
		/// <param name="cellName"></param>
		/// <returns></returns>
		internal string GetCellValue(string cellName)
		{
			if (string.IsNullOrWhiteSpace(cellName) || !myValidCellNames.Contains(cellName))
			{
				return null;
			}
			return mySpreadsheet.GetCellValue(cellName).ToString();
		}

		/// <summary>
		/// Gets the cell contents and return the contents as a string to be displayed in the UI.
		/// </summary>
		/// <param name="cellName"></param>
		/// <returns></returns>
		internal string GetCellContents(string cellName)
		{
			if (string.IsNullOrWhiteSpace(cellName) || !myValidCellNames.Contains(cellName))
			{
				return null;
			}
			return mySpreadsheet.GetCellContents(cellName).ToString();
		}

		/// <summary>
		/// Sets the cell contents in the MainDataGrid
		/// </summary>
		/// <param name="cellName"></param>
		/// <param name="cellContents"></param>
		/// <returns></returns>
		internal async Task<bool> SetCellContents(string cellName, string cellContents, string spreadsheetVersion = null)
		{
			if (string.IsNullOrWhiteSpace(cellName))
			{
				return false;
			}

			var spreadsheet = mySpreadsheet;
			string originalContents = null;
			ISet<string> changedCellNames;

			try
			{
				originalContents = spreadsheet.GetCellContents(cellName).ToString();
			}
			catch
			{
			}

			try
			{
				changedCellNames = spreadsheet.SetContentsOfCell(cellName, cellContents);
			}
			catch (CircularException)
			{
				ShowErrorMessage("a cell cannot reference itself.");
				return false;
			}
			catch (Exception ex)
			{
				ShowErrorMessage(ex.Message);
				return false;
			}

			if (spreadsheetVersion == null)
			{
				var response = await mySpreadsheetServiceClient.ChangeCell(mySpreadsheetName, mySpreadsheetVersion, cellName, cellContents);
				var errorMessage = response.ErrorMessage;

				if (response.VersionOutOfDate)
				{
					errorMessage = "the spreadsheet is out of date.";
				}

				if (errorMessage != null)
				{
					if (originalContents != null)
					{
						spreadsheet.SetContentsOfCell(cellName, originalContents);
					}

					ShowErrorMessage(errorMessage);

					return false;
				}

				mySpreadsheetVersion = response.Version;
			}
			else
			{
				mySpreadsheetVersion = spreadsheetVersion;
			}

			var validCellNames = myValidCellNames;

			if (!validCellNames.Contains(cellName))
			{
				validCellNames.Add(cellName);
			}

			var changedRowNumbers = new SortedSet<int>();

			foreach (var changedCellName in changedCellNames)
			{
				var rowNumber = GetCellNumbers(changedCellName).Item2;

				if (!changedRowNumbers.Contains(rowNumber))
				{
					changedRowNumbers.Add(rowNumber);
				}
			}

			foreach (var row in myViewModel.Rows)
			{
				if (changedRowNumbers.Contains(row.Number))
				{
					row.NotifyDataChanged();
				}
			}

			return true;
		}

		/// <summary>
		/// Highlights and selects the cell below the current selected cell.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CurrentCellContentsTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SetFocusOnCurrentCell();
			}
			return;
		}
	}
}
