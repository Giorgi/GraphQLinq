namespace HSL
{
    using System.Collections.Generic;

    public partial class Itinerary
    {
        public long startTime { get; set; }
        public long endTime { get; set; }
        public long duration { get; set; }
        public long waitingTime { get; set; }
        public long walkTime { get; set; }
        public float walkDistance { get; set; }
        public List<Leg> legs { get; set; }
        public List<fare> fares { get; set; }
    }
}