namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class String_comparison_exp
    {
        public string _eq { get; set; }
        public string _gt { get; set; }
        public string _gte { get; set; }
        public string _ilike { get; set; }
        public List<string> _in { get; set; }
        public bool? _is_null { get; set; }
        public string _like { get; set; }
        public string _lt { get; set; }
        public string _lte { get; set; }
        public string _neq { get; set; }
        public string _nilike { get; set; }
        public List<string> _nin { get; set; }
        public string _nlike { get; set; }
        public string _nsimilar { get; set; }
        public string _similar { get; set; }
    }
}