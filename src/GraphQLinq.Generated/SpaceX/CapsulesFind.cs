namespace SpaceX
{
    using System;

    public partial class CapsulesFind
    {
        public string Id { get; set; }
        public int? Landings { get; set; }
        public string Mission { get; set; }
        public DateTime? Original_launch { get; set; }
        public int? Reuse_count { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
    }
}