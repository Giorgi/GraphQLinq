namespace SpaceX
{
    using System.Collections.Generic;

    public partial class Users_aggregate
    {
        public Users_aggregate_fields Aggregate { get; set; }
        public List<Users> Nodes { get; set; }
    }
}