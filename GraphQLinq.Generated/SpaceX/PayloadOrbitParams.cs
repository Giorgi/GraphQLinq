namespace SpaceX
{
    using System;

    public partial class PayloadOrbitParams
    {
        public float? Apoapsis_km { get; set; }
        public float? Arg_of_pericenter { get; set; }
        public float? Eccentricity { get; set; }
        public DateTime? Epoch { get; set; }
        public float? Inclination_deg { get; set; }
        public float? Lifespan_years { get; set; }
        public float? Longitude { get; set; }
        public float? Mean_anomaly { get; set; }
        public float? Mean_motion { get; set; }
        public float? Periapsis_km { get; set; }
        public float? Period_min { get; set; }
        public float? Raan { get; set; }
        public string Reference_system { get; set; }
        public string Regime { get; set; }
        public float? Semi_major_axis_km { get; set; }
    }
}