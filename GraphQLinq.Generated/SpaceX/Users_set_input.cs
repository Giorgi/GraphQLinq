namespace SpaceX
{
    using System;

    public partial class Users_set_input
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Rocket { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string Twitter { get; set; }
    }
}