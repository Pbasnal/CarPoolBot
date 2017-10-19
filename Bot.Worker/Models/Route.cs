using Bot.Data;
using System.Collections.Generic;

namespace Bot.Worker.Models
{
    public class Route
    {
        public Coordinate Origin;
        public Coordinate Destination;
        public IList<Coordinate> Path;

        public Route(int pathLength)
        {
            Path = new List<Coordinate>(pathLength);
        }
    }
}
 