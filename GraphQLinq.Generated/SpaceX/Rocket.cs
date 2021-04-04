namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class Rocket
    {
        public bool? Active { get; set; }
        public int? Boosters { get; set; }
        public string Company { get; set; }
        public int? Cost_per_launch { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public Distance Diameter { get; set; }
        public RocketEngines Engines { get; set; }
        public DateTime? First_flight { get; set; }
        public RocketFirstStage First_stage { get; set; }
        public Distance Height { get; set; }
        public string Id { get; set; }
        public RocketLandingLegs Landing_legs { get; set; }
        public Mass Mass { get; set; }
        public string Name { get; set; }
        public List<RocketPayloadWeight> Payload_weights { get; set; }
        public RocketSecondStage Second_stage { get; set; }
        public int? Stages { get; set; }
        public int? Success_rate_pct { get; set; }
        public string Type { get; set; }
        public string Wikipedia { get; set; }
    }
}