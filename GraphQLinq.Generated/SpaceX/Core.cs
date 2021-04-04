namespace SpaceX
{
    using System.Collections.Generic;
    using System;

    public partial class Core
    {
        public int? Asds_attempts { get; set; }
        public int? Asds_landings { get; set; }
        public int? Block { get; set; }
        public string Id { get; set; }
        public List<CapsuleMission> Missions { get; set; }
        public DateTime? Original_launch { get; set; }
        public int? Reuse_count { get; set; }
        public int? Rtls_attempts { get; set; }
        public int? Rtls_landings { get; set; }
        public string Status { get; set; }
        public bool? Water_landing { get; set; }
    }
}