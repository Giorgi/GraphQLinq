using System.Collections.Generic;

namespace GraphQLinq
{
    public class SuperChargersGraphContext : GraphContext
    {
        public SuperChargersGraphContext(string baseUrl) : base(baseUrl, "") { }
        public SuperChargersGraphContext(string baseUrl, string authorization) : base(baseUrl, authorization) { }

        public GraphQuery<Location> Locations(string before = null, string after = null, bool? openSoon = null, bool? isGallery = null, float? boundingBox = null,
            int? first = null, int? last = null, List<LocationType> type = null, Region? region = null, Country? country = null)
        {
            var parameterValues = new object[] { before, after, openSoon, isGallery, boundingBox, first, last, type, region, country };

            return BuildQuery<Location>(parameterValues);
        }

        public GraphQuery<Location> Near(float latitude, float longitude, int? first = null, int? last = null, List<LocationType> type = null, string before = null, string after = null)
        {
            var parameterValues = new object[] { latitude, longitude, first, last, type, before, after };

            return BuildQuery<Location>(parameterValues);
        }
    }
}