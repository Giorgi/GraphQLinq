namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class Ship
    {
        public int? Abs { get; set; }
        public bool? Active { get; set; }
        public int? Attempted_landings { get; set; }
        public int? Class { get; set; }
        public int? Course_deg { get; set; }
        public string Home_port { get; set; }
        public string Id { get; set; }
        public string Image { get; set; }
        public int? Imo { get; set; }
        public List<ShipMission> Missions { get; set; }
        public int? Mmsi { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public ShipLocation Position { get; set; }
        public List<string> Roles { get; set; }
        public float? Speed_kn { get; set; }
        public string Status { get; set; }
        public int? Successful_landings { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public int? Weight_kg { get; set; }
        public int? Weight_lbs { get; set; }
        public int? Year_built { get; set; }
    }
}