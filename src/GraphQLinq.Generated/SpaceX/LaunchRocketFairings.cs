namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class LaunchRocketFairings
    {
        public bool? Recovered { get; set; }
        public bool? Recovery_attempt { get; set; }
        public bool? Reused { get; set; }
        public string Ship { get; set; }
    }
}