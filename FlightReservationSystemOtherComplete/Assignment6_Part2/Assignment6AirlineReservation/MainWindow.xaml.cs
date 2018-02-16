using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Assignment6AirlineReservation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// So the app can change functionality to choose a passenger
        /// </summary>
        private bool choosingPassenger = false;

        /// <summary>
        /// So that other functionality doesn't interfere while refreshing.
        /// </summary>
        private bool refreshingControls;

        /// <summary>
        /// A list of all the seat buttons
        /// </summary>
        private IList<Button> SeatButtons { get; set; }

        /// <summary>
        /// The Main window of the application.
        /// </summary>
        public MainWindow()
        {
            try
            {
                InitializeComponent();

                // Fill and bind the Choose flight combo box
                cbChooseFlight.ItemsSource = App.AirlineRepository.FlightList;
                cbChooseFlight.SelectedItem = App.AirlineRepository.FlightList.FirstOrDefault(flight => flight.FlightID == App.SelectedFlight?.FlightID);
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        
        /// <summary>
        /// This constructor is called when the user wants to select a seat.
        /// </summary>
        /// <param name="selectSeat"></param>
        public MainWindow(bool selectSeat) :
            this()
        {
            try
            {
                if (selectSeat)
                {
                    SelectSeat();
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// Allows the user to select a seat.
        /// </summary>
        private void SelectSeat()
        {
            try
            {
                choosingPassenger = true;
                RefreshControls();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// When the user selects a flight from the “Choose Flight” combo box two things should happen.  
        /// The first is that the appropriate flight should be displayed on the left of the program, and the second is that the “Choose Passenger” combo box 
        /// should be enabled and loaded with the passengers on the selected flight from the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbChooseFlight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                App.SelectedFlight = (Flight)cbChooseFlight.SelectedItem;
                gPassengerCommands.IsEnabled = true;

                if (App.SelectedFlight.AircraftType == "Boeing 767")
                {
                    gbSeatA380.Visibility = Visibility.Hidden;
                    gbSeat767.Visibility = Visibility.Visible;

                    SeatButtons = new[]
                    {
                        lblSeat_1_767,
                        lblSeat_2_767,
                        lblSeat_3_767,
                        lblSeat_4_767,
                        lblSeat_5_767,
                        lblSeat_6_767,
                        lblSeat_7_767,
                        lblSeat_8_767,
                        lblSeat_9_767,
                        lblSeat_10_767,
                        lblSeat_11_767,
                        lblSeat_12_767,
                        lblSeat_13_767,
                        lblSeat_14_767,
                        lblSeat_15_767,
                        lblSeat_16_767,
                        lblSeat_17_767,
                        lblSeat_18_767,
                        lblSeat_19_767,
                        lblSeat_20_767
                    };
                }
                else if (App.SelectedFlight.AircraftType == "Airbus A380")
                {
                    gbSeat767.Visibility = Visibility.Hidden;
                    gbSeatA380.Visibility = Visibility.Visible;

                    SeatButtons = new[]
                    {
                        lblSeat_1_A380,
                        lblSeat_2_A380,
                        lblSeat_3_A380,
                        lblSeat_4_A380,
                        lblSeat_5_A380,
                        lblSeat_6_A380,
                        lblSeat_7_A380,
                        lblSeat_8_A380,
                        lblSeat_9_A380,
                        lblSeat_10_A380,
                        lblSeat_11_A380,
                        lblSeat_12_A380,
                        lblSeat_13_A380,
                        lblSeat_14_A380,
                        lblSeat_15_A380,
                        lblSeat_16_A380,
                        lblSeat_17_A380,
                        lblSeat_18_A380,
                        lblSeat_19_A380,
                        lblSeat_20_A380,
                        lblSeat_21_A380,
                        lblSeat_22_A380,
                        lblSeat_23_A380,
                        lblSeat_24_A380
                    };
                }

                RefreshControls();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// Refresh the controls of the main window
        /// </summary>
        public void RefreshControls()
        {
            try
            {
                refreshingControls = true;

                cbChoosePassenger.IsEnabled = !choosingPassenger;
                cbChooseFlight.IsEnabled = !choosingPassenger;
                cmdAddPassenger.IsEnabled = !choosingPassenger;
                cmdChangeSeat.IsEnabled = !choosingPassenger;
                cmdDeletePassenger.IsEnabled = !choosingPassenger;

                var passengers = App.AirlineRepository.LoadPassengers();
                cbChoosePassenger.ItemsSource = passengers;
                cbChoosePassenger.SelectedItem = passengers.FirstOrDefault(passenger => passenger.PassengerID == App.SelectedPassenger?.PassengerID);

                var flightPassengerLinks = App.AirlineRepository.LoadFlightPassengerLinks(App.SelectedFlight);
                var takenSeatNumbers = new SortedSet<string>(flightPassengerLinks.Select(flightPassengerLink0 => flightPassengerLink0.SeatNumber), StringComparer.Ordinal);

                var passengerId = App.SelectedPassenger?.PassengerID;
                var flightPassengerLink = flightPassengerLinks.FirstOrDefault(flightPassengerLink0 => flightPassengerLink0.PassengerID == passengerId);

                var passengerSeatNumber = flightPassengerLink?.SeatNumber;
                lblPassengersSeatNumber.Content = passengerSeatNumber;

                foreach (var button in SeatButtons)
                {
                    var seatNumber = (string)button.Content;
                    Brush background;

                    if (seatNumber == passengerSeatNumber)
                    {
                        background = Brushes.Green;
                    }
                    else if (takenSeatNumbers.Contains(seatNumber))
                    {
                        background = Brushes.Red;
                    }
                    else
                    {
                        background = Brushes.Blue;
                    }

                    button.Background = background;
                }

                refreshingControls = false;
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// Opens up a new window which allows the user to enter a first name and last name that will be added to the database. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdAddPassenger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new wndAddPassenger().Show();
                Close();
                return;
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// Handles Error messages
        /// </summary>
        /// <param name="sClass"></param>
        /// <param name="sMethod"></param>
        /// <param name="exception"></param>
        private void HandleError(string sClass, string sMethod, Exception exception)
        {
            try
            {
                MessageBox.Show(sClass + "." + sMethod + " -> " + exception.Message + "---- " + exception.StackTrace);
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\Error.txt", Environment.NewLine + "HandleError Exception: " + ex.Message);
            }
        }

        /// <summary>
        /// The chosen passenger should show the seat number in the PassengerSeatTextBlock and display the selected passenger seat in green on the now disabled button in the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbChoosePassenger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (refreshingControls)
                {
                    return;
                }

                App.SelectedPassenger = (Passenger)cbChoosePassenger.SelectedItem;
                RefreshControls();

                return;
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// Deletes a current passengers seat in the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdDeletePassenger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var passenger = App.SelectedPassenger;

                if (passenger != null)
                {
                    AirlineRepository.DeletePassenger(passenger.PassengerID);
                    App.SelectedPassenger = null;
                    RefreshControls();
                }

                return;
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// Changes a current passengers seat in the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdChangeSeat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectSeat();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// Handles a click for any of the Buttons in the Boeing 767 Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gbSeat767_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GridButtonBase_Click(sender, e);
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// Handles a click for any of the Buttons in the Airbus A380 Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gbSeatA380_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GridButtonBase_Click(sender, e);
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// Handles the event of a button click. Either choose a passenger or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridButtonBase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var source = e.OriginalSource as FrameworkElement;

                if (source == null)
                {
                    return;
                }

                Control sourceControl = source as Control;

                if (choosingPassenger)
                {
                    if (sourceControl.Background == Brushes.Red)
                    {
                        return;
                    }

                    var passenger = App.SelectedPassenger;

                    if (passenger != null)
                    {
                        var seatNumber = source.Name.Split('_')[1];
                        App.AirlineRepository.AddOrUpdateSeat(App.SelectedFlight, passenger, seatNumber);
                    }

                    choosingPassenger = false;
                    RefreshControls();
                }
                else
                {
                    NotChoosingPassenger(source, sourceControl);
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// Is called when a seat button is clicked and the user is not choosing a passenger.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceControl"></param>
        private void NotChoosingPassenger(FrameworkElement source, Control sourceControl)
        {
            try
            {
                if (sourceControl.Background != Brushes.Red)
                {
                    return;
                }

                string seatNumber = source.Name.Split('_')[1];

                var flightPassengerLink = App.AirlineRepository.FlightPassengerLinkList.FirstOrDefault(flightPassengerLink0 => flightPassengerLink0.SeatNumber == seatNumber);

                if (flightPassengerLink != null)
                {
                    var passengerID = flightPassengerLink.PassengerID;
                    cbChoosePassenger.SelectedItem = App.AirlineRepository.PassengerList.FirstOrDefault(passenger => passenger.PassengerID == passengerID);
                }

                return;
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                   MethodInfo.GetCurrentMethod().Name, ex);
            }
        }
    }
}