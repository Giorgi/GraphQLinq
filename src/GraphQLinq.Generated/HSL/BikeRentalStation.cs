namespace HSL
{
    using System.Collections.Generic;

    public partial class BikeRentalStation : Node, PlaceInterface
    {
        public string id { get; set; }
        public string stationId { get; set; }
        public string name { get; set; }
        public int bikesAvailable { get; set; }
        public int spacesAvailable { get; set; }
        public bool realtime { get; set; }
        public bool allowDropoff { get; set; }
        public List<string> networks { get; set; }
        public float lon { get; set; }
        public float lat { get; set; }
    }
}