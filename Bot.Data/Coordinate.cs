using Bot.Data.EfModels;
using Bot.Data.Models;
using System;

namespace Bot.Data
{
    public struct Coordinate
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public Coordinate(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + (int)Latitude;
            hash = hash * 31 + (int)Longitude;
            return hash;
        }

        public override bool Equals(object other)
        {
            return other is Coordinate ? Equals(other) : false;
        }

        public bool Equals(Coordinate other)
        {
            return Latitude == other.Latitude &&
                   Longitude == other.Longitude;
        }

        public static explicit operator Coordinate(EfCoordinate efcoordinate)  // explicit byte to digit conversion operator
        {
            return new Coordinate(efcoordinate.Latitude, efcoordinate.Longitude);
        }
    }
}
