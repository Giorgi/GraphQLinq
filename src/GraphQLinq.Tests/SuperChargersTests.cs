using System.Collections.Generic;
using System.Linq;
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
        public void SelectingCitiesReturnsListOfCities()
        {   
            var query = superChargersContext.Locations(type: locationTypes).Select(l => l.city);

            var locations = query.ToList();

            CollectionAssert.IsNotEmpty(locations);
            CollectionAssert.AllItemsAreNotNull(locations);
        }

        [Test]
        public void SelectingLocationsDoesNotReturnPhones()
        {
            var query = superChargersContext.Locations(type: locationTypes);

            var locations = query.ToList();
            Assert.That(locations, Is.All.Matches<Location>(l => l.salesPhone == null));
        }

        [Test]
        public void SelectingLocationsAndIncludingPhonesReturnsPhones()
        {
            var query = superChargersContext.Locations(type: locationTypes).Include(location => location.salesPhone);

            var locations = query.ToList();

            Assert.That(locations, Is.All.Matches<Location>(l => l.salesPhone != null));
        }

        [Test]
        public void SelectingCitiesAndPhonesReturnsPhones()
        {
            var query = superChargersContext.Locations(type: locationTypes).Select(location => new { location.city, location.salesPhone });

            var locations = query.ToList();

            var locationsWithNullPhones = locations.Where(location => location.salesPhone == null).ToList();
            CollectionAssert.IsEmpty(locationsWithNullPhones);
        }

        [Test]
        public void SelectingCitiesWithAliasAndPhonesReturnsPhonesAndCities()
        {
            var query = superChargersContext.Locations(type: locationTypes).Select(location => new { CityName = location.city, location.salesPhone });

            var locations = query.ToList();

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