namespace SpaceX
{
    public partial class Users_aggregate_order_by
    {
        public Order_by Count { get; set; }
        public Users_max_order_by Max { get; set; }
        public Users_min_order_by Min { get; set; }
    }
}