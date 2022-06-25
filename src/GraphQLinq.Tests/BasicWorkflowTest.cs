using System.Threading.Tasks;
using NUnit.Framework;

namespace GraphQLinq.Tests
{
    [TestFixture]
    public class BasicWorkflowTest : TestWithTestServer
    {
        [Test]
        public async Task CanExecuteQuery()
        {
            var user = await Context.UserTemporaryFixForNullable(42).ToItem();

            Assert.Multiple(() =>
            {
                Assert.AreEqual("Jon", user.FirstName);
                Assert.AreEqual("Smith", user.LastName);
            });
        }

        [Test]
        public void CanHandleFailedQueries()
        {
            Assert.CatchAsync<GraphQueryExecutionException>(async () => await Context.FailUser().ToItem());
        }

        [Test]
        public void NullableDoesntWork()
        {
            Assert.CatchAsync<GraphQueryExecutionException>(async () => await Context.User(42).ToItem());
        }
    }
}