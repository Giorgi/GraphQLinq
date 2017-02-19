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
            var parameters = MethodBase.GetCurrentMethod().GetParameters();
            var parameterValues = new object[] { before, after, openSoon, isGallery, boundingBox, first, last, type, region, country };

            var dictionary = parameters.Zip(parameterValues, (info, value) => new { info.Name, Value = value }).ToDictionary(arg => arg.Name, arg => arg.Value);

            return BuildQuery(dictionary);
        }

        private GraphQuery<Location> BuildQuery(Dictionary<string, object> parameters, [CallerMemberName] string queryName = null)
        {
            return new GraphQuery<Location>(this, queryName) { Arguments = parameters };
        }
    }
}