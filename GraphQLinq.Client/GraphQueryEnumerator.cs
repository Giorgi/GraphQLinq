using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GraphQLinq
{
    class GraphQueryEnumerator<T, TSource> : IEnumerator<T>
    {
        private IEnumerator<T> listEnumerator;

        private readonly string query;
        private readonly string baseUrl;
        private readonly string authorization;
        private readonly QueryType queryType;
        private readonly Func<TSource, T> mapper;
        private readonly IContractResolver resolver;

        private const string DataPathPropertyName = "data";
        private const string ErrorPathPropertyName = "errors";

        internal GraphQueryEnumerator(GraphContext context, string query, QueryType queryType, Func<TSource, T> mapper)
        {
            this.query = query;
            this.mapper = mapper;
            this.queryType = queryType;
            baseUrl = context.BaseUrl;
            resolver = context.ContractResolver ?? new DefaultContractResolver();
            authorization = context.Authorization;
        }

        public void Dispose()
        {
            listEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            if (listEnumerator == null)
            {
                listEnumerator = DownloadData().GetEnumerator();
            }

            return listEnumerator.MoveNext();
        }

        private IEnumerable<T> DownloadData()
        {
            var json = DownloadJson();

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

        private string DownloadJson()
        {
            WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;

            using (var webClient = new WebClient { Proxy = WebRequest.DefaultWebProxy })
            {
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                if (!string.IsNullOrEmpty(authorization))
                {
                    webClient.Headers.Add(HttpRequestHeader.Authorization, authorization);
                }

                try
                {
                    return webClient.UploadString(baseUrl, query);
                }
                catch (WebException exception)
                {
                    using (var responseStream = exception.Response?.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        [ExcludeFromCodeCoverage]
        public void Reset()
        {
            throw new NotImplementedException();
        }

        public T Current => listEnumerator.Current;

        [ExcludeFromCodeCoverage]
        object IEnumerator.Current => Current;
    }
}