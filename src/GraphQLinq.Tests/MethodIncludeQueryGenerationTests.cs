using System.Linq;
using HSL;
using NUnit.Framework;

namespace GraphQLinq.Tests
{
    [TestFixture(Category = "Query generation tests")]
    public class MethodIncludeQueryGenerationTests
    {
        HslGraphContext hslGraphContext = new HslGraphContext("http://example.com");


        [Test]
        public void IncludingNavigationMethodQueryIncludesIncludedNavigationMethodWithNestedProperties()
        {
            var agencyQuery = hslGraphContext.Agency("248798")
                .Include(agency => agency.routes.Select(route => route.trips.Select(trip => trip.stoptimesForDate("20170427"))));

            Assert.That(agencyQuery.Query, Does.Contain("stoptimesForDate")
                                           .And.Contains(nameof(Stoptime.scheduledArrival))
                                           .And.Contains(nameof(Stoptime.realtimeArrival)));
        }

        [Test]
        public void IncludingNavigationMethodQueryIncludesIncludedNavigationMethodParameter()
        {
            var agencyQuery = hslGraphContext.Agency("248798")
                .Include(agency => agency.routes.Select(route => route.trips.Select(trip => trip.stoptimesForDate("20170427"))));

            Assert.That(agencyQuery.Query, Does.Contain("$serviceDay1: String!").And.Contains("stoptimesForDate(serviceDay: $serviceDay1)"));
        }

        [Test]
        public void IncludingNavigationMethodQueryVariablesIncludesIncludedNavigationMethodArgument()
        {
            var agencyQuery = hslGraphContext.Agency("248798")
                .Include(agency => agency.routes.Select(route => route.trips.Select(trip => trip.stoptimesForDate("20170427"))));

            Assert.That(agencyQuery.QueryVariables, Does.ContainKey("serviceDay1").With.ContainValue("20170427"));
        }
    }
}