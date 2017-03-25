using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GraphQLinq
{
    public class GraphContext
    {
        protected GraphContext(string baseUrl, string authorization)
        {
            BaseUrl = baseUrl;
            Authorization = authorization;
        }

        public string BaseUrl { get; private set; }
        public string Authorization { get; private set; }

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
            var parameters = GetType().GetMethod(queryName).GetParameters();
            var arguments = parameters.Zip(parameterValues, (info, value) => new { info.Name, Value = value }).ToDictionary(arg => arg.Name, arg => arg.Value);
            return arguments;
        }
    }
}