using System.ComponentModel.DataAnnotations.Schema;

namespace Bot.Data.EfModels
{
    [ComplexType]
    public class EfCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public EfCoordinate()
        { }

        public EfCoordinate(Coordinate coordinate)
        {
            Latitude = coordinate.Latitude;
            Longitude = coordinate.Longitude;
        }

        public static explicit operator EfCoordinate(Coordinate coordinate)  // explicit byte to digit conversion operator
        {
            return new EfCoordinate(coordinate);
        }

        public Coordinate GetCoordinate()
        {
            return new Coordinate(Latitude, Longitude);
        }
    }
}
