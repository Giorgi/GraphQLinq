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
        private readonly string queryName;
        private LambdaExpression selector;

        private const string QueryTemplate = @"{{ result: {0} {1} {{ {2} }}}}";

        internal GraphQuery(GraphContext graphContext, string queryName)
        {
            this.graphContext = graphContext;
            this.queryName = queryName;
        }

        internal Dictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

        private GraphQuery<TR> Clone<TR>()
        {
            return new GraphQuery<TR>(graphContext, queryName);
        }

        public GraphQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> resultSelector)
        {
            if (resultSelector.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException($"{resultSelector} must be lambda expression", nameof(resultSelector));
            }

            var graphQuery = Clone<TResult>();

            graphQuery.selector = resultSelector;
            graphQuery.Arguments = Arguments;

            return graphQuery;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var args = "";
            var selectClause = "";

            if (selector != null)
            {
                var body = selector.Body;

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

            var argsWithValues = Arguments.Where(pair => pair.Value != null);

            if (argsWithValues.Any())
            {
                //(type: [STANDARD_CHARGER, STORE], openSoon: true)
                var argList = argsWithValues.Select(pair =>
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

                args = $"({string.Join(", ", argList)})";
            }

            var query = string.Format(QueryTemplate, queryName.ToLower(), args, selectClause);

            return new GraphQueryEnumerator<T>(query, graphContext.BaseUrl);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
}