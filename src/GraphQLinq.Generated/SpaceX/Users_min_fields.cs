namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class Users_min_fields
    {
        public string Name { get; set; }
        public string Rocket { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string Twitter { get; set; }
    }
}