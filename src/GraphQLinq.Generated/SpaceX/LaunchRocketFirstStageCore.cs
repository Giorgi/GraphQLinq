namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class LaunchRocketFirstStageCore
    {
        public int? Block { get; set; }
        public Core Core { get; set; }
        public int? Flight { get; set; }
        public bool? Gridfins { get; set; }
        public bool? Land_success { get; set; }
        public bool? Landing_intent { get; set; }
        public string Landing_type { get; set; }
        public string Landing_vehicle { get; set; }
        public bool? Legs { get; set; }
        public bool? Reused { get; set; }
    }
}