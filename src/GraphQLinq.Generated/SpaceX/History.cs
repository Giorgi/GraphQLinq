namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class History
    {
        public string Details { get; set; }
        public DateTime? Event_date_unix { get; set; }
        public DateTime? Event_date_utc { get; set; }
        public string Id { get; set; }
        public Link Links { get; set; }
        public string Title { get; set; }
        public Launch Flight { get; set; }
    }
}