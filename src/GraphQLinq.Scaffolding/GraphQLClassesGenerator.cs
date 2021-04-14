using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Spectre.Console;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace GraphQLinq.Scaffolding
{
    class GraphQLClassesGenerator
    {
        private Dictionary<string, string> renamedClasses = new();
        private readonly CodeGenerationOptions options;

        private static readonly Dictionary<string, string> TypeMapping = new()
        {
            { "Int", "int" },
            { "Float", "float" },
            { "String", "string" },
            { "ID", "string" },
            { "Date", "DateTime" },
            { "Boolean", "bool" },
            { "Long", "long" },
            { "uuid", "Guid" },
            { "timestamptz", "DateTimeOffset" },
        };

        private static readonly List<string> BuiltInTypes = new()
        {
            "ID",
            "Int",
            "Float",
            "String",
            "Boolean"
        };

        private static readonly AdhocWorkspace Workspace = new();

        public GraphQLClassesGenerator(CodeGenerationOptions options)
        {
            this.options = options;
        }

        public string GenerateClient(Schema schema, string endpointUrl)
        {
            var queryType = schema.QueryType.Name;
            var mutationType = schema.MutationType.Name;
            var subscriptionType = schema.SubscriptionType?.Name;

            var types = schema.Types.Where(type => !type.Name.StartsWith("__")
                                                                && !BuiltInTypes.Contains(type.Name)
                                                                && queryType != type.Name && mutationType != type.Name && subscriptionType != type.Name).ToList();

            var enums = types.Where(type => type.Kind == TypeKind.Enum);
            var classes = types.Where(type => type.Kind == TypeKind.Object || type.Kind == TypeKind.InputObject).OrderBy(type => type.Name);
            var interfaces = types.Where(type => type.Kind == TypeKind.Interface);

            AnsiConsole.WriteLine("Scaffolding enums ...");
            foreach (var enumInfo in enums)
            {
                var syntax = GenerateEnum(enumInfo);
                FormatAndWriteToFile(syntax, enumInfo.Name);
            }

            AnsiConsole.WriteLine("Scaffolding classes ...");
            foreach (var classInfo in classes)
            {
                var syntax = GenerateClass(classInfo);
                FormatAndWriteToFile(syntax, classInfo.Name);
            }

            AnsiConsole.WriteLine("Scaffolding interfaces ...");
            foreach (var interfaceInfo in interfaces)
            {
                var syntax = GenerateInterface(interfaceInfo);
                FormatAndWriteToFile(syntax, interfaceInfo.Name);
            }

            var classesWithArgFields = classes.Where(type => (type.Fields ?? new List<Field>()).Any(field => field.Args.Any())).ToList();

            AnsiConsole.WriteLine("Scaffolding Query Extensions ...");
            var queryExtensions = GenerateQueryExtensions(classesWithArgFields);
            FormatAndWriteToFile(queryExtensions, "QueryExtensions");

            var queryClass = schema.Types.Single(type => type.Name == queryType);

            AnsiConsole.WriteLine("Scaffolding GraphQLContext ...");
            var graphContext = GenerateGraphContext(queryClass, endpointUrl);
            FormatAndWriteToFile(graphContext, $"{options.ContextName}Context");

            return $"{options.ContextName}Context";
        }


        private SyntaxNode GenerateEnum(Type enumInfo)
        {
            var topLevelDeclaration = RoslynUtilities.GetTopLevelNode(options.Namespace);
            var name = enumInfo.Name.NormalizeIfNeeded(options);

            var declaration = EnumDeclaration(name).AddModifiers(Token(SyntaxKind.PublicKeyword));

            foreach (var enumValue in enumInfo.EnumValues)
            {
                declaration = declaration.AddMembers(EnumMemberDeclaration(Identifier(enumValue.Name)));
            }

            return topLevelDeclaration.AddMembers(declaration);
        }

        private SyntaxNode GenerateClass(Type classInfo)
        {
            var topLevelDeclaration = RoslynUtilities.GetTopLevelNode(options.Namespace);

            var usings = new HashSet<string>();

            var semicolonToken = Token(SyntaxKind.SemicolonToken);

            var className = classInfo.Name.NormalizeIfNeeded(options);

            var declaration = ClassDeclaration(className)
                                            .AddModifiers(Token(SyntaxKind.PublicKeyword))
                                            .AddModifiers(Token(SyntaxKind.PartialKeyword));

            foreach (var @interface in classInfo.Interfaces ?? new List<Type>())
            {
                declaration = declaration.AddBaseListTypes(SimpleBaseType(ParseTypeName(@interface.Name)));
            }

            foreach (var field in classInfo.Fields ?? classInfo.InputFields ?? new List<Field>())
            {
                var fieldName = field.Name.NormalizeIfNeeded(options);

                if (fieldName == className)
                {
                    declaration = declaration.ReplaceToken(declaration.Identifier, Identifier($"{className}Type"));
                    renamedClasses.Add(className, $"{className}Type");
                }

                var (fieldType, @namespace) = GetSharpTypeName(field.Type);
                usings.Add(@namespace);

                if (NeedsNullable(fieldType, field.Type))
                {
                    fieldType += "?";
                }

                var property = PropertyDeclaration(ParseTypeName(fieldType), fieldName)
                                            .AddModifiers(Token(SyntaxKind.PublicKeyword));

                property = property.AddAccessorListAccessors(AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                   .WithSemicolonToken(semicolonToken));

                property = property.AddAccessorListAccessors(AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                   .WithSemicolonToken(semicolonToken));

                declaration = declaration.AddMembers(property);
            }

            foreach (var @using in usings.Where(s => !string.IsNullOrEmpty(s)))
            {
                topLevelDeclaration = topLevelDeclaration.AddUsings(UsingDirective(IdentifierName(@using)));
            }

            topLevelDeclaration = topLevelDeclaration.AddMembers(declaration);

            return topLevelDeclaration;
        }

        private SyntaxNode GenerateInterface(Type interfaceInfo)
        {
            var topLevelDeclaration = RoslynUtilities.GetTopLevelNode(options.Namespace);
            var semicolonToken = Token(SyntaxKind.SemicolonToken);

            var name = interfaceInfo.Name.NormalizeIfNeeded(options);

            var declaration = InterfaceDeclaration(name)
                                            .AddModifiers(Token(SyntaxKind.PublicKeyword));

            foreach (var field in interfaceInfo.Fields)
            {
                var (type, _) = GetSharpTypeName(field.Type);

                var fieldName = field.Name.NormalizeIfNeeded(options);

                var property = PropertyDeclaration(ParseTypeName(type), fieldName);

                property = property.AddAccessorListAccessors(AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                   .WithSemicolonToken(semicolonToken));

                property = property.AddAccessorListAccessors(AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                   .WithSemicolonToken(semicolonToken));

                declaration = declaration.AddMembers(property);
            }

            return topLevelDeclaration.AddMembers(declaration);
        }

        private SyntaxNode GenerateQueryExtensions(List<Type> classesWithArgFields)
        {
            var exceptionMessage = Literal("This method is not implemented. It exists solely for query purposes.");
            var argumentListSyntax = ArgumentList(SingletonSeparatedList(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, exceptionMessage))));

            var notImplemented = ThrowStatement(ObjectCreationExpression(IdentifierName("NotImplementedException"), argumentListSyntax, null));

            var topLevelDeclaration = RoslynUtilities.GetTopLevelNode(options.Namespace);

            var usings = new HashSet<string> { "System" };

            var declaration = ClassDeclaration("QueryExtensions")
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddModifiers(Token(SyntaxKind.StaticKeyword));

            foreach (var @class in classesWithArgFields)
            {
                foreach (var field in @class.Fields.Where(f => f.Args.Any()))
                {
                    var (type, @namespace) = GetSharpTypeName(field.Type);
                    usings.Add(@namespace);

                    var fieldName = field.Name.NormalizeIfNeeded(options);

                    var methodDeclaration = MethodDeclaration(ParseTypeName(type), fieldName)
                                             .AddModifiers(Token(SyntaxKind.PublicKeyword))
                                             .AddModifiers(Token(SyntaxKind.StaticKeyword));

                    var thisParameter = Parameter(Identifier(@class.Name.ToCamelCase()))
                                             .WithType(ParseTypeName(@class.Name.NormalizeIfNeeded(options)))
                                             .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)));

                    var methodParameters = new List<ParameterSyntax> { thisParameter };

                    foreach (var arg in field.Args)
                    {
                        (type, @namespace) = GetSharpTypeName(arg.Type);
                        usings.Add(@namespace);

                        var typeName = TypeMapping.ContainsValue(type) ? type : type.NormalizeIfNeeded(options);
                        var parameterSyntax = Parameter(Identifier(arg.Name)).WithType(ParseTypeName(typeName));
                        methodParameters.Add(parameterSyntax);
                    }

                    methodDeclaration = methodDeclaration.AddParameterListParameters(methodParameters.ToArray())
                                             .WithBody(Block(notImplemented));

                    declaration = declaration.AddMembers(methodDeclaration);
                }
            }

            foreach (var @using in usings.Where(s => !string.IsNullOrEmpty(s)))
            {
                topLevelDeclaration = topLevelDeclaration.AddUsings(UsingDirective(IdentifierName(@using)));
            }

            topLevelDeclaration = topLevelDeclaration.AddMembers(declaration);

            return topLevelDeclaration;
        }

        private SyntaxNode GenerateGraphContext(Type queryInfo, string endpointUrl)
        {
            var topLevelDeclaration = RoslynUtilities.GetTopLevelNode(options.Namespace);

            var usings = new HashSet<string> { "System", "GraphQLinq" };

            var className = $"{options.ContextName}Context";
            var declaration = ClassDeclaration(className)
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddBaseListTypes(SimpleBaseType(ParseTypeName("GraphContext")));

            var thisInitializer = ConstructorInitializer(SyntaxKind.ThisConstructorInitializer)
                                    .AddArgumentListArguments(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(endpointUrl))));

            var defaultConstructorDeclaration = ConstructorDeclaration(className)
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .WithInitializer(thisInitializer)
                .WithBody(Block());

            var baseInitializer = ConstructorInitializer(SyntaxKind.BaseConstructorInitializer)
                                    .AddArgumentListArguments(Argument(IdentifierName("baseUrl")),
                                                              Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(""))));

            var constructorDeclaration = ConstructorDeclaration(className)
                                    .AddModifiers(Token(SyntaxKind.PublicKeyword))
                                    .AddParameterListParameters(Parameter(Identifier("baseUrl")).WithType(ParseTypeName("string")))
                                    .WithInitializer(baseInitializer)
                                    .WithBody(Block());

            declaration = declaration.AddMembers(defaultConstructorDeclaration, constructorDeclaration);

            foreach (var field in queryInfo.Fields)
            {
                var (type, @namespace) = GetSharpTypeName(field.Type.Kind == TypeKind.NonNull ? field.Type.OfType : field.Type, true);
                usings.Add(@namespace);

                var baseMethodName = type.Replace("GraphItemQuery", "BuildItemQuery")
                                         .Replace("GraphCollectionQuery", "BuildCollectionQuery");

                var fieldName = field.Name.NormalizeIfNeeded(options);

                var methodDeclaration = MethodDeclaration(ParseTypeName(type), fieldName)
                                            .AddModifiers(Token(SyntaxKind.PublicKeyword));

                var methodParameters = new List<ParameterSyntax>();

                var initializer = InitializerExpression(SyntaxKind.ArrayInitializerExpression);

                foreach (var arg in field.Args)
                {
                    (type, @namespace) = GetSharpTypeName(arg.Type);
                    usings.Add(@namespace);

                    var typeName = TypeMapping.ContainsValue(type) ? type : type.NormalizeIfNeeded(options);

                    if (NeedsNullable(typeName, arg.Type))
                    {
                        typeName += "?";
                    }

                    var parameterSyntax = Parameter(Identifier(arg.Name)).WithType(ParseTypeName(typeName));
                    methodParameters.Add(parameterSyntax);

                    initializer = initializer.AddExpressions(IdentifierName(arg.Name));
                }

                var paramsArray = ArrayCreationExpression(ArrayType(ParseTypeName("object[]")), initializer);

                var parametersDeclaration = LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
                                            .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("parameterValues"))
                                            .WithInitializer(EqualsValueClause(paramsArray)))));

                var returnStatement = ReturnStatement(InvocationExpression(IdentifierName(baseMethodName))
                                            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("parameterValues"))))));

                methodDeclaration = methodDeclaration.AddParameterListParameters(methodParameters.ToArray())
                                                        .WithBody(Block(parametersDeclaration, returnStatement));

                declaration = declaration.AddMembers(methodDeclaration);
            }


            foreach (var @using in usings.Where(s => !string.IsNullOrEmpty(s)))
            {
                topLevelDeclaration = topLevelDeclaration.AddUsings(UsingDirective(IdentifierName(@using)));
            }

            topLevelDeclaration = topLevelDeclaration.AddMembers(declaration);

            return topLevelDeclaration;
        }

        private static bool NeedsNullable(string typeName, FieldType type)
        {
            if (type.Kind == TypeKind.Scalar && TypeMapping.ContainsValue(typeName))
            {
                //Hacky way to return false for string and null for primitive types.
                //For int, bool and other types Type.GetType will return null but not for string.
                var builtInType = System.Type.GetType($"System.{typeName}", false, true);

                if (builtInType == null || builtInType.IsValueType)
                {
                    return true;
                }
            }

            return false;
        }


        private void FormatAndWriteToFile(SyntaxNode syntax, string name)
        {
            if (!Directory.Exists(options.OutputDirectory))
            {
                Directory.CreateDirectory(options.OutputDirectory);
            }

            name = name.NormalizeIfNeeded(options);

            var fileName = Path.Combine(options.OutputDirectory, name + ".cs");
            using (var streamWriter = File.CreateText(fileName))
            {
                Formatter.Format(syntax, Workspace).WriteTo(streamWriter);
            }
        }

        private (string type, string @namespace) GetSharpTypeName(FieldType? fieldType, bool wrapWithGraphTypes = false)
        {
            if (fieldType == null)
            {
                throw new NotImplementedException("ofType nested more than three levels not implemented");
            }

            var typeName = fieldType.Name;

            if (typeName == null)
            {
                if (fieldType.Kind == TypeKind.List)
                {
                    var type = GetSharpTypeName(fieldType.OfType).type;
                    typeName = wrapWithGraphTypes ? $"GraphCollectionQuery<{type}>" : $"List<{type}>";

                    return (typeName, "System.Collections.Generic");
                }

                if (fieldType.Kind == TypeKind.NonNull && fieldType.OfType?.Name?.ToUpper() == "ID")
                {
                    typeName = "string";
                }
                else
                {
                    return GetSharpTypeName(fieldType.OfType);
                }
            }
            else
            {
                typeName = GetMappedType(fieldType.Name);
            }

            if (wrapWithGraphTypes)
            {
                typeName = $"GraphItemQuery<{typeName}>";
            }

            if (renamedClasses.ContainsKey(typeName))
            {
                typeName = renamedClasses[typeName];
            }

            return (typeName, typeName == "Guid" || typeName == "DateTime" || typeName == "DateTimeOffset" ? "System" : "");
        }


        private string GetMappedType(string name)
        {
            return TypeMapping.ContainsKey(name) ? TypeMapping[name] : name.NormalizeIfNeeded(options);
        }
    }
}