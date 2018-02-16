﻿namespace Assignment6AirlineReservation
{
    /// <summary>
    /// A flight 
    /// </summary>
    public class Flight
    {
        /// <summary>
        /// The unique ID of the flight
        /// </summary>
        public int FlightID { get; set; }

        /// <summary>
        /// The number of the flight
        /// </summary>
        public string FlightNumber { get; set; }

        /// <summary>
        /// The Aircraft Type which the flight will use
        /// </summary>
        public string AircraftType { get; set; }

        /// <summary>
        /// Returns the flight number and aircraft type in a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{FlightNumber} - {AircraftType}";
        }
    }
}
