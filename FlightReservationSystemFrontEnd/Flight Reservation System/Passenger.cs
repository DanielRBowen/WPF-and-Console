﻿namespace Flight_Reservation_System
{
    /// <summary>
    /// Contains data for a passenger.
    /// </summary>
    public class Passenger
    {
        /// <summary>
        /// The unique ID for a passenger
        /// </summary>
        public int PassengerID { get; set; }

        /// <summary>
        /// The first name of the passenger
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The second name of the passenger.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Overrides the ToString method to return the full name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
