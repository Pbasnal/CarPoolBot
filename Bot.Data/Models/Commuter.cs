using System;
using System.ComponentModel.DataAnnotations;

namespace Bot.Data.Models
{
    public class Commuter : ModelBase
    {
        public Guid CommuterId { get; set; }
        public string CommuterName { get; set; }
        public Coordinate OfficeCoordinate { get; set; }
        public Coordinate HomeCoordinate { get; set; }
        public Vehicle Vehicle { get; set; }
        public CommuterStatus Status { get; set; }
        public string MediaId { get; set; }

        public Commuter(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
            HomeCoordinate = new Coordinate();
            OfficeCoordinate = new Coordinate();
            Vehicle = new Vehicle();
            CommuterId = new Guid();
        }
    }
}
