using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraphQLinq
{
    class GraphQueryExecutor<T, TSource>
    {
        private readonly GraphContext context;
        private readonly string query;
        private readonly QueryType queryType;
        private readonly Func<TSource, T> mapper;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        private const string DataPathPropertyName = "data";
        private const string ErrorPathPropertyName = "errors";

        internal GraphQueryExecutor(GraphContext context, string query, QueryType queryType, Func<TSource, T> mapper)
        {
            this.context = context;
            this.query = query;
            this.mapper = mapper;
            this.queryType = queryType;

            jsonSerializerOptions = context.JsonSerializerOptions;
        }

        private T JsonElementToItem(JsonElement jsonElement)
        {
            if (mapper != null)
            {
                var result = jsonElement.Deserialize<TSource>(jsonSerializerOptions);
                return mapper.Invoke(result);
            }
            else
            {
                var result = jsonElement.Deserialize<T>(jsonSerializerOptions);
                return result;
            }
        }

        internal async Task<(T Item, IEnumerable<T> Enumerable)> Execute()
        {
            using (var content = new StringContent(query, Encoding.UTF8, "application/json"))
            {
                using (var response = await context.HttpClient.PostAsync("", content))
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var document = await JsonDocument.ParseAsync(stream);

                        var hasError = document.RootElement.TryGetProperty(ErrorPathPropertyName, out var errorElement);

                        if (hasError)
                        {
                            var errors = errorElement.Deserialize<List<GraphQueryError>>();
                            throw new GraphQueryExecutionException(errors, query);
                        }

                        var hasData = document.RootElement.TryGetProperty(DataPathPropertyName, out var dataElement);

                        if (!hasData)
                        {
                            throw new GraphQueryExecutionException(query);
                        }

                        var hasResult = dataElement.TryGetProperty(GraphQueryBuilder<T>.ResultAlias, out var resultElement);

                        if (!hasResult)
                        {
                            throw new GraphQueryExecutionException(query);
                        }

                        if (queryType == QueryType.Item)
                        {
                            return (JsonElementToItem(resultElement), null);
                        }

                        return (default, resultElement.EnumerateArray().Select(JsonElementToItem));
                    }
                }
            }
        }
    }
}