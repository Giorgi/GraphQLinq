namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class RocketsResult
    {
        public Result Result { get; set; }
        public List<Rocket> Data { get; set; }
    }
}