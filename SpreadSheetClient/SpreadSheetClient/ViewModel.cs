using System.Collections.ObjectModel;
/// See http://en.wikipedia.org/wiki/View_model
/// This view model helps with controling the rows of the MainDataGrid.
using System.ComponentModel;
using System.Threading.Tasks;

namespace SpreadSheetClient
{
    internal sealed class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;

            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private readonly MainWindow myMainWindow; // The main window will contain this view Model

        /// <summary>
        /// Send a main window to the view model to initialize.
        /// </summary>
        /// <param name="mainWindow"></param>
		public ViewModel(MainWindow mainWindow)
        {
            myMainWindow = mainWindow;
        }

        private string myCurrentCellName;

        /// <summary>
        /// The current cell name
        /// </summary>
		public string CurrentCellName
        {
            get
            {
                return myCurrentCellName;
            }
            set
            {
                if (value != myCurrentCellName)
                {
                    myCurrentCellName = value;
                    NotifyPropertyChanged("CurrentCellName");
                    NotifyPropertyChanged("CurrentCellValue");
                    NotifyPropertyChanged("CurrentCellContents");
                }
            }
        }

        /// <summary>
        /// The Current Cell Value in the main window
        /// </summary>
		public string CurrentCellValue
        {
            get
            {
                return myMainWindow.GetCellValue(myCurrentCellName);
            }
        }

        /// <summary>
        /// The Current Cell contents in the main window
        /// </summary>
		public string CurrentCellContents
        {
            get
            {
                return myMainWindow.GetCellContents(myCurrentCellName);
            }
            set
            {
                SetCurrentCellContents(value);
            }
        }

        public async Task SetCurrentCellContents(string value)
        {
            if (value != CurrentCellContents)
            {
                var ok = await myMainWindow.SetCellContents(myCurrentCellName, value);

                if (ok)
                {
                    NotifyPropertyChanged("CurrentCellValue");
                    NotifyPropertyChanged("CurrentCellContents");
                }
            }
            return;
        }

        /// <summary>
        /// Used for the rows when items get added, changed, removed or the whole row get refreshed 
        /// </summary>
		private readonly ObservableCollection<RowViewModel> myRows = new ObservableCollection<RowViewModel>();
        public ObservableCollection<RowViewModel> Rows
        {
            get
            {
                return myRows;
            }
        }
    }
}
