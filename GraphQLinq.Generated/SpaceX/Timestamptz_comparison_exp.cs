namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class Timestamptz_comparison_exp
    {
        public DateTimeOffset? _eq { get; set; }
        public DateTimeOffset? _gt { get; set; }
        public DateTimeOffset? _gte { get; set; }
        public List<DateTimeOffset> _in { get; set; }
        public bool? _is_null { get; set; }
        public DateTimeOffset? _lt { get; set; }
        public DateTimeOffset? _lte { get; set; }
        public DateTimeOffset? _neq { get; set; }
        public List<DateTimeOffset> _nin { get; set; }
    }
}