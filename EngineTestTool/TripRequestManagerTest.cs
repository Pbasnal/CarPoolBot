using Bot.Data;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EngineTestTool
{
    public partial class TripRequestManagerTest : Form
    {
        int MIN = 0;
        int MAX = 5;

        int OldCommuterCount = 0;
        int OldPoolerCount = 0;

        static int commuterId = 0;
        Random random = new Random();

        public TripRequestManagerTest()
        {
            InitializeComponent();
        }

        private void addCommuterOfficeRequestButton_Click(object sender, EventArgs e)
        {
            var request = GetRandomRequest(GoingHow.Drive, GoingTo.Office);
            TripRequestManager.AddTripRequest(request);
            RefreshTrees();
        }

        private void addCommuterHomeRequestButton_Click(object sender, EventArgs e)
        {
            var request = GetRandomRequest(GoingHow.Drive, GoingTo.Home);
            TripRequestManager.AddTripRequest(request);
            RefreshTrees();
        }

        private void addPoolerOfficeRequestButton_Click(object sender, EventArgs e)
        {
            var request = GetRandomRequest(GoingHow.Pool, GoingTo.Office);
            TripRequestManager.AddTripRequest(request);
            RefreshTrees();
        }

        private void addPoolerHomeRequestButton_Click(object sender, EventArgs e)
        {
            var request = GetRandomRequest(GoingHow.Pool, GoingTo.Home);
            TripRequestManager.AddTripRequest(request);
            RefreshTrees();
        }

        private void RefreshTrees()
        {
            var commuterRequestTable = TripRequestManager.GetAllCommuterRequests();
            if (OldCommuterCount != commuterRequestTable.Count)
            {
                RefreshTree(commuters, commuterRequestTable);
                commuterTree.Text = "Commuter Tree : " + commuterRequestTable.Count;
                OldCommuterCount = commuterRequestTable.Count;
            }
            

            var poolersRequestTable = TripRequestManager.GetAllPoolerRequests();
            if (OldPoolerCount != poolersRequestTable.Count)
            {
                RefreshTree(poolers, poolersRequestTable);
                poolerTree.Text = "Pooler Tree : " + poolersRequestTable.Count;
                OldPoolerCount = poolersRequestTable.Count;
            }

            commuters.ExpandAll();
            poolers.ExpandAll();
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

                tree.Nodes.Add(rootNode);
            }
        }

        private TripRequest GetRandomRequest(GoingHow how, GoingTo to)
        {
            Commuter commuter = new Commuter
            {
                CommuterId = Guid.NewGuid().ToString(),
                CommuterName = "TestCommuter" + commuterId++,
                HomeCoordinate = new Coordinate
                {
                    Latitude = RandomDoubleBetween(MIN, MAX),
                    Longitude = RandomDoubleBetween(MIN, MAX)
                },
                OfficeCoordinate = new Coordinate
                {
                    Latitude = RandomDoubleBetween(MIN, MAX),
                    Longitude = RandomDoubleBetween(MIN, MAX)
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
                WaitTime = TimeSpan.FromMinutes(15)
            };
        }

        private double RandomDoubleBetween(double a, double b)
        {
            return a + random.NextDouble() * (b - a);
        }
    }
}
