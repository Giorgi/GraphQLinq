namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class Roadster
    {
        public float? Apoapsis_au { get; set; }
        public string Details { get; set; }
        public float? Earth_distance_km { get; set; }
        public float? Earth_distance_mi { get; set; }
        public float? Eccentricity { get; set; }
        public float? Epoch_jd { get; set; }
        public float? Inclination { get; set; }
        public DateTime? Launch_date_unix { get; set; }
        public DateTime? Launch_date_utc { get; set; }
        public int? Launch_mass_kg { get; set; }
        public int? Launch_mass_lbs { get; set; }
        public float? Longitude { get; set; }
        public float? Mars_distance_km { get; set; }
        public float? Mars_distance_mi { get; set; }
        public string Name { get; set; }
        public int? Norad_id { get; set; }
        public float? Orbit_type { get; set; }
        public float? Periapsis_arg { get; set; }
        public float? Periapsis_au { get; set; }
        public float? Period_days { get; set; }
        public float? Semi_major_axis_au { get; set; }
        public float? Speed_kph { get; set; }
        public float? Speed_mph { get; set; }
        public string Wikipedia { get; set; }
    }
}