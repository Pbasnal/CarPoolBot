using Bot.Data;
using Bot.Data.Models;
using Bot.Worker.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace EngineTestTool
{
    public partial class TestTool : Form
    {
        int MIN = 0;
        int MAX = 5;
        int CountIncrement = 500;
        static int commuterId = 0;

        Random random = new Random();
        IDictionary<Coordinate, IList<Coordinate>> CommuterDataSet;
        IDictionary<Coordinate, IList<Coordinate>> PoolerDataSet;

        EngineState state;

        public TestTool()
        {
            InitializeComponent();
            RefreshPlot();
        }

        private void PlotDataSet(int seriesIndex)
        {
            chart1.ChartAreas[0].Axes[0].Title = "N";
            chart1.ChartAreas[0].Axes[0].MajorGrid.Enabled = false;
            chart1.ChartAreas[0].Axes[0].Maximum = MAX + 1;

            chart1.ChartAreas[0].Axes[1].Title = "FIB(N)";
            chart1.ChartAreas[0].Axes[1].MajorGrid.Enabled = false;
            chart1.ChartAreas[0].Axes[1].Maximum = MAX + 1;

            chart1.Series[0].ChartType = SeriesChartType.Point;
            chart1.Series[0].MarkerStyle = MarkerStyle.Circle;
            chart1.Series[0].LegendText = "Points";


            foreach (var dataPoints in CommuterDataSet)
            {
                var xstripline = new StripLine();
                xstripline.Interval = 0;
                xstripline.IntervalOffset = dataPoints.Key.Latitude;
                xstripline.StripWidth = 0.01;
                xstripline.BackColor = Color.Gray;
                chart1.ChartAreas[0].AxisX.StripLines.Add(xstripline);

                var ystripline = new StripLine();
                ystripline.Interval = 0;
                ystripline.IntervalOffset = dataPoints.Key.Latitude;
                ystripline.StripWidth = 0.01;
                ystripline.BackColor = Color.Gray;
                chart1.ChartAreas[0].AxisY.StripLines.Add(ystripline);

                foreach (var point in dataPoints.Value)
                {
                    var chartPoint = new DataPoint(point.Latitude, point.Longitude);
                    chartPoint.Color = Color.Blue;
                    chart1.Series[0].Points.Add(chartPoint);
                }

                KeyList.Items.Add(dataPoints.Key.Latitude + ",  " + dataPoints.Key.Longitude);
            }
            foreach (var dataPoints in PoolerDataSet)
            {
                var xstripline = new StripLine();
                xstripline.Interval = 0;
                xstripline.IntervalOffset = dataPoints.Key.Latitude;
                xstripline.StripWidth = 0.01;
                xstripline.BackColor = Color.Gray;
                chart1.ChartAreas[0].AxisX.StripLines.Add(xstripline);

                var ystripline = new StripLine();
                ystripline.Interval = 0;
                ystripline.IntervalOffset = dataPoints.Key.Latitude;
                ystripline.StripWidth = 0.01;
                ystripline.BackColor = Color.Gray;
                chart1.ChartAreas[0].AxisY.StripLines.Add(ystripline);

                foreach (var point in dataPoints.Value)
                {
                    var chartPoint = new DataPoint(point.Latitude, point.Longitude);
                    chartPoint.Color = Color.Red;
                    chart1.Series[0].Points.Add(chartPoint);
                }

                KeyList.Items.Add(dataPoints.Key.Latitude + ",  " + dataPoints.Key.Longitude);
            }
        }

        private void GenerateRandomPoints()
        {
            CommuterDataSet = new Dictionary<Coordinate, IList<Coordinate>>();
            PoolerDataSet = new Dictionary<Coordinate, IList<Coordinate>>();

            TripRequestManager.ClearAllRequests();

            // generating random people who will drive
            for (int i = 0; i < 10; i++)
            {
                var request = GetRandomRequest(GoingHow.Drive, GoingTo.Office);
                TripRequestManager.AddTripRequest(request);

                var keyPoint = GetKeyPoint(request.Commuter.HomeCoordinate);

                if (!CommuterDataSet.Keys.Contains(keyPoint))
                    CommuterDataSet.Add(keyPoint, new List<Coordinate>());
                CommuterDataSet[keyPoint].Add(request.Commuter.HomeCoordinate);
            }

            // generating random people who will pool
            for (int i = 0; i < 100; i++)
            {
                var request = GetRandomRequest(GoingHow.Pool, GoingTo.Office);
                TripRequestManager.AddTripRequest(request);

                var keyPoint = GetKeyPoint(request.Commuter.HomeCoordinate);

                if (!PoolerDataSet.Keys.Contains(keyPoint))
                    PoolerDataSet.Add(keyPoint, new List<Coordinate>());
                PoolerDataSet[keyPoint].Add(request.Commuter.HomeCoordinate);
            }
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

        private void addCommuterOfficeRequestButton_Click(object sender, EventArgs e)
        {
            var request = GetRandomRequest(GoingHow.Drive, GoingTo.Office);
            TripRequestManager.AddTripRequest(request);
            RefreshPlot();
        }

        private void RefreshPlot()
        {
            GenerateRandomPoints();

            PlotDataSet(0);
        }

        private void addCommuterHomeRequestButton_Click(object sender, EventArgs e)
        {
            var request = GetRandomRequest(GoingHow.Drive, GoingTo.Home);
            TripRequestManager.AddTripRequest(request);
            RefreshPlot();
        }

        private void addPoolerOfficeRequestButton_Click(object sender, EventArgs e)
        {
            var request = GetRandomRequest(GoingHow.Pool, GoingTo.Office);
            TripRequestManager.AddTripRequest(request);
            RefreshPlot();
        }

        private void addPoolerHomeRequestButton_Click(object sender, EventArgs e)
        {
            var request = GetRandomRequest(GoingHow.Pool, GoingTo.Home);
            TripRequestManager.AddTripRequest(request);
            RefreshPlot();
        }

        private TripRequest GetRandomRequest(GoingHow how, GoingTo to)
        {
            Commuter commuter = new Commuter
            {
                CommuterId = Guid.NewGuid(),
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
    }
}
