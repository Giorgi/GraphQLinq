namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class RocketPayloadWeight
    {
        public string Id { get; set; }
        public int? Kg { get; set; }
        public int? Lb { get; set; }
        public string Name { get; set; }
    }
}