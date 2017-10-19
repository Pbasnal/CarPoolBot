using System;

namespace Bot.Data
{
    public static class PoolingMath
    {
        private static int _increment = 3;
        private static int _digitsAfterDecimal = 3;

        public static Coordinate GetKeyPoint(Coordinate point)
        {
            var xWithoutDecimal = point.Latitude * Math.Pow(10, _digitsAfterDecimal);
            var yWithoutDecimal = point.Longitude * Math.Pow(10, _digitsAfterDecimal);

            point.Latitude = (xWithoutDecimal + _increment - (xWithoutDecimal % _increment)) / Math.Pow(10, _digitsAfterDecimal);
            point.Longitude = (yWithoutDecimal + _increment - (yWithoutDecimal % _increment)) / Math.Pow(10, _digitsAfterDecimal);

            return point;
        }

        public static double GetIncrementAmount()
        {
            return 3 / Math.Pow(10, _digitsAfterDecimal);
        }
    }
}
