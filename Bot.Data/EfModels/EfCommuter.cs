using Bot.Data.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Bot.Data.EfModels
{
    public class EfCommuter
    {
        [Key]
        public Guid CommuterId { get; set; }
        public string CommuterName { get; set; }
        public EfCoordinate OfficeCoordinate { get; set; }
        public EfCoordinate HomeCoordinate { get; set; }
        public EfVehicle Vehicle { get; set; }
        public CommuterStatus Status { get; set; }
        public string MediaId { get; set; }

        public EfCommuter(Commuter commuter)
        {
            CommuterId = commuter.CommuterId;
            CommuterName = commuter.CommuterName;
            OfficeCoordinate = (EfCoordinate)commuter.OfficeCoordinate;
            HomeCoordinate = (EfCoordinate)commuter.HomeCoordinate;
            Vehicle = (EfVehicle)commuter.Vehicle;
            Status = commuter.Status;
            MediaId = commuter.MediaId;
        }

        public static explicit operator EfCommuter(Commuter commuter)
        {
            return new EfCommuter(commuter);
        }

        public Commuter GetCommuter(Guid operationId, Guid flowId)
        {
            return new Commuter(operationId, flowId)
            {
                CommuterId = CommuterId,
                CommuterName = CommuterName,
                HomeCoordinate = HomeCoordinate.GetCoordinate(),
                OfficeCoordinate = OfficeCoordinate.GetCoordinate(),
                MediaId = MediaId,
                Status = Status,
                Vehicle = Vehicle.GetVehicle()
            };
        }
    }
}
