using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Serialization;

namespace GraphQLinq
{
    public class GraphContext
    {
        public HttpClient HttpClient { get; }

        protected GraphContext(HttpClient httpClient)
        {
            HttpClient = httpClient;
            ContractResolver = new DefaultContractResolver();
        }

        protected GraphContext(string url, string authorization)
        {
            HttpClient = new HttpClient();
            if (!string.IsNullOrEmpty(url))
            {
                HttpClient.BaseAddress = new Uri(url);
            }
            if (!string.IsNullOrEmpty(authorization))
            {
                HttpClient.DefaultRequestHeaders.Add("Authorization", authorization);
            }
            ContractResolver = new DefaultContractResolver();
        }

        public IContractResolver ContractResolver { get; set; }

        protected GraphCollectionQuery<T> BuildCollectionQuery<T>(object[] parameterValues, [CallerMemberName] string queryName = null)
        {
            var arguments = BuildDictionary(parameterValues, queryName);
            return new GraphCollectionQuery<T, T>(this, queryName) { Arguments = arguments };
        }

        protected GraphItemQuery<T> BuildItemQuery<T>(object[] parameterValues, [CallerMemberName] string queryName = null)
        {
            var arguments = BuildDictionary(parameterValues, queryName);
            return new GraphItemQuery<T, T>(this, queryName) { Arguments = arguments };
        }

        private Dictionary<string, object> BuildDictionary(object[] parameterValues, string queryName)
        {
            var parameters = GetType().GetMethod(queryName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).GetParameters();
            var arguments = parameters.Zip(parameterValues, (info, value) => new { info.Name, Value = value }).ToDictionary(arg => arg.Name, arg => arg.Value);
            return arguments;
        }
    }
}