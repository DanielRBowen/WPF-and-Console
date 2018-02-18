using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Assignment6AirlineReservation
{
    /// <summary>
    /// Allows access to get, update, and delete data from the Airline Database
    /// </summary>
    public class AirlineRepository
    {
        /// <summary>
        /// Access to the database
        /// </summary>
        private clsDataAccess clsData = new clsDataAccess(); 

        /// <summary>
        /// A list of all the Flight in the database
        /// </summary>
        public IList<Flight> FlightList { get; set; }

        /// <summary>
        /// A list of all the Passenger in the database
        /// </summary>
        public IList<Passenger> PassengerList { get; set; }

        /// <summary>
        /// A list of all the FlightPassengerLinks in the database
        /// </summary>
        public IList<FlightPassengerLink> FlightPassengerLinkList { get; set; }

        /// <summary>
        /// Updates the passengers first name and last name.
        /// </summary>
        /// <param name="passenger"></param>
        internal static void UpdatePassenger(Passenger passenger)
        {
            try
            {
                var sql = $"update passenger set First_Name = '{passenger.FirstName}', Last_Name = '{passenger.LastName}' where passenger_id = {passenger.PassengerID}";
                var repository = new clsDataAccess();
                repository.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Insert a passenger into the database
        /// </summary>
        /// <param name="passenger"></param>
        /// <returns></returns>
        internal static int InsertPassenger(Passenger passenger)
        {
            try
            {
                var repository = new clsDataAccess();

                var sql = "select max(passenger_id) from passenger";
                var result = repository.ExecuteScalarSQL(sql);

                if (!int.TryParse(result, out var passengerId))
                {
                    passengerId = 1;
                }
                else
                {
                    ++passengerId;
                }

                sql = $"Insert into Passenger (passenger_id, first_name, last_name) values ({passengerId}, '{passenger.FirstName}', '{passenger.LastName}')";
                repository.ExecuteNonQuery(sql);

                return passengerId;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Initializes all the flights
        /// </summary>
        public AirlineRepository()
        {
            try
            {
                FillFlights();
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Fills the flights
        /// </summary>
        public void FillFlights()
        {
            try
            {
                var iFlightRet = 0;
                var flightDataset = clsData.ExecuteSQLStatement("SELECT * FROM flight", ref iFlightRet);
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

                FlightList = new ObservableCollection<Flight>(flightsQuery);
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Loads the passengers from the database into a IList of Passengers
        /// </summary>
        public IList<Passenger> LoadPassengers()
        {
            try
            {
                var sql = @"
select Passenger_ID, First_Name, Last_Name
from Passenger";

                var count = 0;
                var result = clsData.ExecuteSQLStatement(sql, ref count);
                var table = result.Tables[0];
                var columns = table.Columns;
                var passengerIDColumn = columns["Passenger_ID"];
                var firstNameColumn = columns["First_Name"];
                var lastNameColumn = columns["Last_Name"];

                var passengersQuery =
                    from DataRow row in result.Tables[0].Rows
                    select new Passenger
                    {
                        PassengerID = (int)row[passengerIDColumn],
                        FirstName = row[firstNameColumn] as string,
                        LastName = row[lastNameColumn] as string
                    };

                var passengerList = new ObservableCollection<Passenger>(passengersQuery);
                PassengerList = passengerList;

                return passengerList;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Loads the flight passenger links and returns it as a list
        /// </summary>
        /// <param name="flight"></param>
        /// <returns></returns>
        public IList<FlightPassengerLink> LoadFlightPassengerLinks(Flight flight)
        {
            try
            {
                var sql = @"
select Passenger_ID, Flight_ID, Seat_Number
from Flight_Passenger_Link
where Flight_ID = " + flight.FlightID.ToString(NumberFormatInfo.InvariantInfo);

                var count = 0;
                var result = clsData.ExecuteSQLStatement(sql, ref count);
                var table = result.Tables[0];
                var columns = table.Columns;
                var passengerIDColumn = columns["Passenger_ID"];
                var flightIDColumn = columns["Flight_ID"];
                var seatNumberColumn = columns["Seat_Number"];

                var flightPassengerLinkQuery =
                        from DataRow row in table.Rows
                        select new FlightPassengerLink
                        {
                            PassengerID = (int)row[passengerIDColumn],
                            FlightID = (int)row[flightIDColumn],
                            SeatNumber = row[seatNumberColumn] as string
                        };

                var flightPassengerLinkList = new ObservableCollection<FlightPassengerLink>(flightPassengerLinkQuery);
                FlightPassengerLinkList = flightPassengerLinkList;

                return flightPassengerLinkList;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Add a passenger with the desired first name and last name into the database
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        public void AddPassenger(string firstName, string lastName)
        {
            try
            {
                string sSQL = $"INSERT INTO PASSENGER(First_Name, Last_Name) VALUES('{firstName}','{lastName}')";
                clsData.ExecuteScalarSQL(sSQL);
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Returns true if the seat exists
        /// </summary>
        /// <param name="flightId"></param>
        /// <param name="passengerId"></param>
        /// <returns></returns>
        private bool SeatExists(int flightId, int passengerId)
        {
            try
            {
                var sql = $"select 1 from Flight_Passenger_Link where flight_id = {flightId.ToString(NumberFormatInfo.InvariantInfo)} and passenger_id = {passengerId.ToString(NumberFormatInfo.InvariantInfo)}";
                var exists = clsData.ExecuteScalarSQL(sql);
                return !string.IsNullOrWhiteSpace(exists);
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Updates a seat assoiciated with the flightId, passengerId, and seatNumber
        /// </summary>
        /// <param name="flightId"></param>
        /// <param name="passengerId"></param>
        /// <param name="seatNumber"></param>
        private void UpdateSeat(int flightId, int passengerId, string seatNumber)
        {
            try
            {
                var sql = $"update Flight_Passenger_Link set Seat_Number = '{seatNumber}' where flight_id = {flightId.ToString(NumberFormatInfo.InvariantInfo)} and passenger_id = {passengerId.ToString(NumberFormatInfo.InvariantInfo)}";
                clsData.ExecuteNonQuery(sql);
                return;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Adds a seat for the passenger into the database
        /// </summary>
        /// <param name="flightId"></param>
        /// <param name="passengerId"></param>
        /// <param name="seatNumber"></param>
        private void AddSeat(int flightId, int passengerId, string seatNumber)
        {
            try
            {
                var sql = $"INSERT INTO Flight_Passenger_Link(Flight_ID, Passenger_ID, Seat_Number) VALUES( {flightId.ToString(NumberFormatInfo.InvariantInfo)} , {passengerId.ToString(NumberFormatInfo.InvariantInfo)} , '{seatNumber}')";
                clsData.ExecuteNonQuery(sql);
                return;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Adds or updates a seat if it exists
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="passenger"></param>
        /// <param name="seatNumber"></param>
        public void AddOrUpdateSeat(Flight flight, Passenger passenger, string seatNumber)
        {
            try
            {
                var flightId = flight.FlightID;
                var passengerId = passenger.PassengerID;

                if (SeatExists(flightId, passengerId))
                {
                    UpdateSeat(flightId, passengerId, seatNumber);
                }
                else
                {
                    AddSeat(flightId, passengerId, seatNumber);
                }

                return;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Changes the seat in the data with the parameters
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="passenger"></param>
        /// <param name="seatNumber"></param>
        public void ChangeSeat(Flight flight, Passenger passenger, string seatNumber)
        {
            try
            {
                var iRet = 0;
                string sSQL = sSQL = $"UPDATE FLIGHT_PASSENGER_LINK SET Seat_Number = '{seatNumber}' WHERE FLIGHT_ID = {flight.FlightID} AND PASSENGER_ID = {passenger.PassengerID}";
                clsData.ExecuteSQLStatement(sSQL, ref iRet);
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes the passenger in the data
        /// </summary>
        /// <param name="passenger"></param>
        public void DeletePassenger(Passenger passenger)
        {
            try
            {
                var iRet = 0;
                string sSQL = $"Delete FROM PASSENGER WHERE PASSENGER_ID = {passenger.PassengerID}";
                clsData.ExecuteSQLStatement(sSQL, ref iRet);
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Returns true if the passenger for the passengerId exists
        /// </summary>
        /// <param name="passengerId"></param>
        /// <returns></returns>
        public static bool PassengerExists(int passengerId)
        {
            try
            {
                var repository = new clsDataAccess();
                var sql = $"select 1 from passenger where passenger_id = {passengerId}";
                var exists = repository.ExecuteScalarSQL(sql);
                return !string.IsNullOrWhiteSpace(exists);
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes a passenger in the database associated with the corresponding Id
        /// </summary>
        /// <param name="passengerId"></param>
        public static void DeletePassenger(int passengerId)
        {
            try
            {
                var repository = new clsDataAccess();

                var sql = $"delete from FLIGHT_PASSENGER_LINK where Passenger_ID = {passengerId.ToString(NumberFormatInfo.InvariantInfo)}";
                repository.ExecuteNonQuery(sql);

                sql = $"delete from Passenger where Passenger_ID = {passengerId.ToString(NumberFormatInfo.InvariantInfo)}";
                repository.ExecuteNonQuery(sql);

                return;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }
    }
}
