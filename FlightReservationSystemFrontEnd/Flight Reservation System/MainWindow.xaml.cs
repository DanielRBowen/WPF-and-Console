using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Flight_Reservation_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Create an object of type clsDataAccess to access the database
        /// </summary>
        internal DataAccess database = new DataAccess();

        /// <summary>
        /// A list of all the Flight in the database
        /// </summary>
        private ObservableCollection<Flight> flightList;

        /// <summary>
        /// A list of all the Passenger in the database
        /// </summary>
        private ObservableCollection<Passenger> passengerList;

        /// <summary>
        /// A list of all the FlightPassengerLinks in the database
        /// </summary>
        private ObservableCollection<FlightPassengerLink> flightPassengerLinkList;

        /// <summary>
        /// Initialize the MainWindow
        /// </summary>
        public MainWindow()
        {
            try
            {
                InitializeComponent();

                // Fill the flight combobox when the window starts
                FillChooseFlightComboBox();
                
                if (App.PreviousSelectedButton != null)
                {
                    App.PreviousSelectedButton.Background = Brushes.Green;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Fills in the flight combo box
        /// The “Choose Flight” combo box should be filled up with the two different flights that are extracted from the database Flight table.  
        /// When the form first loads, the flights should be extracted from the database and inserted into the combo box.  
        /// All other controls on the form should be disabled at this point.
        /// </summary>
        private void FillChooseFlightComboBox()
        {
            try
            {
                if (flightList != null)
                {
                    return;
                }

                var iFlightRet = 0;
                var flightDataset = database.ExecuteSQLStatement("SELECT * FROM flight", ref iFlightRet);
                var table = flightDataset.Tables[0];
                var columns = table.Columns;
                var flightIDColumn = columns["Flight_ID"];
                var flightNumberColumn = columns["Flight_Number"];
                var aircraftTypeColumn = columns["Aircraft_Type"];

                var flightsQuery =
                    from DataRow row in table.Rows
                    select new Flight
                    {
                        FlightID = (int)row[flightIDColumn],
                        FlightNumber = row[flightNumberColumn] as string,
                        AircraftType = row[aircraftTypeColumn] as string
                    };

                flightList = new ObservableCollection<Flight>(flightsQuery);
                ChooseFlightComboBox.ItemsSource = flightList;
                ChooseFlightComboBox.SelectedItem = flightList.FirstOrDefault(flight => flight.FlightID == App.SelectedFlight?.FlightID);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Shows an error message of the exception. Used to catch event errors.
        /// </summary>
        /// <param name="ex"></param>
        private void ShowErrorMessage(Exception ex)
        {
            MessageBox.Show(this, $"{ex.Message} {ex.TargetSite} {ex.StackTrace}/n", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Adds a Passenger to the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPassenger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new AddPassengerWindow().Show();
                Close();
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Changes a current passengers seat in the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeSeat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // The “Change Seat” and “Delete Passenger” buttons will not have any functionality for the first part of this assignment.
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Deletes a current passengers seat in the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeletePassenger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // The “Change Seat” and “Delete Passenger” buttons will not have any functionality for the first part of this assignment.
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// When the user selects a flight from the “Choose Flight” combo box two things should happen.  
        /// The first is that the appropriate flight should be displayed on the left of the program, and the second is that the “Choose Passenger” combo box 
        /// should be enabled and loaded with the passengers on the selected flight from the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseFlightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                App.SelectedFlight = (Flight)ChooseFlightComboBox.SelectedItem;

                if (App.SelectedFlight.AircraftType == "Airbus A380")
                {
                    AirbusA380Border.Visibility = Visibility.Visible;
                    Boeing767Border.Visibility = Visibility.Hidden;
                }
                else if (App.SelectedFlight.AircraftType == "Boeing 767")
                {
                    AirbusA380Border.Visibility = Visibility.Hidden;
                    Boeing767Border.Visibility = Visibility.Visible;
                }

                FillChoosePassengerComboBox();
                ChoosePassengerComboBox.IsEnabled = true;
                AddPassengerButton.IsEnabled = true;
                DeletePassengerButton.IsEnabled = true;
                ChangeSeatButton.IsEnabled = true;
                //App.PreviousSelectedButton = null;

                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Fills the choose passenger combo box
        /// Once the combobox is filled the passengers seats should be shown as taken on the UI
        /// </summary>
        private void FillChoosePassengerComboBox()
        {
            try
            {
                if (passengerList != null)
                {
                    passengerList.Clear();
                }

                var iPassengerRet = 0;
                string sSQL = "";

                if (App.SelectedFlight.FlightID == 1)
                {
                    sSQL = "SELECT Passenger.Passenger_ID, First_Name, Last_Name, Flight_ID, Seat_Number " +
                        "FROM Passenger, Flight_Passenger_Link " +
                        "WHERE Passenger.Passenger_ID = Flight_Passenger_Link.Passenger_ID AND " +
                        "Flight_id = 1";
                }
                else if (App.SelectedFlight.FlightID == 2)
                {
                    sSQL = "SELECT Passenger.Passenger_ID, First_Name, Last_Name, Flight_ID, Seat_Number " +
                        "FROM Passenger, Flight_Passenger_Link " +
                        "WHERE Passenger.Passenger_ID = Flight_Passenger_Link.Passenger_ID AND " +
                        "Flight_id = 2";
                }

                var passengerDataset = database.ExecuteSQLStatement(sSQL, ref iPassengerRet);
                var table = passengerDataset.Tables[0];
                var columns = table.Columns;
                var passengerIDColumn = columns["Passenger_ID"];
                var firstNameColumn = columns["First_Name"];
                var lastNameColumn = columns["Last_Name"];
                var seatNumberColumn = columns["Seat_Number"];
                var flightIDColumn = columns["Flight_ID"];

                var flightPassengerLinkQuery =
                    from DataRow row in table.Rows
                    select new FlightPassengerLink
                    {
                        PassengerID = (int)row[passengerIDColumn],
                        FlightID = (int)row[flightIDColumn],
                        SeatNumber = row[seatNumberColumn] as string
                    };

                var passengersQuery =
                    from DataRow row in table.Rows
                    select new Passenger
                    {
                        PassengerID = (int)row[passengerIDColumn],
                        FirstName = row[firstNameColumn] as string,
                        LastName = row[lastNameColumn] as string
                    };

                flightPassengerLinkList = new ObservableCollection<FlightPassengerLink>(flightPassengerLinkQuery);
                passengerList = new ObservableCollection<Passenger>(passengersQuery);

                ChoosePassengerComboBox.ItemsSource = passengerList;
                ChoosePassengerComboBox.SelectedItem = passengerList.FirstOrDefault(passenger => passenger.PassengerID == App.SelectedPassenger?.PassengerID);

                DisplayTakenSeats();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Displays the Taken Seats
        /// </summary>
        private void DisplayTakenSeats()
        {
            try
            {
                foreach (var seat in flightPassengerLinkList)
                {
                    string seatFlight = $"Seat_{seat.SeatNumber}_{App.SelectedFlight.FlightModel}";

                    Button seatButton = FindName(seatFlight) as Button;

                    seatButton.Background = Brushes.Red;
                    seatButton.IsEnabled = true;
                }

                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }
        
        /// <summary>
        /// The chosen passenger should show the seat number in the PassengerSeatTextBlock and display the selected passenger seat in green on the now disabled button in the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChoosePassengerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ChoosePassengerComboBox.SelectedItem != null)
                {
                    App.SelectedPassenger = (Passenger)ChoosePassengerComboBox.SelectedItem;

                    PassengerSeatTextBlock.Text = flightPassengerLinkList.FirstOrDefault(flightPassengerLink => flightPassengerLink.PassengerID == App.SelectedPassenger?.PassengerID).SeatNumber.ToString();

                    string seatFlight = $"Seat_{PassengerSeatTextBlock.Text}_{App.SelectedFlight.FlightModel}";
                    Button seatButton = FindName(seatFlight) as Button;
                    seatButton.Background = Brushes.Green;

                    if (App.PreviousSelectedButton != null)
                    {
                        App.PreviousSelectedButton.Background = Brushes.Red;
                    }

                    App.PreviousSelectedButton = seatButton;
                }
                else
                {
                    PassengerSeatTextBlock.Text = null;
                }

                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Handles a click for any of the Buttons in the Airbus A380 Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AirbusA380Grid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GridButtonBase_Click(sender, e);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Handles a click for any of the Buttons in the Boeing 767 Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Boeing767Grid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GridButtonBase_Click(sender, e);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Handles the event of a button click
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

                Button sourceButton = source as Button;
                if (sourceButton.Background != Brushes.Red)
                {
                    return;
                }

                string seatNumber = source.Name.Split('_')[1];

                var passengerID = flightPassengerLinkList.FirstOrDefault(flightPassengerLink => flightPassengerLink.SeatNumber == seatNumber).PassengerID;
                ChoosePassengerComboBox.SelectedItem = passengerList.FirstOrDefault(passenger => passenger.PassengerID == passengerID);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }
    }
}
