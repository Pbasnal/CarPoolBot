using Bot.Data.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Bot.Data.EfModels
{
    public class EfVehicle
    {
        [Key]
        public Guid VehicleId { get; set; }

        public bool VehicleOnboarded { get; set; }
        public int MaxPassengerCount { get; set; }
        public int OccupiedSeats { get; set; }
        public string VehicleNumber { get; set; }

        public EfVehicle(Vehicle vehicle)
        {
            VehicleId = vehicle.VehicleId;
            VehicleOnboarded = vehicle.VehicleOnboarded;
            MaxPassengerCount = vehicle.MaxPassengerCount;
            OccupiedSeats = vehicle.OccupiedSeats;
            VehicleNumber = vehicle.VehicleNumber;
        }

        public static explicit operator EfVehicle(Vehicle vehicle)
        {
            return new EfVehicle(vehicle);
        }

        public Vehicle GetVehicle()
        {
            return new Vehicle
            {
                MaxPassengerCount = MaxPassengerCount,
                OccupiedSeats = OccupiedSeats,
                VehicleId = VehicleId,
                VehicleNumber = VehicleNumber,
                VehicleOnboarded = VehicleOnboarded
            };
        }
    }
}
