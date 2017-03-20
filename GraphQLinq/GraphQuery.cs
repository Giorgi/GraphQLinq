using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQLinq
{
    public class GraphQuery<T> : IEnumerable<T>
    {
        private readonly GraphContext graphContext;
        private readonly GraphQueryBuilder<T> queryBuilder = new GraphQueryBuilder<T>();

        private readonly Lazy<string> lazyQuery;

        internal string QueryName { get; }
        internal LambdaExpression Selector { get; private set; }
        internal List<string> Includes { get; private set; } = new List<string>();
        internal Dictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

        internal GraphQuery(GraphContext graphContext, string queryName)
        {
            QueryName = queryName;
            this.graphContext = graphContext;

            lazyQuery = new Lazy<string>(() => queryBuilder.BuildQuery(this, Includes));
        }

        public IEnumerator<T> GetEnumerator()
        {
            var query = lazyQuery.Value;

            return new GraphQueryEnumerator<T>(query, graphContext.BaseUrl, graphContext.Authorization);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public GraphQuery<T> Include<TProperty>(Expression<Func<T, TProperty>> path)
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
            return new GraphQuery<TR>(graphContext, QueryName) { Arguments = Arguments, Selector = Selector, Includes = Includes.ToList() };
        }

        private static bool TryParsePath(Expression expression, out string path)
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

                return genericArguments[0];
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
                return "String";
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