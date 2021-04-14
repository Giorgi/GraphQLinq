namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class MissionResult
    {
        public Result Result { get; set; }
        public List<Mission> Data { get; set; }
    }
}