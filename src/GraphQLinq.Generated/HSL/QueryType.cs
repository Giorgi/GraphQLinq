namespace HSL
{
    using System.Collections.Generic;

    public partial class QueryType
    {
        public Node node { get; set; }
        public List<Agency> agencies { get; set; }
        public Agency agency { get; set; }
        public List<Stop> stops { get; set; }
        public List<Stop> stopsByBbox { get; set; }
        public stopAtDistanceConnection stopsByRadius { get; set; }
        public placeAtDistanceConnection nearest { get; set; }
        public DepartureRow departureRow { get; set; }
        public Stop stop { get; set; }
        public Stop station { get; set; }
        public List<Stop> stations { get; set; }
        public List<Route> routes { get; set; }
        public Route route { get; set; }
        public List<Trip> trips { get; set; }
        public Trip trip { get; set; }
        public Trip fuzzyTrip { get; set; }
        public List<Pattern> patterns { get; set; }
        public Pattern pattern { get; set; }
        public List<Cluster> clusters { get; set; }
        public Cluster cluster { get; set; }
        public List<Alert> alerts { get; set; }
        public serviceTimeRange serviceTimeRange { get; set; }
        public List<BikeRentalStation> bikeRentalStations { get; set; }
        public BikeRentalStation bikeRentalStation { get; set; }
        public List<BikePark> bikeParks { get; set; }
        public BikePark bikePark { get; set; }
        public List<CarPark> carParks { get; set; }
        public CarPark carPark { get; set; }
        public QueryType viewer { get; set; }
        public Plan plan { get; set; }
    }
}