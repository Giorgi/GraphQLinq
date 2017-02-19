using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GraphQLinq
{
    public class GraphContext
    {
        public string BaseUrl { get; set; }

        public GraphContext(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

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

        private GraphQuery<T> BuildQuery<T>(object[] parameterValues, [CallerMemberName] string queryName = null)
        {
            var parameters = GetType().GetMethod(queryName).GetParameters();
            var arguments = parameters.Zip(parameterValues, (info, value) => new { info.Name, Value = value }).ToDictionary(arg => arg.Name, arg => arg.Value);

            return new GraphQuery<T>(this, queryName) { Arguments = arguments };
        }
    }
}