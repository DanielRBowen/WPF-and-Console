/// The custom binding for the cell
/// Makes the MainDataGrid UI element work with the cells.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SpreadSheetClient
{
    [ValueConversion(typeof(RowViewModel), typeof(string))]
    internal class CellConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rowViewModel = value as RowViewModel;

            if (rowViewModel != null)
            {
                var parameterTuple = (Tuple<MainWindow, string>)parameter;
                var cellName = parameterTuple.Item2 + rowViewModel.Number.ToString(NumberFormatInfo.InvariantInfo);

                try
                {
                    return parameterTuple.Item1.GetCellValue(cellName);
                }
                catch
                {
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
