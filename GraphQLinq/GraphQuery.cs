using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQLinq
{
    public abstract class GraphQuery<T>
    {
        private readonly GraphContext context;
        private readonly Lazy<GraphQLQuery> lazyQuery;
        private readonly GraphQueryBuilder<T> queryBuilder = new GraphQueryBuilder<T>();

        internal string QueryName { get; }
        internal LambdaExpression Selector { get; private set; }
        internal List<string> Includes { get; private set; } = new List<string>();
        internal Dictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

        internal GraphQuery(GraphContext graphContext, string queryName)
        {
            QueryName = queryName;
            context = graphContext;

            lazyQuery = new Lazy<GraphQLQuery>(() => queryBuilder.BuildQuery(this, Includes));
        }

        public override string ToString()
        {
            return lazyQuery.Value.FullQuery;
        }

        protected GraphQuery<TR> Clone<TR>()
        {
            var genericQueryType = GetType().GetGenericTypeDefinition();
            var genericArguments = GetType().GetGenericArguments();
            var cloneType = genericQueryType.MakeGenericType(typeof(TR), genericArguments[1]);

            var instance = (GraphQuery<TR>)Activator.CreateInstance(cloneType, context, QueryName);

            instance.Arguments = Arguments;
            instance.Selector = Selector;
            instance.Includes = Includes.ToList();

            return instance;
        }

        protected static bool TryParsePath(Expression expression, out string path)
        {
            path = null;
            var withoutConvert = expression.RemoveConvert(); // Removes boxing
            var memberExpression = withoutConvert as MemberExpression;
            var callExpression = withoutConvert as MethodCallExpression;

            if (memberExpression != null)
            {
                var thisPart = memberExpression.Member.Name;
                string parentPart;
                if (!TryParsePath(memberExpression.Expression, out parentPart))
                {
                    return false;
                }
                path = parentPart == null ? thisPart : (parentPart + "." + thisPart);
            }
            else if (callExpression != null)
            {
                if (callExpression.Method.Name == "Select"
                    && callExpression.Arguments.Count == 2)
                {
                    string parentPart;
                    if (!TryParsePath(callExpression.Arguments[0], out parentPart))
                    {
                        return false;
                    }
                    if (parentPart != null)
                    {
                        var subExpression = callExpression.Arguments[1] as LambdaExpression;
                        if (subExpression != null)
                        {
                            string thisPart;
                            if (!TryParsePath(subExpression.Body, out thisPart))
                            {
                                return false;
                            }
                            if (thisPart != null)
                            {
                                path = parentPart + "." + thisPart;
                                return true;
                            }
                        }
                    }
                }
                return false;
            }

            return true;
        }

        protected GraphQuery<T> BuildInclude<TProperty>(Expression<Func<T, TProperty>> path)
        {
            string include;
            if (!TryParsePath(path.Body, out include) || include == null)
            {
                throw new ArgumentException("Invalid Include Path Expression", nameof(path));
            }

            var graphQuery = Clone<T>();
            graphQuery.Includes.Add(include);

            return graphQuery;
        }

        protected GraphQuery<TResult> BuildSelect<TResult>(Expression<Func<T, TResult>> resultSelector)
        {
            if (resultSelector.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException($"{resultSelector} must be lambda expression", nameof(resultSelector));
            }

            var graphQuery = Clone<TResult>();
            graphQuery.Selector = resultSelector;

            return graphQuery;
        }

        internal IEnumerator<T> BuildEnumerator<TSource>(QueryType queryType)
        {
            var query = lazyQuery.Value;

            var mapper = (Func<TSource, T>) Selector?.Compile();

            return new GraphQueryEnumerator<T, TSource>(query.FullQuery, context.BaseUrl, context.Authorization, queryType, mapper);
        }
    }

    public abstract class GraphItemQuery<T> : GraphQuery<T>
    {
        protected GraphItemQuery(GraphContext graphContext, string queryName) : base(graphContext, queryName) { }

        public GraphItemQuery<T> Include<TProperty>(Expression<Func<T, TProperty>> path)
        {
            return (GraphItemQuery<T>)BuildInclude(path);
        }

        public GraphItemQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> resultSelector)
        {
            return (GraphItemQuery<TResult>)BuildSelect(resultSelector);
        }

        public abstract T ToItem();
    }

    public abstract class GraphCollectionQuery<T> : GraphQuery<T>, IEnumerable<T>
    {
        protected GraphCollectionQuery(GraphContext graphContext, string queryName) : base(graphContext, queryName) { }

        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public GraphCollectionQuery<T> Include<TProperty>(Expression<Func<T, TProperty>> path)
        {
            return (GraphCollectionQuery<T>)BuildInclude(path);
        }

        public GraphCollectionQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> resultSelector)
        {
            return (GraphCollectionQuery<TResult>)BuildSelect(resultSelector);
        }
    }

    public class GraphItemQuery<T, TSource> : GraphItemQuery<T>
    {
        public GraphItemQuery(GraphContext graphContext, string queryName) : base(graphContext, queryName)
        {
        }

        public override T ToItem()
        {
            using (var enumerator = BuildEnumerator<TSource>(QueryType.Item))
            {
                enumerator.MoveNext();
                return enumerator.Current;
            }
        }
    }

    public class GraphCollectionQuery<T, TSource> : GraphCollectionQuery<T>
    {
        public GraphCollectionQuery(GraphContext graphContext, string queryName) : base(graphContext, queryName)
        {
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return BuildEnumerator<TSource>(QueryType.Collection);
        }
    }

    internal enum QueryType
    {
        Item,
        Collection
    }

    static class ExtensionsUtils
    {
        internal static bool IsPrimitiveOrString(this Type type)
        {
            return type.IsPrimitive || type == typeof(string);
        }

        internal static bool IsList(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        internal static bool HasNestedProperties(this Type type)
        {
            var trueType = GetTypeOrListType(type);

            return !IsPrimitiveOrString(trueType);
        }

        internal static Type GetTypeOrListType(this Type type)
        {
            if (type.IsList())
            {
                var genericArguments = type.GetGenericArguments();

                return genericArguments[0].GetTypeOrListType();
            }

            return type;
        }

        internal static Expression RemoveConvert(this Expression expression)
        {
            while ((expression != null)
                   && (expression.NodeType == ExpressionType.Convert
                       || expression.NodeType == ExpressionType.ConvertChecked))
            {
                expression = RemoveConvert(((UnaryExpression)expression).Operand);
            }

            return expression;
        }

        internal static string ToCamelCase(this string input)
        {
            if (char.IsLower(input[0]))
            {
                return input;
            }
            return input.Substring(0, 1).ToLower() + input.Substring(1);
        }

        internal static string ToGraphQlType(this Type type)
        {
            if (type == typeof(bool))
            {
                return "Boolean";
            }

            if (type == typeof(int))
            {
                return "Int";
            }

            if (type == typeof(string))
            {
                return "String!";
            }

            if (type == typeof(float))
            {
                return "Float";
            }

            if (type.IsList())
            {
                var listType = type.GetTypeOrListType();
                return "[" + ToGraphQlType(listType) + "]";
            }

            return type.Name;
        }
    }
}