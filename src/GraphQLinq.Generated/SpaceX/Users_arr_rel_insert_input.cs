namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class Users_arr_rel_insert_input
    {
        public List<Users_insert_input> Data { get; set; }
        public Users_on_conflict On_conflict { get; set; }
    }
}