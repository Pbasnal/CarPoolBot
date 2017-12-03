using Bot.MessagingFramework;
using Bot.Worker.Messages;

namespace EngineTestTool
{
    public class VehicleOwnerAddedToStateHandler : MessageHandler<VehicleOwnerAddedToStateMessage>
    {
        public override void Handle(VehicleOwnerAddedToStateMessage message)
        {
            var path = Program.Map.ConvertCoordinatesToPoint(message.Route.Waypoints);
            //Program.Map.DisplayRoute(path, System.Drawing.Color.Red, 4);
        }
    }
}
