namespace SpaceX
{
    using System.Collections.Generic;

    public partial class Dragon
    {
        public bool? Active { get; set; }
        public int? Crew_capacity { get; set; }
        public string Description { get; set; }
        public Distance Diameter { get; set; }
        public int? Dry_mass_kg { get; set; }
        public int? Dry_mass_lb { get; set; }
        public string First_flight { get; set; }
        public DragonHeatShield Heat_shield { get; set; }
        public Distance Height_w_trunk { get; set; }
        public string Id { get; set; }
        public Mass Launch_payload_mass { get; set; }
        public Volume Launch_payload_vol { get; set; }
        public string Name { get; set; }
        public int? Orbit_duration_yr { get; set; }
        public DragonPressurizedCapsule Pressurized_capsule { get; set; }
        public Mass Return_payload_mass { get; set; }
        public Volume Return_payload_vol { get; set; }
        public float? Sidewall_angle_deg { get; set; }
        public List<DragonThrust> Thrusters { get; set; }
        public DragonTrunk Trunk { get; set; }
        public string Type { get; set; }
        public string Wikipedia { get; set; }
    }
}