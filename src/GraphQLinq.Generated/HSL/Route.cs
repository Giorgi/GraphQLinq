namespace HSL
{
    using System.Collections.Generic;

    public partial class Route : Node
    {
        public string id { get; set; }
        public string gtfsId { get; set; }
        public Agency agency { get; set; }
        public string shortName { get; set; }
        public string longName { get; set; }
        public string mode { get; set; }
        public string desc { get; set; }
        public string url { get; set; }
        public string color { get; set; }
        public string textColor { get; set; }
        public BikesAllowed bikesAllowed { get; set; }
        public List<Pattern> patterns { get; set; }
        public List<Stop> stops { get; set; }
        public List<Trip> trips { get; set; }
        public List<Alert> alerts { get; set; }
    }
}