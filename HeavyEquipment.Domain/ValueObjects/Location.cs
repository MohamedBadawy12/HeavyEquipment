namespace HeavyEquipment.Domain.ValueObjects
{
    public class Location
    {
        public string City { get; }
        public string Address { get; }
        public double Latitude { get; }
        public double Longitude { get; }


        public Location(string city, string address, double latitude, double longitude)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("المدينة مطلوبة");
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("خط العرض غير صحيح");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("خط الطول غير صحيح");

            City = city;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// حساب المسافة بين موقعين بالكيلومتر (Haversine Formula)
        /// </summary>
        public double DistanceTo(Location other)
        {
            const double R = 6371;
            var dLat = ToRad(other.Latitude - Latitude);
            var dLon = ToRad(other.Longitude - Longitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(Latitude)) * Math.Cos(ToRad(other.Latitude)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static double ToRad(double deg) => deg * Math.PI / 180;

        public override string ToString() => $"{Address}, {City}";

        public override bool Equals(object? obj)
            => obj is Location other && Latitude == other.Latitude && Longitude == other.Longitude;

        public override int GetHashCode()
            => HashCode.Combine(Latitude, Longitude);
    }
}
