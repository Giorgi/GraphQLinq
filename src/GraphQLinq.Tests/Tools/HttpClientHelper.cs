using System;
using System.Net.Http;

namespace GraphQLinq.Tests.Tools
{
    public static class HttpClientHelper
    {
        public static HttpClient Stub { get; } = new HttpClient();
        
        public static HttpClient Create(string url)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(url);

            return client;
        }
    }
}