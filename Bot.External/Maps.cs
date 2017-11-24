using Bot.Data;
using System.Collections.Generic;
using GMap.NET;
using GMap.NET.MapProviders;
using Bot.Data.Models;
//using Google.Maps;
//using Google.Maps.Direction;

namespace Bot.External
{
    public class Maps
    {
        public IList<Coordinate> GetRoute(TripRequest commuterRequest)
        {
            if (commuterRequest.GoingTo == GoingTo.Office)
            {
                return GetRoute(commuterRequest.Commuter.HomeCoordinate,
                    commuterRequest.Commuter.OfficeCoordinate);
            }
            else
            {
                return GetRoute(commuterRequest.Commuter.OfficeCoordinate,
                    commuterRequest.Commuter.HomeCoordinate);
            }
        }

        public IList<Coordinate> GetRoute(Coordinate pointA, Coordinate pointB)
        {
            PointLatLng p1 = new PointLatLng(pointA.Latitude, pointA.Longitude);
            PointLatLng p2 = new PointLatLng(pointB.Latitude, pointB.Longitude);

            var route = GoogleMapProvider.Instance.GetRoutePoints(p1, p2, false, true, 12, "blah");

            if (route == null)
                return new List<Coordinate>();

            var routeCoordinates = new List<Coordinate>();
            foreach (PointLatLng point in route.Points)
            {
                routeCoordinates.Add(new Coordinate
                {
                    Latitude = point.Lat,
                    Longitude = point.Lng
                });
            }

            return routeCoordinates;
        }
    }
}
