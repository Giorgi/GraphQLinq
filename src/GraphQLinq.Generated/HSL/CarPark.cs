namespace HSL
{
    public partial class CarPark : Node, PlaceInterface
    {
        public string id { get; set; }
        public string carParkId { get; set; }
        public string name { get; set; }
        public int maxCapacity { get; set; }
        public int spacesAvailable { get; set; }
        public bool realtime { get; set; }
        public float lon { get; set; }
        public float lat { get; set; }
    }
}