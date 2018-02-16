namespace Flight_Reservation_System
{
    /// <summary>
    /// Links a passenger with a flight when they are added
    /// </summary>
    public class FlightPassengerLink
    {
        /// <summary>
        /// The ID of the flight
        /// </summary>
        public int FlightID { get; set; }

        /// <summary>
        /// The ID of the passenger.
        /// </summary>
        public int PassengerID { get; set; }

        /// <summary>
        /// The seat number where the passenger sits on the flight.
        /// </summary>
        public string SeatNumber { get; set; }
    }
}
