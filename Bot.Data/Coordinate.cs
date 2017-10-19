namespace Bot.Data
{
    public struct Coordinate
    {
        public double Latitude;
        public double Longitude;

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + (int)Latitude;
            hash = hash * 31 + (int)Longitude;
            return hash;
        }

        public override bool Equals(object other)
        {
            return other is Coordinate ? Equals((Coordinate)other) : false;
        }

        public bool Equals(Coordinate other)
        {
            return Latitude == other.Latitude &&
                   Longitude == other.Longitude;
        }
    }
}
