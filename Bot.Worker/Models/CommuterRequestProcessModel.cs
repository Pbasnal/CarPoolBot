using Bot.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Worker.Models
{
    public class CommuterRequestProcessModel
    {
        public Trip Trip { get; private set; }
        public TripRequestInProcess TripOwnerRequest { get; private set; }
        public List<TripRequestInProcess> PoolerRequests { get; private set; }

        private Coordinate _currentNode;
        public Route KeyNodeRoute { get; private set; }
        public Route CompleteRoute { get; private set; }

        public bool LastNodeReached = false;

        public Coordinate CurrentNode
        {
            get
            {
                var currentNode = _currentNode;
                var currentNodeIndex = KeyNodeRoute.Path.IndexOf(_currentNode);
                if (currentNodeIndex < KeyNodeRoute.Path.Count - 1)
                    _currentNode = KeyNodeRoute.Path[currentNodeIndex + 1];
                else
                    LastNodeReached = true;

                return currentNode;
            }
        }

        public CommuterRequestProcessModel(Trip trip, IList<Coordinate> inputRoute)
        {
            Trip = trip;
            KeyNodeRoute = new Route(inputRoute.Count);

            if (inputRoute == null || inputRoute.Count == 0)
                throw new ArgumentException("inputRoute");

            foreach (var coordinate in inputRoute)
            {
                var keyPoint = PoolingMath.GetKeyPoint(coordinate);
                if (!KeyNodeRoute.Path.Contains(keyPoint))
                    KeyNodeRoute.Path.Add(keyPoint);
            }

            if (KeyNodeRoute.Path.Count > 0)
            {
                _currentNode = KeyNodeRoute.Path[0];
                KeyNodeRoute.Origin = KeyNodeRoute.Path[0];
                KeyNodeRoute.Destination = KeyNodeRoute.Path[KeyNodeRoute.Path.Count - 1];
            }
            else
                KeyNodeRoute.Path = null;

            CompleteRoute = new Route(inputRoute.Count);
            CompleteRoute.Path = new List<Coordinate>(inputRoute);
            PoolerRequests = new List<TripRequestInProcess>();
        }
    }
}
