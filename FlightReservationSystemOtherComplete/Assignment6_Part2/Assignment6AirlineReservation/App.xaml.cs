using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Assignment6AirlineReservation
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
        /// Allows access to get and update data from the Airline Database
        /// </summary>
        internal static AirlineRepository AirlineRepository = new AirlineRepository();
    }
}
