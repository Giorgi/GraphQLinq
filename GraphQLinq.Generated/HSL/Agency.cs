namespace HSL
{
    using System.Collections.Generic;

    public partial class Agency : Node
    {
        public string id { get; set; }
        public string gtfsId { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string timezone { get; set; }
        public string lang { get; set; }
        public string phone { get; set; }
        public string fareUrl { get; set; }
        public List<Route> routes { get; set; }
        public List<Alert> alerts { get; set; }
    }
}