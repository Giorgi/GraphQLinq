using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphQLinq
{
    class GraphQueryBuilder<T>
    {
        private const string QueryTemplate = @"query {0} {{ {1}: {2} {3} {{ {4} }}}}";
        internal const string ResultAlias = "result";

        public GraphQLQuery BuildQuery(GraphQuery<T> graphQuery, List<IncludeDetails> includes)
        {
            var selectClause = "";

            var passedArguments = graphQuery.Arguments.Where(pair => pair.Value != null).ToList();
            var queryVariables = passedArguments.ToDictionary(pair => pair.Key, pair => pair.Value);

            if (graphQuery.Selector != null)
            {
                var body = graphQuery.Selector.Body;

                var padding = new string(' ', 4);

                var fields = new List<string>();

                switch (body)
                {
                    case MemberExpression memberExpression:
                        var member = memberExpression.Member;
                        selectClause = BuildMemberAccessSelectClause(body, selectClause, padding, member.Name);
                        break;

                    case NewExpression newExpression:
                        foreach (var argument in newExpression.Arguments.OfType<MemberExpression>())
                        {
                            var selectField = BuildMemberAccessSelectClause(argument, selectClause, padding, argument.Member.Name);
                            fields.Add(selectField);
                        }
                        selectClause = string.Join(Environment.NewLine, fields);
                        break;

                    case MemberInitExpression memberInitExpression:
                        foreach (var argument in memberInitExpression.Bindings.OfType<MemberAssignment>())
                        {
                            var selectField = BuildMemberAccessSelectClause(argument.Expression, selectClause, padding, argument.Member.Name);
                            fields.Add(selectField);
                        }
                        selectClause = string.Join(Environment.NewLine, fields);
                        break;
                    default:
                        throw new NotSupportedException($"Selector of type {body.NodeType} is not implemented yet");
                }
            }
            else
            {
                var select = BuildSelectClauseForType(typeof(T), includes);
                selectClause = select.SelectClause;

                foreach (var item in select.IncludeArguments)
                {
                    queryVariables.Add(item.Key, item.Value);
                }
            }

            selectClause = Environment.NewLine + selectClause + Environment.NewLine;

            var queryParameters = passedArguments.Any() ? $"({string.Join(", ", passedArguments.Select(pair => $"{pair.Key}: ${pair.Key}"))})" : "";
            var queryParameterTypes = queryVariables.Any() ? $"({string.Join(", ", queryVariables.Select(pair => $"${pair.Key}: {pair.Value.GetType().ToGraphQlType()}"))})" : "";

            var graphQLQuery = string.Format(QueryTemplate, queryParameterTypes, ResultAlias, graphQuery.QueryName, queryParameters, selectClause);

            var dictionary = new Dictionary<string, object> { { "query", graphQLQuery }, { "variables", queryVariables } };

            var json = JsonSerializer.Serialize(dictionary, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            });

            return new GraphQLQuery(graphQLQuery, queryVariables, json);
        }

        private static string BuildMemberAccessSelectClause(Expression body, string selectClause, string padding, string alias)
        {
            if (body is MemberExpression memberExpression)
            {
                var member = memberExpression.Member as PropertyInfo;

                if (member != null)
                {
                    if (string.IsNullOrEmpty(selectClause))
                    {
                        selectClause = $"{padding}{alias}: {member.Name.ToCamelCase()}";

                        if (!member.PropertyType.GetTypeOrListType().IsValueTypeOrString())
                        {
                            var fieldForProperty = BuildSelectClauseForType(member.PropertyType.GetTypeOrListType(), 3);
                            selectClause = $"{selectClause} {{{Environment.NewLine}{fieldForProperty}{Environment.NewLine}{padding}}}";
                        }
                    }
                    else
                    {
                        selectClause = $"{member.Name.ToCamelCase()} {{ {Environment.NewLine}{selectClause}}}";
                    }
                    return BuildMemberAccessSelectClause(memberExpression.Expression, selectClause, padding, "");
                }
                return selectClause;
            }
            return selectClause;
        }

        private static string BuildSelectClauseForType(Type targetType, int depth = 1)
        {
            var propertyInfos = targetType.GetProperties();

            var propertiesToInclude = propertyInfos.Where(info => !info.PropertyType.HasNestedProperties());

            var selectClause = string.Join(Environment.NewLine, propertiesToInclude.Select(info => new string(' ', depth * 2) + info.Name.ToCamelCase()));

            return selectClause;
        }

        private static SelectClauseDetails BuildSelectClauseForType(Type targetType, List<IncludeDetails> includes)
        {
            var selectClause = BuildSelectClauseForType(targetType);
            var includeVariables = new Dictionary<string, object>();

            for (var index = 0; index < includes.Count; index++)
            {
                var include = includes[index];
                var prefix = includes.Count == 1 ? "" : index.ToString();

                var fieldsFromInclude = BuildSelectClauseForInclude(targetType, include, includeVariables, prefix);
                selectClause = selectClause + Environment.NewLine + fieldsFromInclude;
            }
            return new SelectClauseDetails { SelectClause = selectClause, IncludeArguments = includeVariables };
        }

        private static string BuildSelectClauseForInclude(Type targetType, IncludeDetails includeDetails, Dictionary<string, object> includeVariables, string parameterPrefix = "", int parameterIndex = 0, int depth = 1)
        {
            var include = includeDetails.Path;
            if (string.IsNullOrEmpty(include))
            {
                return BuildSelectClauseForType(targetType, depth);
            }
            var leftPadding = new string(' ', depth * 2);

            var dotIndex = include.IndexOf(".", StringComparison.InvariantCultureIgnoreCase);

            var currentIncludeName = dotIndex >= 0 ? include.Substring(0, dotIndex) : include;

            Type propertyType;
            var propertyInfo = targetType.GetProperty(currentIncludeName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            var includeName = currentIncludeName.ToCamelCase();

            var includeMethodInfo = includeDetails.MethodIncludes.Count > parameterIndex ? includeDetails.MethodIncludes[parameterIndex].Method : null;
            var includeByMethod = includeMethodInfo != null && currentIncludeName == includeMethodInfo.Name && propertyInfo.PropertyType == includeMethodInfo.ReturnType;

            if (includeByMethod)
            {
                var methodDetails = includeDetails.MethodIncludes[parameterIndex];
                parameterIndex++;

                propertyType = methodDetails.Method.ReturnType.GetTypeOrListType();

                var includeMethodParams = methodDetails.Parameters.Where(pair => pair.Value != null).ToList();
                includeName = methodDetails.Method.Name.ToCamelCase();

                if (includeMethodParams.Any())
                {
                    var includeParameters = string.Join(", ", includeMethodParams.Select(pair => pair.Key + ": $" + pair.Key + parameterPrefix + parameterIndex));
                    includeName = $"{includeName}({includeParameters})";

                    foreach (var item in includeMethodParams)
                    {
                        includeVariables.Add(item.Key + parameterPrefix + parameterIndex, item.Value);
                    }
                }
            }
            else
            {
                propertyType = propertyInfo.PropertyType.GetTypeOrListType();
            }

            if (propertyType.IsValueTypeOrString())
            {
                return leftPadding + includeName;
            }

            var restOfTheInclude = new IncludeDetails(includeDetails.MethodIncludes) { Path = dotIndex >= 0 ? include.Substring(dotIndex + 1) : "" };

            var fieldsFromInclude = BuildSelectClauseForInclude(propertyType, restOfTheInclude, includeVariables, parameterPrefix, parameterIndex, depth + 1);
            fieldsFromInclude = $"{leftPadding}{includeName} {{{Environment.NewLine}{fieldsFromInclude}{Environment.NewLine}{leftPadding}}}";
            return fieldsFromInclude;
        }
    }

    class GraphQLQuery
    {
        public GraphQLQuery(string query, IReadOnlyDictionary<string, object> variables, string fullQuery)
        {
            Query = query;
            Variables = variables;
            FullQuery = fullQuery;
        }

        public string Query { get; }
        public string FullQuery { get; }
        public IReadOnlyDictionary<string, object> Variables { get; }
    }

    class SelectClauseDetails
    {
        public string SelectClause { get; set; }
        public Dictionary<string, object> IncludeArguments { get; set; }
    }
}