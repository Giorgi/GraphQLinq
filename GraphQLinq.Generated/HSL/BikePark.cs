namespace HSL
{
    public partial class BikePark : Node, PlaceInterface
    {
        public string id { get; set; }
        public string bikeParkId { get; set; }
        public string name { get; set; }
        public int spacesAvailable { get; set; }
        public bool realtime { get; set; }
        public float lon { get; set; }
        public float lat { get; set; }
    }
}