namespace HSL
{
    using System.Collections.Generic;

    public partial class Trip : Node
    {
        public string id { get; set; }
        public string gtfsId { get; set; }
        public Route route { get; set; }
        public string serviceId { get; set; }
        public List<string> activeDates { get; set; }
        public string tripShortName { get; set; }
        public string tripHeadsign { get; set; }
        public string routeShortName { get; set; }
        public string directionId { get; set; }
        public string blockId { get; set; }
        public string shapeId { get; set; }
        public WheelchairBoarding wheelchairAccessible { get; set; }
        public BikesAllowed bikesAllowed { get; set; }
        public Pattern pattern { get; set; }
        public List<Stop> stops { get; set; }
        public List<Stop> semanticHash { get; set; }
        public List<Stoptime> stoptimes { get; set; }
        public List<Stoptime> stoptimesForDate { get; set; }
        public List<List<float>> geometry { get; set; }
        public List<Alert> alerts { get; set; }
    }
}