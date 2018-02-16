using System.Collections.Generic;

namespace Bot.Data.DataManagers
{
    // needs to be thought upon
    public class RouteManager
    {
        private IList<Route> _routes;

        public void AddRoute(Coordinate origin, Coordinate destination, List<Coordinate> waypoints)
        {
            var route = new Route
            {
                Waypoints = waypoints
            };

            if (_routes == null)
                _routes = new List<Route>();

            _routes.Add(route); 
        }

        public Route GetRoute(Coordinate origin, Coordinate destination)
        {
            return new Route();
        }
    }
}
