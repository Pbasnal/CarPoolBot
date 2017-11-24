using System;

namespace Bot.Data.Models
{
    public class Commuter
    {
        public string CommuterName { get; set; }
        public Guid CommuterId { get; set; }
        public Coordinate OfficeCoordinate { get; set; }
        public Coordinate HomeCoordinate { get; set; }
        public Vehicle Vehicle { get; set; }
        public CommuterStatus Status { get; set; }
        public string MediaId { get; set; }

        public Commuter()
        {
            HomeCoordinate = new Coordinate();
            OfficeCoordinate = new Coordinate();
            Vehicle = new Vehicle();
            CommuterId = new Guid();
        }
    }
}
