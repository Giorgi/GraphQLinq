using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GraphQLinq.Tests
{
    [Ignore("www.superchargers.io is down")]
    [TestFixture(Category = "Integration tests")]
    class SuperChargersTests
    {
        readonly List<LocationType> locationTypes = new List<LocationType> { LocationType.STORE, LocationType.SERVICE };
        readonly SuperChargersGraphContext superChargersContext = new SuperChargersGraphContext("https://www.superchargers.io/graphql");

        [Test]
        public async Task SelectingCitiesReturnsListOfCities()
        {   
            var query = superChargersContext.Locations(type: locationTypes).Select(l => l.city);

            var locations = await query.ToArray();

            CollectionAssert.IsNotEmpty(locations);
            CollectionAssert.AllItemsAreNotNull(locations);
        }

        [Test]
        public async Task SelectingLocationsDoesNotReturnPhones()
        {
            var query = superChargersContext.Locations(type: locationTypes);

            var locations = await query.ToArray();
            Assert.That(locations, Is.All.Matches<Location>(l => l.salesPhone == null));
        }

        [Test]
        public async Task SelectingLocationsAndIncludingPhonesReturnsPhones()
        {
            var query = superChargersContext.Locations(type: locationTypes).Include(location => location.salesPhone);

            var locations = await query.ToArray();

            Assert.That(locations, Is.All.Matches<Location>(l => l.salesPhone != null));
        }

        [Test]
        public async Task SelectingCitiesAndPhonesReturnsPhones()
        {
            var query = superChargersContext.Locations(type: locationTypes).Select(location => new { location.city, location.salesPhone });

            var locations = await query.ToArray();

            var locationsWithNullPhones = locations.Where(location => location.salesPhone == null).ToList();
            CollectionAssert.IsEmpty(locationsWithNullPhones);
        }

        [Test]
        public async Task SelectingCitiesWithAliasAndPhonesReturnsPhonesAndCities()
        {
            var query = superChargersContext.Locations(type: locationTypes).Select(location => new { CityName = location.city, location.salesPhone });

            var locations = await query.ToArray();

            var locationsWithNullPhones = locations.Where(location => location.salesPhone == null).ToList();
            var locationsWithNullCity = locations.Where(location => location.CityName == null).ToList();

            Assert.Multiple(() =>
            {
                CollectionAssert.IsEmpty(locationsWithNullPhones);
                CollectionAssert.IsEmpty(locationsWithNullCity);
            });
        }
    }
}