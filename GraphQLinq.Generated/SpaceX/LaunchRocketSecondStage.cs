namespace SpaceX
{
    using System.Collections.Generic;

    public partial class LaunchRocketSecondStage
    {
        public int? Block { get; set; }
        public List<Payload> Payloads { get; set; }
    }
}