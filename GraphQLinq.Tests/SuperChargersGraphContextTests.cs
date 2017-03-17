using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLinq.Tests
{
    [TestFixture]
    public class SuperChargersGraphContextTests
    {
        SuperChargersGraphContext context = new SuperChargersGraphContext("");

        [Test]
        public void SelectingSinglePropertyQueryIncludesSelectedProperty()
        {
            var locations = context.Locations().Select(l => l.city);

            var query = locations.ToString();

            Assert.That(query, Does.Contain("city"));
        }

        [Test]
        public void SelectingMultiplePropertiesQueryIncludesSelectedProperties()
        {
            var locations = context.Locations().Select(l => new { l.city, l.region });

            var query = locations.ToString();

            Assert.That(query, Does.Contain("city").And.Contains("region"));
        }

        [Test]
        public void SelectingNavigationPropertyQueryIncludesPropertiesOfNavigationProperty()
        {
            var locations = context.Locations().Select(l => l.salesPhone);

            var query = locations.ToString();

            Assert.That(query, Does.Contain("number").And.Contains("label"));
        }

        [Test]
        public void SelectingPrimitiveAndNavigationPropertyQueryIncludesPropertiesOfNavigationProperty()
        {
            var locations = context.Locations().Select(l => new { l.salesPhone, l.city });

            var query = locations.ToString();

            Assert.That(query, Does.Contain("number").And.Contains("label").And.Contains("city"));
        }

        [Test]
        public void SelectingPropertyQueryIncludesPropertyInCamelCase()
        {
            var locations = context.Locations().Select(l => l.Country);

            var query = locations.ToString();

            Assert.That(query, Does.Contain("country"));
        }

        [Test]
        public void SelectingAllPropertiesQueryDoesNotIncludeNavigationProperties()
        {
            var locations = context.Locations();

            var query = locations.ToString();

            Assert.That(query, Does.Not.Contain(nameof(Location.salesPhone)).And.Not.Contains(nameof(Location.emails)));
        }

        [Test]
        public void IncludingNavigationPropertyQueryIncludesIncludedNavigationPropertyWithNestedProperties()
        {
            var locations = context.Locations().Include(l => l.salesPhone);

            var query = locations.ToString();

            Assert.That(query, Does.Contain(nameof(Location.salesPhone))
                               .And.Contains(nameof(Phone.label))
                               .And.Contains(nameof(Phone.number)));
        }

        [Test]
        public void IncludingNavigationPropertyWithSpecificPropertyQueryIncludesIncludedNavigationPropertySpecifiedProperty()
        {
            var locations = context.Locations().Include(l => l.salesPhone.Select(p => p.number));

            var query = locations.ToString();

            Assert.That(query, Does.Contain(nameof(Location.salesPhone))
                               .And.Not.Contains(nameof(Phone.label))
                               .And.Contains(nameof(Phone.number)));
        }
                
        [Test]
        public void FilteringQueryWithScalarParameterGeneratedQueryIncludesPassedParameter()
        {
            var locations = context.Locations(openSoon: true).Select(l => l.city);

            var query = locations.ToString();

            Assert.That(query, Does.Contain("openSoon: true"));
        }

        [Test]
        public void FilteringQueryWithCollectionParameterGeneratedQueryIncludesPassedParameter()
        {
            var locations = context.Locations(type: new List<LocationType> { LocationType.SERVICE, LocationType.STORE }).Select(l => l.city);

            var query = locations.ToString();

            Assert.That(query, Does.Contain("type: [SERVICE, STORE]"));
        }
    }
}
