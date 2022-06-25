using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GraphQLinq.Tests.Tools;
using NUnit.Framework;

namespace GraphQLinq.Tests
{
    public class TestWithTestServer
    {
        public const string TEST_SERVER_URL = "http://localhost:10000/graphql?sdl";
        
        public TestWithTestServer()
        {
            Context = new TestServer.QueryContext(HttpClientHelper.Create(TEST_SERVER_URL));
        }

        public TestServer.QueryContext Context
        {
            get;
        }
        
        [OneTimeSetUp]
        public async Task RunSever()
        {
            var server = TestServer.Program.Main(Array.Empty<string>());

            var httpClient = new HttpClient();
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var response = await httpClient.GetAsync(TEST_SERVER_URL);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return;
                    }

                    await Task.Delay(500);
                }
                catch
                {
                }
            }

            Assert.Fail("Failed to run test graphql server.");
        }

        [OneTimeTearDown]
        public void StopServer()
        {
            TestServer.Program.CancellationTokenSource.Cancel();
        }

    }
}