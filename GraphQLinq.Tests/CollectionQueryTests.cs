using System.Linq;
using HSL;
using NUnit.Framework;

namespace GraphQLinq.Tests
{
    [TestFixture]
    [Category("Collection query")]
    [Category("Integration tests")]
    class CollectionQueryTests
    {
        readonly HslGraphContext hslGraphContext = new HslGraphContext("https://api.digitransit.fi/routing/v1/routers/finland/index/graphql");

        [Test]
        public void SelectingNamesReturnsListOfNames()
        {
            var query = hslGraphContext.Stations().Select(l => l.name);

            var names = query.ToList();

            Assert.Multiple(() =>
            {
                CollectionAssert.IsNotEmpty(names);
                CollectionAssert.AllItemsAreNotNull(names);
            });
        }

        [Test]
        public void SelectingNamesDoesNotReturnStops()
        {
            var query = hslGraphContext.Stations();

            var stations = query.ToList();

            Assert.That(stations, Is.All.Matches<Stop>(l => l.stops == null));
        }
        
        [Test]
        public void SelectingNamesAndIncludingStopsReturnsStops()
        {
            var query = hslGraphContext.Stations().Include(s => s.stops);

            var stations = query.ToList();

            Assert.That(stations, Is.All.Matches<Stop>(s => s.stops != null));
        }

        [Test]
        public void SelectingNamesAndStopsReturnsStops()
        {
            var query = hslGraphContext.Stations().Select(location => new { location.name, location.stops });

            var stations = query.ToList();

            var stationsWithNullStops = stations.Where(s => s.stops == null).ToList();
            CollectionAssert.IsEmpty(stationsWithNullStops);
        }

        [Test]
        public void SelectingNamesWithAliasAndStopsReturnsStopsAndNames()
        {
            var query = hslGraphContext.Stations().Select(location => new { StationName = location.name, location.stops });

            var stations = query.ToList();

            var stationsWithNullStops = stations.Where(s => s.stops == null).ToList();
            var stationsWithNullCity = stations.Where(s => s.StationName == null).ToList();

            Assert.Multiple(() =>
            {
                CollectionAssert.IsEmpty(stationsWithNullStops);
                CollectionAssert.IsEmpty(stationsWithNullCity);
            });
        }
    }
}