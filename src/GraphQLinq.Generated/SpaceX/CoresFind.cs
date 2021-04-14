namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class CoresFind
    {
        public int? Asds_attempts { get; set; }
        public int? Asds_landings { get; set; }
        public int? Block { get; set; }
        public string Id { get; set; }
        public string Missions { get; set; }
        public DateTime? Original_launch { get; set; }
        public int? Reuse_count { get; set; }
        public int? Rtls_attempts { get; set; }
        public int? Rtls_landings { get; set; }
        public string Status { get; set; }
        public bool? Water_landing { get; set; }
    }
}