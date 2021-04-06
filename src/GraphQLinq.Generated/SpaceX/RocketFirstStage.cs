namespace SpaceX
{
    public partial class RocketFirstStage
    {
        public int? Burn_time_sec { get; set; }
        public int? Engines { get; set; }
        public float? Fuel_amount_tons { get; set; }
        public bool? Reusable { get; set; }
        public Force Thrust_sea_level { get; set; }
        public Force Thrust_vacuum { get; set; }
    }
}