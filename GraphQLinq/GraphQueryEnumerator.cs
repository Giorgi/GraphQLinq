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

        private const string DataPathPropertyName = "data";
        private const string ErrorPathPropertyName = "errors";

        private static readonly bool HasNestedProperties = !(typeof(T).IsPrimitiveOrString() || typeof(T).IsList());

        public GraphQueryEnumerator(string query, string baseUrl, string authorization)
        {
            this.query = query;
            this.baseUrl = baseUrl;
            this.authorization = authorization;
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
            var defaultWebProxy = WebRequest.DefaultWebProxy;
            defaultWebProxy.Credentials = CredentialCache.DefaultCredentials;

            using (var webClient = new WebClient { Proxy = defaultWebProxy })
            {
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                if (!string.IsNullOrEmpty(authorization))
                {
                    webClient.Headers.Add(HttpRequestHeader.Authorization, authorization);
                }

                var json = "";
                try
                {
                    json = webClient.UploadString(baseUrl, query);
                }
                catch (WebException exception)
                {
                    using (var responseStream = exception.Response.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            json = streamReader.ReadToEnd();
                        }
                    }
                }

                var jObject = JObject.Parse(json);

                if (jObject.SelectToken(ErrorPathPropertyName) != null)
                {
                    var errors = jObject[ErrorPathPropertyName].ToObject<List<GraphQueryError>>();
                    throw new GraphQueryExecutionException(errors, query);
                }

                var enumerable = jObject[DataPathPropertyName][GraphQueryBuilder<T>.ResultAlias]
                    .Select(token => (HasNestedProperties ? token : token[GraphQueryBuilder<T>.ItemAlias]).ToObject<T>());

                return enumerable;
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