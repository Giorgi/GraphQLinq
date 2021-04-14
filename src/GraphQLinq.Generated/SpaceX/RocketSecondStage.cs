namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class RocketSecondStage
    {
        public int? Burn_time_sec { get; set; }
        public int? Engines { get; set; }
        public float? Fuel_amount_tons { get; set; }
        public RocketSecondStagePayloads Payloads { get; set; }
        public Force Thrust { get; set; }
    }
}