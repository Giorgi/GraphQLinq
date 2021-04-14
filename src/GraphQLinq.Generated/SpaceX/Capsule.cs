namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class Capsule
    {
        public string Id { get; set; }
        public int? Landings { get; set; }
        public List<CapsuleMission> Missions { get; set; }
        public DateTime? Original_launch { get; set; }
        public int? Reuse_count { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public Dragon Dragon { get; set; }
    }
}