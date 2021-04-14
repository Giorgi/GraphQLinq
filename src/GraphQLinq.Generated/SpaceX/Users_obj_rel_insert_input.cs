namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class Users_obj_rel_insert_input
    {
        public Users_insert_input Data { get; set; }
        public Users_on_conflict On_conflict { get; set; }
    }
}