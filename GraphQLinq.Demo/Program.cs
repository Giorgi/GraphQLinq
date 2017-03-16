using System.Collections.Generic;
using System.Linq;

namespace GraphQLinq.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var locationTypes = new List<LocationType> { LocationType.STORE, LocationType.SERVICE };
            var graphQuery = new SuperChargersGraphContext("https://www.superchargers.io/graphql").Locations(type: locationTypes);//Near(30, -90);

            //var q = graphQuery.Select(location => new { c = location.locationType });
            var t = graphQuery.Include(location => location.Details("na", "desc"));
            //graphQuery.Select(location => new
            //{
            //    location.emails,
            //    location.salesPhone
            //}).Include(s => s.emails).ToList();
            graphQuery = graphQuery.Include(location => location.salesPhone).Include(location => location.emails);
            //var enumerator = q.ToList();

            var locations = t.ToList();
            var list = graphQuery.ToList();
        }
    }
}
