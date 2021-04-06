namespace HSL
{
    using System.Collections.Generic;

    public partial class DepartureRow : Node, PlaceInterface
    {
        public string id { get; set; }
        public Stop stop { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public Pattern pattern { get; set; }
        public List<Stoptime> stoptimes { get; set; }
    }
}