namespace SpaceX
{
    using System.Collections.Generic;

    public partial class Users_on_conflict
    {
        public Users_constraint Constraint { get; set; }
        public List<Users_update_column> Update_columns { get; set; }
    }
}