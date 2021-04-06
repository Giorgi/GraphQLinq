namespace SpaceX
{
    using System;

    public partial class LaunchFind
    {
        public float? Apoapsis_km { get; set; }
        public int? Block { get; set; }
        public string Cap_serial { get; set; }
        public string Capsule_reuse { get; set; }
        public int? Core_flight { get; set; }
        public string Core_reuse { get; set; }
        public string Core_serial { get; set; }
        public string Customer { get; set; }
        public float? Eccentricity { get; set; }
        public DateTime? End { get; set; }
        public DateTime? Epoch { get; set; }
        public string Fairings_recovered { get; set; }
        public string Fairings_recovery_attempt { get; set; }
        public string Fairings_reuse { get; set; }
        public string Fairings_reused { get; set; }
        public string Fairings_ship { get; set; }
        public string Gridfins { get; set; }
        public string Id { get; set; }
        public float? Inclination_deg { get; set; }
        public string Land_success { get; set; }
        public string Landing_intent { get; set; }
        public string Landing_type { get; set; }
        public string Landing_vehicle { get; set; }
        public DateTime? Launch_date_local { get; set; }
        public DateTime? Launch_date_utc { get; set; }
        public string Launch_success { get; set; }
        public string Launch_year { get; set; }
        public string Legs { get; set; }
        public float? Lifespan_years { get; set; }
        public float? Longitude { get; set; }
        public string Manufacturer { get; set; }
        public float? Mean_motion { get; set; }
        public string Mission_id { get; set; }
        public string Mission_name { get; set; }
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
        public string Reused { get; set; }
        public string Rocket_id { get; set; }
        public string Rocket_name { get; set; }
        public string Rocket_type { get; set; }
        public string Second_stage_block { get; set; }
        public float? Semi_major_axis_km { get; set; }
        public string Ship { get; set; }
        public string Side_core1_reuse { get; set; }
        public string Side_core2_reuse { get; set; }
        public string Site_id { get; set; }
        public string Site_name_long { get; set; }
        public string Site_name { get; set; }
        public DateTime? Start { get; set; }
        public string Tbd { get; set; }
        public string Tentative_max_precision { get; set; }
        public string Tentative { get; set; }
    }
}