using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GraphQLinq
{
    class GraphQueryExecutor<T, TSource>
    {
        private readonly GraphContext context;
        private readonly string query;
        private readonly QueryType queryType;
        private readonly Func<TSource, T> mapper;
        private readonly IContractResolver resolver;

        private const string DataPathPropertyName = "data";
        private const string ErrorPathPropertyName = "errors";

        internal GraphQueryExecutor(GraphContext context, string query, QueryType queryType, Func<TSource, T> mapper)
        {
            this.context = context;
            this.query = query;
            this.mapper = mapper;
            this.queryType = queryType;
            this.resolver = context.ContractResolver ?? new DefaultContractResolver();
        }

        private async Task<IEnumerable<T>> DownloadData()
        {
            var stream = await DownloadJson(context.HttpClient);
            using (var textReader = new StreamReader(stream))
            {
                using (var jsonReader = new JsonTextReader(textReader))
                {
                    var jObject = await JObject.LoadAsync(jsonReader);

                    var errorToken = jObject[ErrorPathPropertyName];

                    if (errorToken != null)
                    {
                        var errors = errorToken.ToObject<List<GraphQueryError>>();
                        throw new GraphQueryExecutionException(errors, query);
                    }

                    var dataToken = jObject[DataPathPropertyName];
                    var resultToken = dataToken?[GraphQueryBuilder<T>.ResultAlias];

                    if (resultToken == null)
                    {
                        throw new GraphQueryExecutionException(query);
                    }
                    
                    var enumerable = resultToken
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
            }
        }

        private async Task<Stream> DownloadJson(HttpClient httpClient)
        {
            var content = new StringContent(query, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("", content);
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<IEnumerable<T>> Execute()
        {
            var enumerable = await DownloadData();

            return enumerable;
        }
    }
}