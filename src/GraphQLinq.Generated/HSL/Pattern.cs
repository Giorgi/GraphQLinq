namespace HSL
{
    using System.Collections.Generic;

    public partial class Pattern : Node
    {
        public string id { get; set; }
        public Route route { get; set; }
        public int directionId { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string headsign { get; set; }
        public List<Trip> trips { get; set; }
        public List<Trip> tripsForDate { get; set; }
        public List<Stop> stops { get; set; }
        public List<Coordinates> geometry { get; set; }
        public string semanticHash { get; set; }
        public List<Alert> alerts { get; set; }
    }
}