namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class HistoriesResult
    {
        public Result Result { get; set; }
        public List<History> Data { get; set; }
    }
}