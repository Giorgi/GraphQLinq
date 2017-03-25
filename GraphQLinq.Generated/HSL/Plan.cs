namespace HSL
{
    using System.Collections.Generic;

    public partial class Plan
    {
        public long date { get; set; }
        public Place from { get; set; }
        public Place to { get; set; }
        public List<Itinerary> itineraries { get; set; }
        public List<string> messageEnums { get; set; }
        public List<string> messageStrings { get; set; }
        public debugOutput debugOutput { get; set; }
    }
}