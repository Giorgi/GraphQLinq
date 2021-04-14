namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class PayloadsFind
    {
        public float? Apoapsis_km { get; set; }
        public string Customer { get; set; }
        public float? Eccentricity { get; set; }
        public DateTime? Epoch { get; set; }
        public float? Inclination_deg { get; set; }
        public float? Lifespan_years { get; set; }
        public float? Longitude { get; set; }
        public string Manufacturer { get; set; }
        public float? Mean_motion { get; set; }
        public string Nationality { get; set; }
        public int? Norad_id { get; set; }
        public string Orbit { get; set; }
        public string Payload_id { get; set; }
        public string Payload_type { get; set; }
        public float? Periapsis_km { get; set; }
        public float? Period_min { get; set; }
        public float? Raan { get; set; }
        public string Reference_system { get; set; }
        public string Regime { get; set; }
        public bool? Reused { get; set; }
        public float? Semi_major_axis_km { get; set; }
    }
}