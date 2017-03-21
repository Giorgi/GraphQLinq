using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace GraphQLinq
{
    class GraphQueryEnumerator<T> : IEnumerator<T>
    {
        private IEnumerator<T> listEnumerator;

        private readonly string query;
        private readonly string baseUrl;
        private readonly string authorization;
        private readonly QueryType queryType;

        private const string DataPathPropertyName = "data";
        private const string ErrorPathPropertyName = "errors";

        private static readonly bool HasNestedProperties = !(typeof(T).IsPrimitiveOrString() || typeof(T).IsList());

        internal GraphQueryEnumerator(string query, string baseUrl, string authorization, QueryType queryType)
        {
            this.query = query;
            this.baseUrl = baseUrl;
            this.authorization = authorization;
            this.queryType = queryType;
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
                            JToken jToken;

                            if (queryType == QueryType.Collection)
                            {
                                jToken = HasNestedProperties ? token : token[GraphQueryBuilder<T>.ItemAlias];
                            }
                            else
                            {
                                jToken = token.Parent;

                                var itemToken = jToken.SelectToken(GraphQueryBuilder<T>.ItemAlias);
                                while (itemToken == null)
                                {
                                    jToken = jToken.First;
                                    itemToken = jToken.SelectToken(GraphQueryBuilder<T>.ItemAlias);
                                }
                                jToken = itemToken;
                            }

                            return jToken.ToObject<T>();
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

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public T Current => listEnumerator.Current;

        object IEnumerator.Current => Current;
    }
}