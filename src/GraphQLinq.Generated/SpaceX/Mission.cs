namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class Mission
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public List<string> Manufacturers { get; set; }
        public string Name { get; set; }
        public string Twitter { get; set; }
        public string Website { get; set; }
        public string Wikipedia { get; set; }
        public List<Payload> Payloads { get; set; }
    }
}