using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace GraphQLinq.Tests
{
    [TestFixture(Category = "Query generation tests")]
    public class QueryGenerationTests
    {
        SuperChargersGraphContext context = new SuperChargersGraphContext("http://example.com");
        HslGraphContext hslGraphContext = new HslGraphContext("http://example.com");

        [Test]
        public void SelectingSinglePropertyQueryIncludesSelectedProperty()
        {
            var locations = context.Locations().Select(l => l.city);

            Assert.That(locations.Query, Does.Contain("city"));
        }

        [Test]
        public void SelectingMultiplePropertiesQueryIncludesSelectedProperties()
        {
            var locations = context.Locations().Select(l => new { l.city, l.region });

            Assert.That(locations.Query, Does.Contain("city").And.Contains("region"));
        }

        [Test]
        public void SelectingNavigationPropertyQueryIncludesPropertiesOfNavigationProperty()
        {
            var locations = context.Locations().Select(l => l.salesPhone);

            Assert.That(locations.Query, Does.Contain("number").And.Contains("label"));
        }

        [Test]
        public void SelectingListOfStringNavigationPropertyQueryDoesNotIncludesPropertiesOfNavigationProperty()
        {
            var locations = context.Locations().Select(l => l.locationType);

            Assert.That(locations.Query, Does.Not.Contain("length").And.Not.Contains("chars"));
        }

        [Test]
        public void SelectingPrimitiveAndNavigationPropertyQueryIncludesPropertiesOfNavigationProperty()
        {
            var locations = context.Locations().Select(l => new { l.salesPhone, l.city });

            Assert.That(locations.Query, Does.Contain("number").And.Contains("label").And.Contains("city"));
        }

        [Test]
        public void SelectingPropertyQueryIncludesPropertyInCamelCase()
        {
            var locations = context.Locations().Select(l => l.Country);

            Assert.That(locations.Query, Does.Contain("country"));
        }

        [Test]
        public void SelectingAllPropertiesQueryDoesNotIncludeNavigationProperties()
        {
            var locations = context.Locations();

            Assert.That(locations.Query, Does.Not.Contain(nameof(Location.salesPhone)).And.Not.Contains(nameof(Location.emails)));
        }

        [Test]
        public void IncludingNavigationPropertyQueryIncludesIncludedNavigationPropertyWithNestedProperties()
        {
            var locations = context.Locations().Include(l => l.salesPhone);

            Assert.That(locations.Query, Does.Contain(nameof(Location.salesPhone))
                               .And.Contains(nameof(Phone.label))
                               .And.Contains(nameof(Phone.number)));
        }

        [Test]
        public void IncludingNavigationPropertyWithSpecificPropertyQueryIncludesIncludedNavigationPropertySpecifiedProperty()
        {
            var locations = context.Locations().Include(l => l.salesPhone.Select(p => p.number));

            Assert.That(locations.Query, Does.Contain(nameof(Location.salesPhone))
                               .And.Not.Contains(nameof(Phone.label))
                               .And.Contains(nameof(Phone.number)));
        }

        [Test]
        public void FilteringQueryWithScalarParameterGeneratedQueryIncludesPassedParameter()
        {
            var locations = context.Locations(openSoon: true).Select(l => l.city);

            Assert.Multiple(() =>
            {
                Assert.That(locations.Query, Does.Contain("$openSoon: Boolean").And.Contain("openSoon: $openSoon"));
                CollectionAssert.Contains(locations.QueryVariables, new KeyValuePair<string, object>("openSoon", true));
            });
        }

        [Test]
        public void FilteringQueryWithCollectionParameterGeneratedQueryIncludesPassedParameter()
        {
            var locations = context.Locations(type: new List<LocationType> { LocationType.SERVICE, LocationType.STORE }).Select(l => l.city);

            var query = locations.ToString();

            Assert.That(query, Does.Contain("type\":[\"SERVICE\",\"STORE\"]"));
        }

        [Test]
        public void FilteringQueryWithCollectionParameterGeneratedQueryIncludesPassedParameterTypeInformation()
        {
            var locations = context.Locations(type: new List<LocationType> { LocationType.SERVICE, LocationType.STORE }).Select(l => l.city);

            var query = locations.ToString();

            Assert.That(query, Does.Contain("$type: [LocationType]"));
        }

        [Test]
        public void FilteringQueryWithCollectionParameterGeneratedQueryFiltersLocationsByType()
        {
            var locations = context.Locations(type: new List<LocationType> { LocationType.SERVICE, LocationType.STORE }).Select(l => l.city);

            var query = locations.ToString();

            Assert.That(query, Does.Contain("type: $type"));
        }

        [Test]
        public void SelectingListOfListNestedPropertyQueryShouldNotIncludeListProperties()
        {
            var agency = hslGraphContext.Agency("232919").Include(a => a.routes.Select(route => route.trips.Select(trip => trip.geometry)));

            Assert.That(agency.Query, Does.Not.Contain("capacity").And.Not.Contain("count").And.Not.Contain("item"));
        }
    }
}
