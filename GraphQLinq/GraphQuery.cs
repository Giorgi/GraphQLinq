using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using Newtonsoft.Json.Linq;

namespace GraphQLinq
{
    public class GraphQuery<T> : IEnumerable<T>
    {
        private readonly GraphContext graphContext;
        private readonly GraphQueryBuilder<T> queryBuilder = new GraphQueryBuilder<T>();

        private readonly Lazy<string> lazyQuery;

        internal string QueryName { get; }
        internal LambdaExpression Selector { get; private set; }
        internal Dictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

        internal GraphQuery(GraphContext graphContext, string queryName)
        {
            QueryName = queryName;
            this.graphContext = graphContext;

            lazyQuery = new Lazy<string>(() => queryBuilder.BuildQuery(this));
        }

        public IEnumerator<T> GetEnumerator()
        {
            var query = lazyQuery.Value;

            return new GraphQueryEnumerator<T>(query, graphContext.BaseUrl);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public GraphQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> resultSelector)
        {
            if (resultSelector.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException($"{resultSelector} must be lambda expression", nameof(resultSelector));
            }

            var graphQuery = Clone<TResult>();

            graphQuery.Selector = resultSelector;

            return graphQuery;
        }

        public override string ToString()
        {
            return lazyQuery.Value;
        }

        private GraphQuery<TR> Clone<TR>()
        {
            return new GraphQuery<TR>(graphContext, QueryName) { Arguments = Arguments };
        }
    }

    class GraphQueryBuilder<T>
    {
        private const string QueryTemplate = @"{{ result: {0} {1} {{ {2} }}}}";

        public string BuildQuery(GraphQuery<T> graphQuery)
        {
            var selectClause = "";

            if (graphQuery.Selector != null)
            {
                var body = graphQuery.Selector.Body;

                if (body.NodeType == ExpressionType.MemberAccess)
                {
                    selectClause = "item: " + ((MemberExpression)body).Member.Name;
                }

                if (body.NodeType == ExpressionType.New)
                {
                    var newExpression = (NewExpression)body;

                    var queryFields = newExpression.Members.Zip(newExpression.Arguments,
                        (memberInfo, expression) => new { Alias = memberInfo.Name, ((MemberExpression)expression).Member.Name });

                    selectClause = string.Join(" ", queryFields.Select(arg => arg.Alias + ": " + arg.Name));
                }
            }
            else
            {
                selectClause = BuildSelectClauseForType(typeof(T));
            }

            //(type: [STANDARD_CHARGER, STORE], openSoon: true)
            var argList = graphQuery.Arguments.Where(pair => pair.Value != null).Select(pair =>
            {
                var value = pair.Value.ToString();

                if (pair.Value is bool)
                {
                    value = value.ToLowerInvariant();
                }

                var enumerable = pair.Value as IEnumerable;
                if (enumerable != null)
                {
                    value = $"[{string.Join(", ", enumerable.Cast<object>())}]";
                }

                return $"{pair.Key}: {value}";
            });

            var args = string.Join(", ", argList);
            var argsWithParentheses = string.IsNullOrEmpty(args) ? "" : $"({args})";

            return string.Format(QueryTemplate, graphQuery.QueryName.ToLower(), argsWithParentheses, selectClause);
        }

        private static string BuildSelectClauseForType(Type targetType)
        {
            var propertyInfos = targetType.GetProperties();

            var propertiesToInclude = propertyInfos.Where(info => !info.PropertyType.HasNestedProperties());
            var propertiesToRecurse = propertyInfos.Where(info => info.PropertyType.HasNestedProperties());

            var selectClause = string.Join(Environment.NewLine, propertiesToInclude.Select(info => info.Name));

            var recursiveSelectClause = propertiesToRecurse.Select(info =>
            {
                if (info.PropertyType.IsGenericType)
                {
                    return string.Format("{0}{1}{{{1}{2}}} ", info.Name, Environment.NewLine, BuildSelectClauseForType(info.PropertyType.GetGenericArguments()[0]));
                }

                return BuildSelectClauseForType(info.PropertyType);
            });

            return $"{selectClause}{Environment.NewLine}{string.Join(Environment.NewLine, recursiveSelectClause)}";
        }
    }

    class GraphQueryEnumerator<T> : IEnumerator<T>
    {
        private IEnumerator<T> listEnumerator;

        private readonly string query;
        private readonly string baseUrl;

        private const string DataPathPropertyName = "data";
        private const string ResultPathPropertyName = "result";
        private static readonly bool HasNestedProperties = typeof(T).HasNestedProperties();

        public GraphQueryEnumerator(string query, string baseUrl)
        {
            this.query = query;
            this.baseUrl = baseUrl;
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

            var hasNext = listEnumerator.MoveNext();

            Current = listEnumerator.Current;

            return hasNext;
        }

        private IEnumerable<T> DownloadData()
        {
            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/graphql");

            var json = webClient.UploadString(baseUrl, query);

            var jObject = JObject.Parse(json);

            if (jObject["errors"].HasValues)
            {
                var errors = jObject["errors"].ToObject<List<GraphQueryError>>();
                throw new GraphQueryExecutionException(errors, query);
            }

            var enumerable = jObject[DataPathPropertyName][ResultPathPropertyName].Select(token =>
            {
                token = HasNestedProperties ? token : token["item"];

                return (T)token.ToObject(typeof(T));
            });

            return enumerable;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public T Current { get; set; }

        object IEnumerator.Current => Current;
    }


    static class TypeExtensions
    {
        internal static bool IsPrimitiveOrString(this Type type)
        {
            return type.IsPrimitive || type == typeof(string);
        }

        internal static bool HasNestedProperties(this Type type)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var genericArguments = type.GetGenericArguments();

                return !IsPrimitiveOrString(genericArguments[0]);
            }

            return !IsPrimitiveOrString(type);
        }
    }

    public class GraphQueryError
    {
        public string Message { get; set; }
        public ErrorLocation[] Locations { get; set; }
    }

    public class ErrorLocation
    {
        public int Line { get; set; }
        public int Column { get; set; }
    }

    public class GraphQueryExecutionException : Exception
    {
        public GraphQueryExecutionException(IEnumerable<GraphQueryError> errors, string query)
            : base($"One or more errors occured during query execution. Check {nameof(Errors)} property for details")
        {
            Errors = errors;
            GraphQLQuery = query;
        }

        public string GraphQLQuery { get; private set; }
        public IEnumerable<GraphQueryError> Errors { get; private set; }
    }
}