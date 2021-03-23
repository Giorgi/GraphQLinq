using System.Collections.Generic;
using System.Linq;
using HSL;
using NUnit.Framework;

namespace GraphQLinq.Tests
{
    [TestFixture]
    [Category("Single item query")]
    [Category("Integration tests")]
    class SingleItemQueryTests
    {
        const string TripId = "HSL:6908 3_20170814_Ti_1_1215";
        readonly HslGraphContext hslGraphContext = new HslGraphContext("https://api.digitransit.fi/routing/v1/routers/finland/index/graphql");

        [Test]
        public void SelectingSingleTripIdIsNotNull()
        {
            var tripId = hslGraphContext.Trip(TripId).Select(t => t.gtfsId).ToItem();

            Assert.That(tripId, Is.Not.Null);
        }

        [Test]
        public void SelectingNestedPropertiesOfSingleTripNestedPropertiesAreNotNull()
        {
            var item = hslGraphContext.Trip(TripId).Select(trip =>
                new TripDetails(trip.gtfsId, trip.route.gtfsId, trip.pattern.geometry, trip.route.agency.name, trip.route.agency.phone)
            ).ToItem();

            Assert.Multiple(() =>
            {
                Assert.That(item.tg, Is.Not.Null);
                Assert.That(item.aatrg, Is.Not.Null);
                Assert.That(item.geometry, Is.Not.Null);
                Assert.That(item.n, Is.Not.Null);
                Assert.That(item.p, Is.Not.Null);
            });
        }

        [Test]
        public void SelectingNestedPropertiesOfSingleTripAndCallingConstructorNestedPropertiesAreNotNull()
        {
            var item = hslGraphContext.Trip(TripId)
                .Select(trip => new TripDetails(trip.gtfsId, trip.route.gtfsId, trip.pattern.geometry, trip.route.agency.name, trip.route.agency.phone))
                .ToItem();

            Assert.Multiple(() =>
            {
                Assert.That(item.tg, Is.Not.Null);
                Assert.That(item.aatrg, Is.Not.Null);
                Assert.That(item.geometry, Is.Not.Null);
                Assert.That(item.n, Is.Not.Null);
                Assert.That(item.p, Is.Not.Null);
            });
        }

        [Test]
        public void SelectingThreeLevelNestedPropertyOfSingleTripNestedPropertyIsNotNull()
        {
            var routes = hslGraphContext.Trip(TripId).Select(trip => trip.route.agency.routes).ToItem();

            CollectionAssert.IsNotEmpty(routes);
        }

        [Test]
        public void SelectingSingleTripNestedPropertyIsNull()
        {
            var trip = hslGraphContext.Trip(TripId).ToItem();

            Assert.That(trip.pattern, Is.Null);
        }

        [Test]
        public void SelectingAndIncludingNestedPropertySingleTripNestedPropertyIsNotNull()
        {
            var pattern = hslGraphContext.Trip(TripId).Include(trip => trip.route).ToItem();

            Assert.That(pattern.route, Is.Not.Null);
        }

        [Test]
        public void SelectingListOfListNestedPropertyShouldCheckListTypeRecursively()
        {
            Agency agency = null;

            var agencyId = "248798";
            Assert.DoesNotThrow(() => agency = hslGraphContext.Agency(agencyId).Include(a => a.routes.Select(route => route.trips.Select(trip => trip.geometry))).ToItem());

            if (agency == null)
            {
                Assert.Inconclusive($"Agency with id {agencyId} not found");
            }
            else
            {
                Assert.Multiple(() =>
                {
                    CollectionAssert.IsNotEmpty(agency.routes[0].trips[0].geometry);
                    CollectionAssert.IsNotEmpty(agency.routes[1].trips[0].geometry);
                });
            }
        }
    }

    class TripDetails
    {
        public string tg { get; }
        public string aatrg { get; }
        public List<Coordinates> geometry { get; }
        public string n { get; }
        public string p { get; }

        internal TripDetails(string tg, string aatrg, List<Coordinates> geometry, string n, string p)
        {
            this.tg = tg;
            this.aatrg = aatrg;
            this.geometry = geometry;
            this.n = n;
            this.p = p;
        }
    }
}
