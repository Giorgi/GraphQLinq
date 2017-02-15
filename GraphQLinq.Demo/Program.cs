using System.Linq;

namespace GraphQLinq.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var graphQuery = new GraphContext("https://www.superchargers.io/graphql").Locations();

            //var q = graphQuery.Select(location => new { c = location.city, t = location.emails.Select(email => email.email) });
            var t = graphQuery.Select(l => l.city);

            //var locations = q.ToList();
            var list = t.ToList();
        }
    }
}
