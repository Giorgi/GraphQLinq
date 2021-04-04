namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class Uuid_comparison_exp
    {
        public Guid? _eq { get; set; }
        public Guid? _gt { get; set; }
        public Guid? _gte { get; set; }
        public List<Guid> _in { get; set; }
        public bool? _is_null { get; set; }
        public Guid? _lt { get; set; }
        public Guid? _lte { get; set; }
        public Guid? _neq { get; set; }
        public List<Guid> _nin { get; set; }
    }
}