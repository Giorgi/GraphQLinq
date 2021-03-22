using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace GraphQLClientGenerator
{
    class GraphQLClassesGenerator
    {
        private readonly CodeGenerationOptions options;

        private static readonly Dictionary<string, string> TypeMapping = new Dictionary<string, string>()
        {
            { "Int", "int"},
            { "Float", "float"},
            { "String", "string"},
            { "ID", "string"},
            { "Date", "DateTime"},
            { "Boolean", "bool"},
            { "Long", "long"},
            { "uuid", "Guid"},
            { "timestamptz", "DateTimeOffset"},
        };

        private static readonly List<string> BuiltInTypes = new List<string>
        {
            "ID",
            "Int",
            "Float",
            "String",
            "Boolean"
        };

        private static readonly AdhocWorkspace Workspace = new AdhocWorkspace();

        public GraphQLClassesGenerator(CodeGenerationOptions options)
        {
            this.options = options;
        }

        public void GenerateClasses(Schema schema)
        {
            var queryType = schema.QueryType.Name;
            var mutationType = schema.MutationType.Name;
            var subscriptionType = schema.SubscriptionType.Name;

            var types = schema.Types.Where(type => !type.Name.StartsWith("__")
                                                                && !BuiltInTypes.Contains(type.Name)
                                                                && queryType != type.Name && mutationType != type.Name && subscriptionType != type.Name).ToList();

            var enums = types.Where(type => type.Kind == TypeKind.Enum);
            var classes = types.Where(type => type.Kind == TypeKind.Object || type.Kind == TypeKind.InputObject);
            var interfaces = types.Where(type => type.Kind == TypeKind.Interface);

            foreach (var enumInfo in enums)
            {
                var syntax = GenerateEnum(enumInfo);
                FormatAndWriteToFile(syntax, options.OutputDirectory, enumInfo.Name);
            }

            foreach (var classInfo in classes)
            {
                var syntax = GenerateClass(classInfo);
                FormatAndWriteToFile(syntax, options.OutputDirectory, classInfo.Name);
            }

            foreach (var interfaceInfo in interfaces)
            {
                var syntax = GenerateInterface(interfaceInfo);
                FormatAndWriteToFile(syntax, options.OutputDirectory, interfaceInfo.Name);
            }

            var classesWithArgFields = classes.Where(type => type.Fields.Any(field => field.Args.Any())).ToList();

            var queryExtensions = GenerateQueryExtensions(classesWithArgFields);
            FormatAndWriteToFile(queryExtensions, options.OutputDirectory, "QueryExtensions");
        }


        private SyntaxNode GenerateEnum(Type enumInfo)
        {
            var namespaceDeclaration = NamespaceDeclaration(IdentifierName(options.Namespace));
            var name = options.NormalizeCasing ? enumInfo.Name.ToPascalCase() : enumInfo.Name;

            var declaration = EnumDeclaration(name).AddModifiers(Token(SyntaxKind.PublicKeyword));

            foreach (var enumValue in enumInfo.EnumValues)
            {
                declaration = declaration.AddMembers(EnumMemberDeclaration(Identifier(enumValue.Name)));
            }

            return namespaceDeclaration.AddMembers(declaration);
        }

        private SyntaxNode GenerateClass(Type classInfo)
        {
            var namespaceDeclaration = NamespaceDeclaration(IdentifierName(options.Namespace));

            var usings = new HashSet<string>();

            var semicolonToken = Token(SyntaxKind.SemicolonToken);

            var name = options.NormalizeCasing ? classInfo.Name.ToPascalCase() : classInfo.Name;

            var declaration = ClassDeclaration(name)
                                            .AddModifiers(Token(SyntaxKind.PublicKeyword))
                                            .AddModifiers(Token(SyntaxKind.PartialKeyword));

            foreach (var @interface in classInfo.Interfaces)
            {
                declaration = declaration.AddBaseListTypes(SimpleBaseType(ParseTypeName(@interface.Name)));
            }

            foreach (var field in classInfo.Fields ?? classInfo.InputFields ?? new List<Field>())
            {
                var (type, @namespace) = GetSharpTypeName(field.Type);
                usings.Add(@namespace);

                var fieldName = options.NormalizeCasing ? field.Name.ToPascalCase() : field.Name;

                var property = PropertyDeclaration(ParseTypeName(type), fieldName)
                                            .AddModifiers(Token(SyntaxKind.PublicKeyword));

                property = property.AddAccessorListAccessors(AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                   .WithSemicolonToken(semicolonToken));

                property = property.AddAccessorListAccessors(AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                   .WithSemicolonToken(semicolonToken));

                declaration = declaration.AddMembers(property);
            }

            foreach (var @using in usings.Where(s => !string.IsNullOrEmpty(s)))
            {
                namespaceDeclaration = namespaceDeclaration.AddUsings(UsingDirective(IdentifierName(@using)));
            }

            namespaceDeclaration = namespaceDeclaration.AddMembers(declaration);

            return namespaceDeclaration;
        }

        private SyntaxNode GenerateInterface(Type interfaceInfo)
        {
            var namespaceDeclaration = NamespaceDeclaration(IdentifierName(options.Namespace));
            var semicolonToken = Token(SyntaxKind.SemicolonToken);

            var name = options.NormalizeCasing ? interfaceInfo.Name.ToPascalCase() : interfaceInfo.Name;

            var declaration = InterfaceDeclaration(name)
                                            .AddModifiers(Token(SyntaxKind.PublicKeyword));

            foreach (var field in interfaceInfo.Fields)
            {
                var (type, _) = GetSharpTypeName(field.Type);

                var fieldName = options.NormalizeCasing ? field.Name.ToPascalCase() : field.Name;

                var property = PropertyDeclaration(ParseTypeName(type), fieldName);

                property = property.AddAccessorListAccessors(AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                   .WithSemicolonToken(semicolonToken));

                property = property.AddAccessorListAccessors(AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                   .WithSemicolonToken(semicolonToken));

                declaration = declaration.AddMembers(property);
            }

            return namespaceDeclaration.AddMembers(declaration);
        }

        private SyntaxNode GenerateQueryExtensions(List<Type> classesWithArgFields)
        {
            var exceptionMessage = Literal("This method is not implemented. It exists solely for query purposes.");
            var argumentListSyntax = ArgumentList(SingletonSeparatedList(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, exceptionMessage))));

            var notImplemented = ThrowStatement(ObjectCreationExpression(IdentifierName("NotImplementedException"), argumentListSyntax, null));

            var namespaceDeclaration = NamespaceDeclaration(IdentifierName(options.Namespace));

            var usings = new HashSet<string>();

            var declaration = ClassDeclaration("QueryExtensions")
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddModifiers(Token(SyntaxKind.StaticKeyword));

            foreach (var @class in classesWithArgFields)
            {
                foreach (var field in @class.Fields.Where(f => f.Args.Any()))
                {
                    var (type, @namespace) = GetSharpTypeName(field.Type);
                    usings.Add(@namespace);

                    var fieldName = options.NormalizeCasing ? field.Name.ToPascalCase() : field.Name;

                    var methodDeclaration = MethodDeclaration(ParseTypeName(type), fieldName)
                                             .AddModifiers(Token(SyntaxKind.PublicKeyword))
                                             .AddModifiers(Token(SyntaxKind.StaticKeyword));

                    var thisParameter = Parameter(Identifier(@class.Name.ToCamelCase()))
                                             .WithType(ParseTypeName(@class.Name))
                                             .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)));

                    var methodParameters = new List<ParameterSyntax> { thisParameter };

                    foreach (var arg in field.Args)
                    {
                        (type, @namespace) = GetSharpTypeName(arg.Type);
                        usings.Add(@namespace);

                        var parameterSyntax = Parameter(Identifier(arg.Name)).WithType(ParseTypeName(type));
                        methodParameters.Add(parameterSyntax);
                    }

                    methodDeclaration = methodDeclaration.AddParameterListParameters(methodParameters.ToArray())
                                             .WithBody(Block(notImplemented));

                    declaration = declaration.AddMembers(methodDeclaration);
                }
            }

            foreach (var @using in usings.Where(s => !string.IsNullOrEmpty(s)))
            {
                namespaceDeclaration = namespaceDeclaration.AddUsings(UsingDirective(IdentifierName(@using)));
            }

            namespaceDeclaration = namespaceDeclaration.AddMembers(declaration);

            return namespaceDeclaration;
        }


        private void FormatAndWriteToFile(SyntaxNode syntax, string directory, string name)
        {
            name = options.NormalizeCasing ? name.ToPascalCase() : name;

            var fileName = Path.Combine(directory, name + ".cs");
            using (var streamWriter = File.CreateText(fileName))
            {
                Formatter.Format(syntax, Workspace).WriteTo(streamWriter);
            }
        }

        private static (string type, string @namespace) GetSharpTypeName(FieldType fieldType)
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
                    typeName = $"List<{GetSharpTypeName(fieldType.OfType).type}>";
                    return (typeName, "System.Collections.Generic");
                }

                if (fieldType.Kind == TypeKind.NonNull && fieldType.OfType?.Name == "ID")
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

            return (typeName, typeName == "Guid" || typeName == "DateTime" || typeName == "DateTimeOffset" ? "System" : "");
        }


        private static string GetMappedType(string name)
        {
            return TypeMapping.ContainsKey(name) ? TypeMapping[name] : name;
        }
    }
}