namespace SpaceX
{
    using System.Collections.Generic;

    public partial class Payload
    {
        public List<string> Customers { get; set; }
        public string Id { get; set; }
        public string Manufacturer { get; set; }
        public string Nationality { get; set; }
        public List<int> Norad_id { get; set; }
        public PayloadOrbitParams Orbit_params { get; set; }
        public string Orbit { get; set; }
        public float? Payload_mass_kg { get; set; }
        public float? Payload_mass_lbs { get; set; }
        public string Payload_type { get; set; }
        public bool? Reused { get; set; }
    }
}