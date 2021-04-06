namespace SpaceX
{
    using System.Collections.Generic;

    public partial class ShipsResult
    {
        public Result Result { get; set; }
        public List<Ship> Data { get; set; }
    }
}