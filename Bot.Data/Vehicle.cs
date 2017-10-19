namespace Bot.Data
{
    public class Vehicle
    {
        public bool VehicleOnboarded = false;
        public int MaxPassengerCount = 0;
        public int OccupiedSeats = 0;
        public string VehicleNumber = "";

        public int RemainingSeats
        {
            get
            {
                return MaxPassengerCount - OccupiedSeats;
            }
        }
    }
}
