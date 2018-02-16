using Bot.Data;
using System.Collections.Generic;
using GMap.NET;
using GMap.NET.MapProviders;
using Bot.Data.Models;
using Bot.Logger;
using Bot.Common;
using System;
//using Google.Maps;
//using Google.Maps.Direction;

namespace Bot.External
{
    public class Maps
    {
        public IList<Coordinate> GetRoute(Guid flowId,TripRequest commuterRequest)
        {
            if (commuterRequest.GoingTo == GoingTo.Office)
            {
                new BotLogger<TripRequest>(commuterRequest.OperationId, flowId, EventCodes.GetRouteToOffice, commuterRequest)
                    .Debug();

                return GetRoute(commuterRequest.OperationId, flowId, commuterRequest.Commuter.HomeCoordinate,
                    commuterRequest.Commuter.OfficeCoordinate);
            }
            else
            {
                new BotLogger<TripRequest>(commuterRequest.OperationId, flowId, EventCodes.GetRouteToHome, commuterRequest)
                    .Debug();

                return GetRoute(commuterRequest.OperationId, flowId, commuterRequest.Commuter.OfficeCoordinate,
                    commuterRequest.Commuter.HomeCoordinate);
            }
        }

        public IList<Coordinate> GetRoute(Guid operationId, Guid flowId, Coordinate pointA, Coordinate pointB)
        {
            Tuple<Coordinate, Coordinate> logObject = new Tuple<Coordinate, Coordinate>(pointA, pointB);
            new BotLogger<Tuple<Coordinate, Coordinate>>(operationId, flowId, EventCodes.GetRouteBetweenCoordinates, logObject)
                .Debug();

            PointLatLng p1 = new PointLatLng(pointA.Latitude, pointA.Longitude);
            PointLatLng p2 = new PointLatLng(pointB.Latitude, pointB.Longitude);

            var route = GoogleMapProvider.Instance.GetRoutePoints(p1, p2, false, true, 12, string.Empty);
            if (route == null)
            {
                new BotLogger<MapRoute>(operationId, flowId, EventCodes.FaileToGetRouteBetweenPoints, route)
                    .Error();
                return new List<Coordinate>();
            }
            new BotLogger<MapRoute>(operationId, flowId, EventCodes.GotRouteBetweenPointsFromGoogle, route)
                .Debug();

            var routeCoordinates = new List<Coordinate>();
            foreach (PointLatLng point in route.Points)
            {
                routeCoordinates.Add(new Coordinate
                {
                    Latitude = point.Lat,
                    Longitude = point.Lng
                });
            }

            new BotLogger<List<Coordinate>>(operationId, flowId, EventCodes.GotRouteBetweenPoints, routeCoordinates)
                .Debug();

            return routeCoordinates;
        }
    }
}
