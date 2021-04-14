namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class LaunchRocket
    {
        public LaunchRocketFairings Fairings { get; set; }
        public LaunchRocketFirstStage First_stage { get; set; }
        public string Rocket_name { get; set; }
        public string Rocket_type { get; set; }
        public Rocket Rocket { get; set; }
        public LaunchRocketSecondStage Second_stage { get; set; }
    }
}