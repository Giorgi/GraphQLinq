using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GraphQLinq
{
    public interface IGraphQueryEnumerator<T>
    {
        Task<IEnumerable<T>> Execute();
    }

    class GraphQueryEnumerator<T, TSource> : IGraphQueryEnumerator<T>
    {
        private readonly GraphContext context;
        private readonly string query;
        private readonly QueryType queryType;
        private readonly Func<TSource, T> mapper;
        private readonly IContractResolver resolver;

        private const string DataPathPropertyName = "data";
        private const string ErrorPathPropertyName = "errors";

        internal GraphQueryEnumerator(GraphContext context, string query, QueryType queryType, Func<TSource, T> mapper)
        {
            this.context = context;
            this.query = query;
            this.mapper = mapper;
            this.queryType = queryType;
            this.resolver = context.ContractResolver ?? new DefaultContractResolver();
        }

        private async Task<IEnumerable<T>> DownloadData()
        {
            var json = await DownloadJson(context.HttpClient);

            var jObject = JObject.Parse(json);

            if (jObject.SelectToken(ErrorPathPropertyName) != null)
            {
                var errors = jObject[ErrorPathPropertyName].ToObject<List<GraphQueryError>>();
                throw new GraphQueryExecutionException(errors, query);
            }

            var enumerable = jObject[DataPathPropertyName][GraphQueryBuilder<T>.ResultAlias]
                .Select(token =>
                {
                    var jsonSerializer = new JsonSerializer { ContractResolver = resolver };
                    var jToken = queryType == QueryType.Collection ? token : token.Parent;

                    if (mapper != null)
                    {
                        var result = jToken.ToObject<TSource>(jsonSerializer);
                        return mapper.Invoke(result);
                    }

                    return jToken.ToObject<T>(jsonSerializer);
                });

            return enumerable;
        }

        private async Task<string> DownloadJson(HttpClient httpClient)
        {
            try
            {
                var content = new StringContent(query, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("", content);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<T>> Execute()
        {
            var enumerable = await DownloadData();

            return enumerable;
        }
    }
}