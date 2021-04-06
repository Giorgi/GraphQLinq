namespace HSL
{
    public partial class Stoptime
    {
        public Stop stop { get; set; }
        public int scheduledArrival { get; set; }
        public int realtimeArrival { get; set; }
        public int arrivalDelay { get; set; }
        public int scheduledDeparture { get; set; }
        public int realtimeDeparture { get; set; }
        public int departureDelay { get; set; }
        public bool timepoint { get; set; }
        public bool realtime { get; set; }
        public RealtimeState realtimeState { get; set; }
        public PickupDropoffType pickupType { get; set; }
        public PickupDropoffType dropoffType { get; set; }
        public long serviceDay { get; set; }
        public Trip trip { get; set; }
        public string headsign { get; set; }
    }
}