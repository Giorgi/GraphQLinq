namespace HSL
{
    using System.Collections.Generic;

    public partial class Stop : Node, PlaceInterface
    {
        public string id { get; set; }
        public List<Stoptime> stopTimesForPattern { get; set; }
        public string gtfsId { get; set; }
        public string name { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public string code { get; set; }
        public string desc { get; set; }
        public string zoneId { get; set; }
        public string url { get; set; }
        public LocationType locationType { get; set; }
        public Stop parentStation { get; set; }
        public WheelchairBoarding wheelchairBoarding { get; set; }
        public string direction { get; set; }
        public string timezone { get; set; }
        public int vehicleType { get; set; }
        public string platformCode { get; set; }
        public Cluster cluster { get; set; }
        public List<Stop> stops { get; set; }
        public List<Route> routes { get; set; }
        public List<Pattern> patterns { get; set; }
        public List<stopAtDistance> transfers { get; set; }
        public List<StoptimesInPattern> stoptimesForServiceDate { get; set; }
        public List<StoptimesInPattern> stoptimesForPatterns { get; set; }
        public List<Stoptime> stoptimesWithoutPatterns { get; set; }
    }
}