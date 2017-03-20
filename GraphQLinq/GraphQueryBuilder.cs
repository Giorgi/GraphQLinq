using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GraphQLinq
{
    class GraphQueryBuilder<T>
    {
        private const string QueryTemplate = @"query {0} {{ {1}: {2} {3} {{ {4} }}}}";
        internal const string ItemAlias = "item";
        internal const string ResultAlias = "result";

        public string BuildQuery(GraphQuery<T> graphQuery, List<string> includes)
        {
            var selectClause = "";

            if (graphQuery.Selector != null)
            {
                var body = graphQuery.Selector.Body;

                var padding = new string(' ', 4);

                if (body.NodeType == ExpressionType.MemberAccess)
                {
                    var member = ((MemberExpression)body).Member as PropertyInfo;
                    if (member != null)
                    {
                        selectClause = $"{padding}{ItemAlias}: {member.Name.ToCamelCase()}";

                        if (!member.PropertyType.IsPrimitiveOrString())
                        {
                            var fieldForProperty = BuildSelectClauseForType(member.PropertyType.GetTypeOrListType(), 3);
                            selectClause = $"{selectClause} {{{Environment.NewLine}{fieldForProperty}{Environment.NewLine}{padding}}}";
                        }
                    }
                }

                if (body.NodeType == ExpressionType.New)
                {
                    var newExpression = (NewExpression)body;

                    var fields = new List<string>();

                    for (var i = 0; i < newExpression.Members.Count; i++)
                    {
                        var member = newExpression.Members[i] as PropertyInfo;

                        if (member == null) continue;

                        var fieldName = ((MemberExpression)newExpression.Arguments[i]).Member.Name;

                        var selectField = padding + member.Name + ": " + fieldName.ToCamelCase();  //generate something like alias: field e.g. mailList: emails

                        if (!member.PropertyType.IsPrimitiveOrString())
                        {
                            var fieldForProperty = BuildSelectClauseForType(member.PropertyType.GetTypeOrListType(), 3);
                            selectField = $"{selectField} {{{Environment.NewLine}{fieldForProperty}{Environment.NewLine}{padding}}}";
                        }

                        fields.Add(selectField);
                    }

                    selectClause = string.Join(Environment.NewLine, fields);
                }
            }
            else
            {
                selectClause = BuildSelectClauseForType(typeof(T), includes);
            }

            selectClause = Environment.NewLine + selectClause + Environment.NewLine;

            var passedArguments = graphQuery.Arguments.Where(pair => pair.Value != null).ToList();

            var queryParameters = passedArguments.Any() ? $"({string.Join(", ", passedArguments.Select(pair => $"{pair.Key}: ${pair.Key}"))})" : "";
            var queryParameterTypes = passedArguments.Any() ? $"({string.Join(", ", passedArguments.Select(pair => $"${pair.Key}: {pair.Value.GetType().ToGraphQlType()}"))})" : "";

            var queryVariables = passedArguments.ToDictionary(pair => pair.Key, pair => pair.Value);
            var graphQLQuery = string.Format(QueryTemplate, queryParameterTypes, ResultAlias, graphQuery.QueryName.ToLower(), queryParameters, selectClause);

            var dictionary = new Dictionary<string, object> { { "query", graphQLQuery }, { "variables", queryVariables } };

            var json = JsonConvert.SerializeObject(dictionary, new StringEnumConverter());
            return json;
        }

        private static string BuildSelectClauseForType(Type targetType, int depth = 1)
        {
            var propertyInfos = targetType.GetProperties();

            var propertiesToInclude = propertyInfos.Where(info => !info.PropertyType.HasNestedProperties());

            var selectClause = string.Join(Environment.NewLine, propertiesToInclude.Select(info => new string(' ', depth * 2) + info.Name.ToCamelCase()));

            return selectClause;
        }

        private static string BuildSelectClauseForType(Type targetType, IEnumerable<string> includes)
        {
            var selectClause = BuildSelectClauseForType(targetType);

            foreach (var include in includes)
            {
                var fieldsFromInclude = BuildSelectClauseForInclude(targetType, include);
                selectClause = selectClause + Environment.NewLine + fieldsFromInclude;
            }

            return selectClause;
        }

        private static string BuildSelectClauseForInclude(Type targetType, string include, int depth = 1)
        {
            if (string.IsNullOrEmpty(include))
            {
                return BuildSelectClauseForType(targetType, depth);
            }
            var leftPadding = new string(' ', depth * 2);

            var dotIndex = include.IndexOf(".", StringComparison.InvariantCultureIgnoreCase);

            var restOfTheIncludePath = dotIndex >= 0 ? include.Substring(dotIndex + 1) : "";
            var currentPropertyName = dotIndex >= 0 ? include.Substring(0, dotIndex) : include;

            var propertyType = targetType.GetProperty(currentPropertyName).PropertyType.GetTypeOrListType();

            if (propertyType.IsPrimitiveOrString())
            {
                return leftPadding + currentPropertyName.ToCamelCase();
            }

            var fieldsFromInclude = BuildSelectClauseForInclude(propertyType, restOfTheIncludePath, depth + 1);
            fieldsFromInclude = $"{leftPadding}{currentPropertyName.ToCamelCase()} {{{Environment.NewLine}{fieldsFromInclude}{Environment.NewLine}{leftPadding}}}";
            return fieldsFromInclude;
        }
    }
}