namespace SpaceX
{
    using System.Collections.Generic;

    public partial class LaunchesPastResult
    {
        public Result Result { get; set; }
        public List<Launch> Data { get; set; }
    }
}