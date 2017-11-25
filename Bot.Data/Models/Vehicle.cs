using System;
using System.ComponentModel.DataAnnotations;

namespace Bot.Data.Models
{
    public class Vehicle
    {
        [Key]
        public Guid VehicleId { get; set; }

        public bool VehicleOnboarded { get; set; }
        public int MaxPassengerCount { get; set; }
        public int OccupiedSeats { get; set; }
        public string VehicleNumber { get; set; }

        public Vehicle()
        {
            VehicleId = Guid.NewGuid();
            VehicleOnboarded = false;
            MaxPassengerCount = 0;
            OccupiedSeats = 0;

        }

        public int RemainingSeats
        {
            get
            {
                return MaxPassengerCount - OccupiedSeats;
            }
        }
    }
}
