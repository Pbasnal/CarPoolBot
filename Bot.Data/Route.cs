using System.Collections.Generic;

namespace Bot.Data
{
    public class Route
    {
        public Coordinate Origin
        {
            get
            {
                if (Waypoints.Count == 0)
                    return Waypoints[0];
                return new Coordinate();
            }
        }
        public Coordinate Destination
        {
            get
            {
                if (Waypoints.Count == 0)
                    return Waypoints[Waypoints.Count - 1];
                return new Coordinate();
            }
        }

        public List<Coordinate> Waypoints;

        public Route()
        {
            Waypoints = new List<Coordinate>();
        }

        public Route(List<Coordinate> waypoints)
        {
            Waypoints = new List<Coordinate>(waypoints);
        }
    }
}
