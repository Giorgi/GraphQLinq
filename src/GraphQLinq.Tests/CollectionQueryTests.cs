using System.Linq;
using System.Threading.Tasks;
using GraphQLinq.Tests.Tools;
using HSL;
using NUnit.Framework;

namespace GraphQLinq.Tests
{
    [TestFixture]
    [Category("Collection query")]
    [Category("Integration tests")]
    class CollectionQueryTests
    {
        readonly HslGraphContext hslGraphContext = new HslGraphContext(HttpClientHelper.Create("https://api.digitransit.fi/routing/v1/routers/finland/index/graphql"));

        [Test]
        public async Task SelectingNamesReturnsListOfNames()
        {
            var query = hslGraphContext.Stations().Select(l => l.name);

            var names = await query.ToEnumerable();

            Assert.Multiple(() =>
            {
                CollectionAssert.IsNotEmpty(names);
                CollectionAssert.AllItemsAreNotNull(names);
            });
        }

        [Test]
        public async Task SelectingNamesDoesNotReturnStops()
        {
            var query = hslGraphContext.Stations();

            var stations = await query.ToEnumerable();

            Assert.That(stations, Is.All.Matches<Stop>(l => l.stops == null));
        }
        
        [Test]
        public async Task SelectingNamesAndIncludingStopsReturnsStops()
        {
            var query = hslGraphContext.Stations().Include(s => s.stops);

            var stations = await query.ToEnumerable();

            Assert.That(stations, Is.All.Matches<Stop>(s => s.stops != null));
        }

        [Test]
        public async Task SelectingNamesAndStopsReturnsStops()
        {
            var query = hslGraphContext.Stations().Select(location => new { location.name, location.stops });

            var stations = await query.ToEnumerable();

            var stationsWithNullStops = stations.Where(s => s.stops == null).ToList();
            CollectionAssert.IsEmpty(stationsWithNullStops);
        }

        [Test]
        public async Task SelectingNamesWithAliasAndStopsReturnsStopsAndNames()
        {
            var query = hslGraphContext.Stations().Select(location => new { StationName = location.name, location.stops });

            var stations = await query.ToEnumerable();

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