namespace HSL
{
    using System.Collections.Generic;

    public partial class Cluster : Node
    {
        public string id { get; set; }
        public string gtfsId { get; set; }
        public string name { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public List<Stop> stops { get; set; }
    }
}