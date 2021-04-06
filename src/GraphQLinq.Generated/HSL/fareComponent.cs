namespace HSL
{
    using System.Collections.Generic;

    public partial class fareComponent
    {
        public string fareId { get; set; }
        public string currency { get; set; }
        public int cents { get; set; }
        public List<Route> routes { get; set; }
    }
}