using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        public GraphQuery<Location> Locations(string before = null, string after = null, bool? openSoon = null, bool? isGallery = null, float? boundingBox = null,
                                              int? first = null, int? last = null, LocationType? type = null, Region? region = null, Country? country = null)
        {
            var parameters = MethodBase.GetCurrentMethod().GetParameters();
            var parameterValues = new object[] { before, after, openSoon, isGallery, boundingBox, first, last, type, region, country };

            var dictionary = parameters.Zip(parameterValues, (info, value) => new { info.Name, Value = value }).ToDictionary(arg => arg.Name, arg => arg.Value);

            return BuildQuery(dictionary);
        }

        private GraphQuery<Location> BuildQuery(Dictionary<string, object> parameters, [CallerMemberName] string queryName = null)
        {
            return new GraphQuery<Location>(this, queryName) { Arguments = parameters };
        }
    }


    public class GraphQuery<T> : IEnumerable<T>
    {
        private Type originalType;
        private readonly GraphContext graphContext;
        private readonly string queryName;
        private LambdaExpression selector;

        private const string QueryTemplate = @"{{ result: {0} {1} {{ {2} }}}}";
        private static readonly MethodInfo SelectMethodInfo = GetMethodByExpression<string, string>(q => q.Select(x => x.ToString())).GetGenericMethodDefinition();

        internal GraphQuery(GraphContext graphContext, string queryName)
        {
            originalType = typeof(T);
            this.graphContext = graphContext;
            this.queryName = queryName;
        }

        internal Dictionary<string, object> Arguments { get; set; }

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
            var selectClause = "";
            var args = "";

            if (selector != null)
            {
                var body = selector.Body;

                if (body.NodeType == ExpressionType.MemberAccess)
                {
                    selectClause = ((MemberExpression)body).Member.Name;
                }

                if (body.NodeType == ExpressionType.New)
                {
                    var newExpression = (NewExpression)body;
                    selectClause = string.Join(" ", newExpression.Arguments.Cast<MemberExpression>().Select(e => e.Member.Name));
                }
            }
            else
            {
                selectClause = BuildSelectClauseForType(typeof(T));
            }

            var argsWithValues = Arguments.Where(pair => pair.Value != null);

            if (argsWithValues.Any())
            {
                //(type: STANDARD_CHARGER, openSoon: true)
                var argList = argsWithValues.Select(pair =>
                {
                    var value = pair.Value.ToString();

                    if (pair.Value is bool)
                    {
                        value = value.ToLowerInvariant();
                    }

                    return $"{pair.Key}: {value}";
                });

                args = $"({string.Join(", ", argList)})";
            }

            var query = string.Format(QueryTemplate, queryName.ToLower(), args, selectClause);

            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/graphql");

            var downloadString = webClient.UploadString(graphContext.BaseUrl, query);

            var rootObjectType = typeof(RootObject<>);
            var genericRootType = rootObjectType.MakeGenericType(originalType);

            var rootObject = JsonConvert.DeserializeObject(downloadString, genericRootType);
            var data = genericRootType.GetProperty("Data").GetValue(rootObject);
            var array = data.GetType().GetProperty("Result").GetValue(data);

            if (selector != null)
            {
                var genericSelect = SelectMethodInfo.MakeGenericMethod(originalType, typeof(T));

                array = genericSelect.Invoke(null, new[] { array, selector.Compile() });
            }

            return (array as IEnumerable<T>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static string BuildSelectClauseForType(Type targetType)
        {
            var propertyInfos = targetType.GetProperties();

            Func<Type, bool> isPrimitiveOrString = type => type.IsPrimitive || type == typeof(string);

            Func<Type, bool> includeDirectly = type =>
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var genericArguments = type.GetGenericArguments();

                    return isPrimitiveOrString(genericArguments[0]);
                }

                return isPrimitiveOrString(type);
            };

            var propertiesToInclude = propertyInfos.Where(info => includeDirectly(info.PropertyType));
            var propertiesToRecurse = propertyInfos.Where(info => !includeDirectly(info.PropertyType));

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

        private static MethodInfo GetMethodByExpression<TIn, TOut>(Expression<Func<IEnumerable<TIn>, IEnumerable<TOut>>> expr)
        {
            return ((MethodCallExpression)expr.Body).Method;
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