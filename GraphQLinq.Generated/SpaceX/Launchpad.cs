namespace SpaceX
{
    using System.Collections.Generic;

    public partial class Launchpad
    {
        public int? Attempted_launches { get; set; }
        public string Details { get; set; }
        public string Id { get; set; }
        public Location Location { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int? Successful_launches { get; set; }
        public List<Rocket> Vehicles_launched { get; set; }
        public string Wikipedia { get; set; }
    }
}