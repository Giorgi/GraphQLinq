namespace SpaceX
{
    using System.Collections.Generic;

    public partial class Users_bool_exp
    {
        public List<Users_bool_exp> _and { get; set; }
        public Users_bool_exp _not { get; set; }
        public List<Users_bool_exp> _or { get; set; }
        public Uuid_comparison_exp Id { get; set; }
        public String_comparison_exp Name { get; set; }
        public String_comparison_exp Rocket { get; set; }
        public Timestamptz_comparison_exp Timestamp { get; set; }
        public String_comparison_exp Twitter { get; set; }
    }
}