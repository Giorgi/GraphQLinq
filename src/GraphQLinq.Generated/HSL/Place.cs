namespace HSL
{
    public partial class Place
    {
        public string name { get; set; }
        public VertexType vertexType { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public Stop stop { get; set; }
        public BikeRentalStation bikeRentalStation { get; set; }
        public BikePark bikePark { get; set; }
        public CarPark carPark { get; set; }
    }
}