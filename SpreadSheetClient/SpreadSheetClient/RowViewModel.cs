using System.ComponentModel;

namespace SpreadSheetClient
{
    internal sealed class RowViewModel : INotifyPropertyChanged
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

        private int myNumber;
        public int Number
        {
            get
            {
                return myNumber;
            }
            set
            {
                if (value != myNumber)
                {
                    myNumber = value;
                    NotifyPropertyChanged("Number");
                }
            }
        }

        public RowViewModel Data { get { return this; } set { } }

        public void NotifyDataChanged()
        {
            NotifyPropertyChanged("Data");
            return;
        }
    }
}
