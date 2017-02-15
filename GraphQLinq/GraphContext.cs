using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;

namespace GraphQLinq
{
    public class GraphContext
    {
        public string BaseUrl { get; set; }

        public GraphContext(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public GraphQuery<Location> Locations()
        {
            var queryName = nameof(Locations);
            return new GraphQuery<Location>(this, queryName);
        }
    }

    public class GraphQuery<T> : IEnumerable<T>
    {
        private Type originalType;
        private readonly GraphContext graphContext;
        private readonly string queryName;
        private LambdaExpression selector;

        internal GraphQuery(GraphContext graphContext, string queryName)
        {
            originalType = typeof(T);
            this.graphContext = graphContext;
            this.queryName = queryName;
        }

        private GraphQuery<TR> Clone<TR>()
        {
            var graphQuery = new GraphQuery<TR>(graphContext, queryName) { originalType = originalType };

            return graphQuery;
        }

        public GraphQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> resultSelector)
        {
            if (resultSelector.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException($"{resultSelector} must be lambda expression", nameof(resultSelector));
            }

            var graphQuery = Clone<TResult>();

            graphQuery.selector = resultSelector;
            return graphQuery;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var query = "";

            var queryTemplate = @"{{ result: {0} {{ {1} }}}}";

            if (selector != null)
            {
                var body = selector.Body;

                if (body.NodeType == ExpressionType.MemberAccess)
                {
                    var expression = (MemberExpression) body;
                    var selectClause = expression.Member.Name;
                    query = String.Format(queryTemplate, queryName.ToLower(), selectClause);
                }

                if (body.NodeType == ExpressionType.New)
                {
                    var newExpression = (NewExpression) body;

                    var selectClause = string.Join(" ", newExpression.Arguments.Cast<MemberExpression>().Select(e => e.Member.Name));
                    query = String.Format(queryTemplate, queryName.ToLower(), selectClause);
                }
            }
            else
            {
                query = String.Format(queryTemplate, queryName.ToLower(), "id city title");
            }

            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/graphql");
            var downloadString = webClient.UploadString(graphContext.BaseUrl, query);

            var type = typeof(RootObject<>);
            var genericRootType = type.MakeGenericType(originalType);

            var rootObject = JsonConvert.DeserializeObject(downloadString, genericRootType);
            var data = genericRootType.GetProperty("Data").GetValue(rootObject);
            var array = data.GetType().GetProperty("Result").GetValue(data);

            if (selector != null)
            {
                var selectMethod = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static).First(info => info.Name == "Select");
                var genericSelect = selectMethod.MakeGenericMethod(originalType, typeof(T));

                array = genericSelect.Invoke(null, new[] { array, selector.Compile() });
            }

            return (array as IEnumerable<T>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    public class RootObject<T>
    {
        public ResultData<T> Data { get; set; }
    }

    public class ResultData<T>
    {
        public List<T> Result { get; set; }
    }
}