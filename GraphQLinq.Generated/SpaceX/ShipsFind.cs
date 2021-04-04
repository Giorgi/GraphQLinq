namespace SpaceX
{
    public partial class ShipsFind
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Role { get; set; }
        public bool? Active { get; set; }
        public int? Imo { get; set; }
        public int? Mmsi { get; set; }
        public int? Abs { get; set; }
        public int? Class { get; set; }
        public int? Weight_lbs { get; set; }
        public int? Weight_kg { get; set; }
        public int? Year_built { get; set; }
        public string Home_port { get; set; }
        public string Status { get; set; }
        public int? Speed_kn { get; set; }
        public int? Course_deg { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public int? Successful_landings { get; set; }
        public int? Attempted_landings { get; set; }
        public string Mission { get; set; }
    }
}