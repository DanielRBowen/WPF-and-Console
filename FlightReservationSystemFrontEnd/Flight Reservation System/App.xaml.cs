using System.Windows;
using System.Windows.Controls;

namespace Flight_Reservation_System
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The Selected Flight from the main window saved to keep data among windows.
        /// </summary>
        internal static Flight SelectedFlight { get; set; }

        /// <summary>
        /// The Selected Passenger from the main window saved to keep data among windows.
        /// </summary>
        internal static Passenger SelectedPassenger { get; set; }

        /// <summary>
        /// The Previous selected button from the main window saved to keep data among windows.
        /// </summary>
        internal static Button PreviousSelectedButton { get; set; }
    }
}
