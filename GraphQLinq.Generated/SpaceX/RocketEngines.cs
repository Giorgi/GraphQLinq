namespace SpaceX
{
    public partial class RocketEngines
    {
        public int? Number { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        public string Layout { get; set; }
        public string Engine_loss_max { get; set; }
        public string Propellant_1 { get; set; }
        public string Propellant_2 { get; set; }
        public Force Thrust_sea_level { get; set; }
        public Force Thrust_vacuum { get; set; }
        public float? Thrust_to_weight { get; set; }
    }
}