namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class Users_mutation_response
    {
        public int Affected_rows { get; set; }
        public List<Users> Returning { get; set; }
    }
}