namespace HSL
{
    using System.Collections.Generic;

    public partial class Leg
    {
        public long startTime { get; set; }
        public long endTime { get; set; }
        public Mode mode { get; set; }
        public float duration { get; set; }
        public LegGeometry legGeometry { get; set; }
        public Agency agency { get; set; }
        public bool realTime { get; set; }
        public float distance { get; set; }
        public bool transitLeg { get; set; }
        public bool rentedBike { get; set; }
        public Place from { get; set; }
        public Place to { get; set; }
        public Route route { get; set; }
        public Trip trip { get; set; }
        public List<Stop> intermediateStops { get; set; }
        public bool intermediatePlace { get; set; }
    }
}