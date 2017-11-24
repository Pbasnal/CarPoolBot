using Bot.Data;
using Bot.Data.Models;
using Bot.External;
using Bot.MessagingFramework;
using Bot.Worker;
using Bot.Worker.Messages;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace EngineTestTool
{
    public partial class Gmap : Form
    {
        static int commuterId = 0;

        double MINX = 17.4118901285532;
        double MAXX = 17.4739467415265;

        double MINY = 78.323335647583;
        double MAXY = 78.3804130554199;

        int CountIncrement = 500;
        
        int MAX_COMMUTER_COUNT = 100;
        int MAX_POOLER_COUNT = 1000;

        Random random = new Random();
        IDictionary<Coordinate, IList<Commuter>> CommuterDataSet;
        IDictionary<Coordinate, IList<Commuter>> PoolerDataSet;

        IDictionary<Guid, TripStartedMessage> DriverPassenger;

        IList<GMarkerGoogle> poolerMarkers = new List<GMarkerGoogle>();
        GMapOverlay markersOverlay = new GMapOverlay("markers");
        GMapOverlay passengerOverlay = new GMapOverlay("passenger");
        GMapOverlay routesOverlay = new GMapOverlay("routes");
        GMapOverlay graphOverlay = new GMapOverlay("graph");

        IList<TripRequest> CommutersRequest = new List<TripRequest>();

        public Gmap()
        {
            InitializeComponent();
        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {

        }

        private void Gmap_Load(object sender, EventArgs e)
        {
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            gMapControl1.Position = new PointLatLng(17.4301704555282, 78.3572387695313);
            gMapControl1.Zoom = 14;

            gMapControl1.Overlays.Add(graphOverlay);
            gMapControl1.Overlays.Add(routesOverlay);
            gMapControl1.Overlays.Add(passengerOverlay);
            gMapControl1.Overlays.Add(markersOverlay);
            RefreshPlot();
        }

        private void RefreshPlot()
        {
            GenerateRandomPoints();
            PlotDataSet(0);

            gMapControl1.Update();
            RefreshCommuterTree();
        }

        private void GenerateRandomPoints()
        {
            CommuterDataSet = new Dictionary<Coordinate, IList<Commuter>>();
            PoolerDataSet = new Dictionary<Coordinate, IList<Commuter>>();

            TripRequestManager.ClearAllRequests();

            // generating random people who will pool
            for (int i = 0; i < MAX_POOLER_COUNT; i++)
            {
                var request = GetRandomRequest(GoingHow.Pool, GoingTo.Office);
                TripRequestManager.AddTripRequest(request);

                var keyPoint = GetKeyPoint(request.Commuter.HomeCoordinate);

                if (!PoolerDataSet.Keys.Contains(keyPoint))
                    PoolerDataSet.Add(keyPoint, new List<Commuter>());
                PoolerDataSet[keyPoint].Add(request.Commuter);
            }

            Maps maps = new Maps();
            // generating random people who will drive
            for (int i = 0; i < MAX_COMMUTER_COUNT; i++)
            {
                var request = GetRandomRequest(GoingHow.Drive, GoingTo.Office);
                var route = maps.GetRoute(request);
                if (route == null || route.Count == 0)
                {
                    i--;
                    continue;
                }

                TripRequestManager.AddTripRequest(request);
                CommutersRequest.Add(request);

                var keyPoint = GetKeyPoint(request.Commuter.HomeCoordinate);

                if (!CommuterDataSet.Keys.Contains(keyPoint))
                    CommuterDataSet.Add(keyPoint, new List<Commuter>());
                CommuterDataSet[keyPoint].Add(request.Commuter);
            }
        }

        private void PlotDataSet(int seriesIndex)
        {
            PlotGraph();
            foreach (var dataPoints in CommuterDataSet)
            {
                foreach (var commuter in dataPoints.Value)
                {
                    AddMarkerAt(commuter.CommuterId.ToString(), commuter.HomeCoordinate, GMarkerGoogleType.blue_dot, null);
                }
            }
            foreach (var dataPoints in PoolerDataSet)
            {
                foreach (var commuter in dataPoints.Value)
                {
                    poolerMarkers.Add(AddMarkerAt(commuter.CommuterId.ToString(), commuter.HomeCoordinate, GMarkerGoogleType.red_dot, null));
                }
            }
        }

        private void PlotGraph()
        {
            var keyMinPoint = PoolingMath.GetKeyPoint(new Coordinate { Latitude = MINX, Longitude = MINY });
            var inc = PoolingMath.GetIncrementAmount();

            for (keyMinPoint.Latitude = MINX; keyMinPoint.Latitude < MAXX; keyMinPoint.Latitude += inc)
            {
                List<PointLatLng> horizontal = new List<PointLatLng>
                    {
                        new PointLatLng(keyMinPoint.Latitude, MINY),
                        new PointLatLng(keyMinPoint.Latitude, MAXY)
                    };

                DisplayGraphLine(horizontal, Color.Blue, 1);
            }
            for (; keyMinPoint.Longitude < MAXY; keyMinPoint.Longitude += inc)
            {
                List<PointLatLng> vertical = new List<PointLatLng>
                    {
                        new PointLatLng(MINX, keyMinPoint.Longitude),
                        new PointLatLng(MAXX, keyMinPoint.Longitude)
                    };

                DisplayGraphLine(vertical, Color.Blue, 1);
            }
        }

        private TripRequest GetRandomRequest(GoingHow how, GoingTo to)
        {
            Commuter commuter = new Commuter
            {
                CommuterId = Guid.NewGuid(),
                CommuterName = "TestCommuter" + commuterId++,
                HomeCoordinate = new Coordinate
                {
                    Latitude = RandomDoubleBetween(MINX, MAXX),
                    Longitude = RandomDoubleBetween(MINY, MAXY)
                },
                OfficeCoordinate = new Coordinate
                {
                    Latitude = RandomDoubleBetween(MINX, MAXX),
                    Longitude = RandomDoubleBetween(MINY, MAXY)
                },
                Vehicle = new Vehicle
                {
                    MaxPassengerCount = 4,
                    OccupiedSeats = 0,
                    VehicleNumber = Guid.NewGuid().ToString(),
                    VehicleOnboarded = true
                }
            };

            return new TripRequest
            {
                Commuter = commuter,
                GoingHow = how,
                GoingTo = to,
                RequestTime = DateTime.UtcNow,
                Status = RequestStatus.Waiting,
                WaitTime = TimeSpan.FromMinutes(15)
            };
        }

        private double RandomDoubleBetween(double a, double b)
        {
            return a + random.NextDouble() * (b - a);
        }

        private Coordinate GetKeyPoint(Coordinate point)
        {
            var roundedPoint = new Coordinate
            {
                Latitude = Math.Round(point.Latitude, 3),
                Longitude = Math.Round(point.Longitude, 3)
            };

            int xcount = BitConverter.GetBytes(decimal.GetBits((decimal)roundedPoint.Latitude)[3])[2];
            int ycount = BitConverter.GetBytes(decimal.GetBits((decimal)roundedPoint.Longitude)[3])[2];

            var xWithoutDecimal = roundedPoint.Latitude * Math.Pow(10, xcount);
            var yWithoutDecimal = roundedPoint.Longitude * Math.Pow(10, ycount);

            roundedPoint.Latitude = (xWithoutDecimal + CountIncrement - (xWithoutDecimal % CountIncrement)) / Math.Pow(10, xcount);
            roundedPoint.Longitude = (yWithoutDecimal + CountIncrement - (yWithoutDecimal % CountIncrement)) / Math.Pow(10, ycount);

            return roundedPoint;
        }

        private GMarkerGoogle AddMarkerAt(string markerId, Coordinate cLoc, GMarkerGoogleType markerType, GMapOverlay overlay)
        {
            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(cLoc.Latitude, cLoc.Longitude),
              markerType);
            marker.Tag = markerId;
            if (overlay == null)
                markersOverlay.Markers.Add(marker);
            else
                overlay.Markers.Add(marker);

            return marker;
        }


        private void RefreshCommuterTree()
        {
            RefreshTree(commutersTreeView, CommutersRequest.Select(x => x.Commuter).ToList());
            commutersTreeView.ExpandAll();
        }

        private void RefreshPassengerTree(Guid commuterId)
        {
            var passenger = DriverPassenger.FirstOrDefault(x => x.Key == commuterId);
            if (passenger.Value == null)
                return;
            RefreshTree(poolersTreeView, passenger.Value.Trip.Passengers);
            poolersTreeView.ExpandAll();
        }

        private void RefreshTree(TreeView tree, IList<Commuter> commuters)
        {
            tree.Nodes.Clear();
            foreach (var commuter in commuters)
            {
                TreeNode rootNode = new TreeNode
                {
                    Text = commuter.CommuterName,
                    Tag = commuter.CommuterId
                };

                if (!tree.InvokeRequired)
                {
                    tree.Nodes.Add(rootNode);
                    continue;
                }

                tree.BeginInvoke((MethodInvoker)delegate
               {
                   tree.Nodes.Add(rootNode);
               });
            }
        }

        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
            var pos = gMapControl1.Position;
        }

        private void gMapControl1_OnMarkerClick(GMapMarker marker, MouseEventArgs e)
        {
            var i = marker.Position;
        }

        private void start_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            foreach (var request in CommutersRequest)
            {
                MessageBus.Instance.Publish(new ProcessTripOwnerRequestMessage
                {
                    TripOwnerRequest = request
                });
            }
        }

        private void ClearMap()
        {
            while (markersOverlay.Markers.Count > 0)
            {
                markersOverlay.Markers[0] = null;
                markersOverlay.Markers.RemoveAt(0);
            }
            while (passengerOverlay.Markers.Count > 0)
            {
                passengerOverlay.Markers[0] = null;
                passengerOverlay.Markers.RemoveAt(0);
            }
            while (routesOverlay.Routes.Count > 0)
            {
                routesOverlay.Routes[0] = null;
                routesOverlay.Routes.RemoveAt(0);
            }
        }

        private void MakeEverythingLessSignificant()
        {
            foreach (var marker in markersOverlay.Markers)
            {
                marker.Size = new Size(15, 15);
            }
            foreach (var marker in passengerOverlay.Markers)
            {
                marker.Size = new Size(15, 15);
            }
            foreach (var route in routesOverlay.Routes)
            {
                route.Stroke = Pens.AliceBlue;
            }
        }

        private void MakeMarkerSignificant(string markerId)
        {
            var marker = markersOverlay.Markers.FirstOrDefault(m => (string)m.Tag == markerId);
            if (marker != null)
            {
                marker.Size = new Size(32, 32);
                return;
            }
        }

        private void DisplayState()
        {
            var state = PoolingEngine.Instance.State;
            var totalCommuter = state.CommuterRequestProcessTable.Count;
            var totalProcessedCommuters = state.CommuterRequestProcessTable.Values.Where(x => x.Trip.Owner.Status == CommuterStatus.InTrip).Count();

            stateText.Text = "Total Commuters : " + totalCommuter + "\n" +
                "Total Processed Commuters : " + totalProcessedCommuters;
        }

        //public void DisplayTrip(TripRequest request)
        //{
        //    ClearMap();
        //    CheckForIllegalCrossThreadCalls = false;
        //    List<PointLatLng> points = ConvertCoordinatesToPoint(PoolingEngine.Instance.GetCommuterRoute(request).Path.ToList());
        //    if (points.Count == 0)
        //        return;

        //    DisplayRoute(points, Color.Red, 3);
        //    AddMarkerAt(points.First(), GMarkerGoogleType.green_dot, null);
        //    AddMarkerAt(points.Last(), GMarkerGoogleType.purple_dot, null);

        //    //RefreshTree(poolersTreeView, PoolingEngine.GetPoolersInTrip(request));

        //    foreach (var pooler in PoolingEngine.Instance.GetPoolersInTrip(request))
        //    {
        //        AddMarkerAt(pooler.HomeCoordinate, GMarkerGoogleType.orange, passengerOverlay);
        //    }

        //    gMapControl1.Update();
        //    gMapControl1.Refresh();
        //}

        public void DisplayRoute(List<PointLatLng> points, Color color, int size)
        {
            GMapRoute route = new GMapRoute(points, "A walk in the park");
            route.Stroke = new Pen(color, size);
            routesOverlay.Routes.Add(route);
            Thread.Sleep(50);//added bcoz while adding routes GMapRoute lib is also accessing routes to display
        }

        private void DisplayGraphLine(List<PointLatLng> points, Color color, int size)
        {
            GMapRoute line = new GMapRoute(points, "A walk in the park");
            line.Stroke = new Pen(color, size);
            graphOverlay.Routes.Add(line);
        }

        public List<PointLatLng> ConvertCoordinatesToPoint(List<Coordinate> coordinates)
        {
            var points = new List<PointLatLng>();

            foreach (var coordinate in coordinates)
            {
                points.Add(new PointLatLng(coordinate.Latitude, coordinate.Longitude));
            }
            return points;
        }

        public void DisplayTrip(TripStartedMessage message)
        {
            MakeEverythingLessSignificant();
            CheckForIllegalCrossThreadCalls = false;
            List<PointLatLng> points = ConvertCoordinatesToPoint(message.Route.Path.ToList());
            if (points.Count == 0)
                return;

            DisplayRoute(points, Color.Red, 3);
            var markerId = message.Trip.Owner.CommuterId.ToString();
            MakeMarkerSignificant(message.Trip.Owner.CommuterId.ToString());
            markerId += "dest";
            var marker = markersOverlay.Markers.FirstOrDefault(m => (string)m.Tag == markerId);
            if (marker == null)
                AddMarkerAt(markerId, message.Route.Destination, GMarkerGoogleType.purple_dot, null);
            else
                MakeMarkerSignificant(markerId);

            foreach (var pooler in message.Trip.Passengers)
            {
                MakeMarkerSignificant(pooler.CommuterId.ToString());
            }
            gMapControl1.Update();
            gMapControl1.Refresh();
        }

        public void AddCommuterAndPassengersToList(TripStartedMessage message)
        {
            //should not get duplicate tripstarted message or multiple messages for same commuter

            if (DriverPassenger == null)
                DriverPassenger = new Dictionary<Guid, TripStartedMessage>();
            if (DriverPassenger.ContainsKey(message.Trip.Owner.CommuterId))
            {
                foreach (var passenger in message.Trip.Passengers)
                {
                    var p = DriverPassenger[message.Trip.Owner.CommuterId].Trip.Passengers.FirstOrDefault(x => x.CommuterId == passenger.CommuterId);
                    if (p != null)
                    {
                        //should inform if the passenger already exists
                        continue;
                    }
                    DriverPassenger[message.Trip.Owner.CommuterId].Trip.AddPassenger(passenger);
                }
            }
            else
            {
                DriverPassenger.Add(message.Trip.Owner.CommuterId, message);
            }

            //RefreshCommuterTree();
            //RefreshPassengerTree(message.Trip.Owner.CommuterId);
        }

        private void commutersTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var commuterId = (Guid)e.Node.Tag;

            if (DriverPassenger == null)
                return;
            var message = DriverPassenger[commuterId];
            DisplayTrip(message);

            RefreshPassengerTree(commuterId);
        }
    }
}
