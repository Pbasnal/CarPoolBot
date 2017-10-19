using Bot.Data;
using Bot.External;
using Bot.Worker;
using Bot.Worker.Core;
using Bot.Worker.Models;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        int OldCommuterCount = 0;
        int OldPoolerCount = 0;

        int MAX_COMMUTER_COUNT = 100;
        int MAX_POOLER_COUNT = 1000;

        Random random = new Random();
        IDictionary<Coordinate, IList<Coordinate>> CommuterDataSet;
        IDictionary<Coordinate, IList<Coordinate>> PoolerDataSet;


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

            gMapControl1.Overlays.Add(markersOverlay);
            gMapControl1.Overlays.Add(routesOverlay);
            gMapControl1.Overlays.Add(graphOverlay);
            gMapControl1.Overlays.Add(passengerOverlay);
            RefreshPlot();
        }

        private void RefreshPlot()
        {
            GenerateRandomPoints();
            PlotDataSet(0);

            gMapControl1.Update();
            RefreshTrees();
        }

        private void GenerateRandomPoints()
        {
            CommuterDataSet = new Dictionary<Coordinate, IList<Coordinate>>();
            PoolerDataSet = new Dictionary<Coordinate, IList<Coordinate>>();

            TripRequestManager.ClearAllRequests();
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
                    CommuterDataSet.Add(keyPoint, new List<Coordinate>());
                CommuterDataSet[keyPoint].Add(request.Commuter.HomeCoordinate);
            }

            // generating random people who will pool
            for (int i = 0; i < MAX_POOLER_COUNT; i++)
            {
                var request = GetRandomRequest(GoingHow.Pool, GoingTo.Office);
                TripRequestManager.AddTripRequest(request);

                var keyPoint = GetKeyPoint(request.Commuter.HomeCoordinate);

                if (!PoolerDataSet.Keys.Contains(keyPoint))
                    PoolerDataSet.Add(keyPoint, new List<Coordinate>());
                PoolerDataSet[keyPoint].Add(request.Commuter.HomeCoordinate);
            }
        }

        private void PlotDataSet(int seriesIndex)
        {
            PlotGraph();
            foreach (var dataPoints in CommuterDataSet)
            {
                foreach (var point in dataPoints.Value)
                {
                    AddMarkerAt(point, GMarkerGoogleType.blue_dot, null);
                }
            }
            foreach (var dataPoints in PoolerDataSet)
            {
                foreach (var point in dataPoints.Value)
                {
                    poolerMarkers.Add(AddMarkerAt(point, GMarkerGoogleType.red_dot, null));
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

        private GMarkerGoogle AddMarkerAt(Coordinate cLoc, GMarkerGoogleType markerType, GMapOverlay overlay)
        {
            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(cLoc.Latitude, cLoc.Longitude),
              markerType);

            if (overlay == null)
                markersOverlay.Markers.Add(marker);
            else
                overlay.Markers.Add(marker);

            return marker;
        }

        private GMarkerGoogle AddMarkerAt(PointLatLng loc, GMarkerGoogleType markerType, GMapOverlay overlay)
        {
            GMarkerGoogle marker = new GMarkerGoogle(loc, markerType);
            if (overlay == null)
                markersOverlay.Markers.Add(marker);
            else
                overlay.Markers.Add(marker);

            return marker;
        }

        private void RefreshTrees()
        {
            var commuterRequestTable = TripRequestManager.GetAllCommuterRequests();
            if (OldCommuterCount != commuterRequestTable.Count)
            {
                RefreshTree(commutersTreeView, commuterRequestTable);
                //commuterTree.Text = "Commuter Tree : " + commuterRequestTable.Count;
                OldCommuterCount = commuterRequestTable.Count;
            }


            var poolersRequestTable = TripRequestManager.GetAllPoolerRequests();
            if (OldPoolerCount != poolersRequestTable.Count)
            {
                RefreshTree(poolersTreeView, poolersRequestTable);
                //poolerTree.Text = "Pooler Tree : " + poolersRequestTable.Count;
                OldPoolerCount = poolersRequestTable.Count;
            }

            commutersTreeView.ExpandAll();
            poolersTreeView.ExpandAll();
        }

        private void RefreshTree(TreeView tree, IDictionary<Coordinate, IList<TripRequest>> commuterRequestTable)
        {
            tree.Nodes.Clear();
            foreach (var requestPair in commuterRequestTable)
            {
                TreeNode rootNode = new TreeNode
                {
                    Text = requestPair.Key.Latitude + " " + requestPair.Key.Longitude
                };
                foreach (var request in requestPair.Value)
                {
                    Coordinate coordinate;

                    switch (request.GoingTo)
                    {
                        case GoingTo.Home:
                            coordinate = request.Commuter.OfficeCoordinate;
                            break;
                        case GoingTo.Office:
                            coordinate = request.Commuter.HomeCoordinate;
                            break;
                        default:
                            return;
                    }

                    var node = new TreeNode
                    {
                        Text = coordinate.Latitude + " " + coordinate.Longitude
                    };

                    rootNode.Nodes.Add(node);
                }

                if (!tree.InvokeRequired)
                {
                    tree.Nodes.Add(rootNode);
                    return;
                }

                tree.BeginInvoke((MethodInvoker) delegate
                {
                    tree.Nodes.Add(rootNode);
                });
            }
        }

       
        private void RefreshTree(TreeView tree, IList<Commuter> commuters)
        {
            IDictionary<Coordinate, IList<TripRequest>> requestTable = new Dictionary<Coordinate, IList<TripRequest>>();
            foreach (var commuter in commuters)
            {
                var keyPoint = PoolingMath.GetKeyPoint(commuter.HomeCoordinate);
                if (!requestTable.ContainsKey(keyPoint))
                    requestTable.Add(keyPoint, new List<TripRequest>());
                requestTable[keyPoint].Add(new TripRequest { Commuter = commuter });
            }

            RefreshTree(tree, requestTable);
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
            PoolingEngine.Instance.SetRequestCallBack(DisplayTrip);
            PoolingEngine.Instance.QueuePoolingRequest(null);
            UpdateStateDisplay();

            AddMockPoolersToTrip();
        }

        private async Task UpdateStateDisplay()
        {
            await Task.Factory.StartNew(() =>
            {
                //todo:remove this logic, implement some way to call back when processing is done for the commuter
                while (PoolingEngine.Instance.Status != EngineStatus.Completed)
                {
                    DisplayState();
                    Thread.Sleep(100);
                }
                DisplayState();

                //foreach (var request in CommutersRequest)
                //{
                //    ClearMap();

                //    DisplayTrip(request);
                //    Thread.Sleep(250);
                //}
            });
        }

        private async Task AddMockPoolersToTrip()
        {
            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < CommutersRequest.Count; i++)
                {
                    var request = CommutersRequest[i];
                    var waitingPoolers = PoolingEngine.Instance.GetPoolersToRequestForTrip(request);

                    if (waitingPoolers == null || waitingPoolers.Count == 0)
                    {
                        Thread.Sleep(100);
                        i--;
                        continue;
                    }

                    var poolerIndices = Enumerable.Range(0, waitingPoolers.Count);
                    PoolingEngine.Instance.AddPoolersToTrip(request, poolerIndices.ToArray());
                }
            });
        }

        private void ClearMap()
        {
            while (markersOverlay.Markers.Count > 0)
            {
                markersOverlay.Markers[0] = null;
                markersOverlay.Markers.RemoveAt(0);
            }
            //foreach (var marker in poolerMarkers)
            //{
            //    AddMarkerAt(marker.Position, GMarkerGoogleType.red_small, null);
            //}
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

        private void DisplayState()
        {
            var state = PoolingEngine.Instance.State;
            var totalCommuter = state.CommuterRequestProcessTable.Count;
            var totalProcessedCommuters = state.CommuterRequestProcessTable.Values.Where(x => x.Trip.Owner.Status == CommuterStatus.InTrip).Count();

            stateText.Text = "Total Commuters : " + totalCommuter + "\n" +
                "Total Processed Commuters : " + totalProcessedCommuters;
        }

        public void DisplayTrip(TripRequest request)
        {
            ClearMap();
            CheckForIllegalCrossThreadCalls = false;
            List<PointLatLng> points = ConvertCoordinatesToPoint(PoolingEngine.Instance.GetCommuterRoute(request));
            if (points.Count == 0)
                return;

            DisplayRoute(points, Color.Red, 3);
            AddMarkerAt(points.First(), GMarkerGoogleType.green_dot, null);
            AddMarkerAt(points.Last(), GMarkerGoogleType.purple_dot, null);

            //RefreshTree(poolersTreeView, PoolingEngine.GetPoolersInTrip(request));

            foreach (var pooler in PoolingEngine.Instance.GetPoolersInTrip(request))
            {
                AddMarkerAt(pooler.HomeCoordinate, GMarkerGoogleType.orange, passengerOverlay);
            }

            gMapControl1.Update();
            gMapControl1.Refresh();
        }

        private void DisplayRoute(List<PointLatLng> points, Color color, int size)
        {
            GMapRoute route = new GMapRoute(points, "A walk in the park");
            route.Stroke = new Pen(color, size);
            routesOverlay.Routes.Add(route);
        }

        private void DisplayGraphLine(List<PointLatLng> points, Color color, int size)
        {
            GMapRoute line = new GMapRoute(points, "A walk in the park");
            line.Stroke = new Pen(color, size);
            graphOverlay.Routes.Add(line);
        }

        private List<PointLatLng> ConvertCoordinatesToPoint(List<Coordinate> coordinates)
        {
            var points = new List<PointLatLng>();

            foreach (var coordinate in coordinates)
            {
                points.Add(new PointLatLng(coordinate.Latitude, coordinate.Longitude));
            }
            return points;
        }
    }
}
