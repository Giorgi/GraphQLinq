namespace SpaceX
{
    public partial class DragonThrust
    {
        public int? Amount { get; set; }
        public string Fuel_1 { get; set; }
        public string Fuel_2 { get; set; }
        public int? Pods { get; set; }
        public Force Thrust { get; set; }
        public string Type { get; set; }
    }
}