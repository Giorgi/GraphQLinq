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

        protected GraphQuery<T> BuildQuery<T>(object[] parameterValues, [CallerMemberName] string queryName = null)
        {
            var parameters = GetType().GetMethod(queryName).GetParameters();
            var arguments = parameters.Zip(parameterValues, (info, value) => new { info.Name, Value = value }).ToDictionary(arg => arg.Name, arg => arg.Value);

            return new GraphQuery<T>(this, queryName) { Arguments = arguments };
        }
    }
}