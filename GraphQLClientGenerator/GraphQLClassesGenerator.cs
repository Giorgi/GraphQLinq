using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace GraphQLClientGenerator
{
    class GraphQLClassesGenerator
    {
        private static readonly Dictionary<string, string> TypeMapping = new Dictionary<string, string>()
        {
            { "Int", "int"},
            { "Float", "float"},
            { "String", "string"},
            { "Boolean", "bool"},
            { "Long", "long"}
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

        public void GenerateClasses(Schema schema, CodeGenerationOptions options)
        {
            var queryType = schema.QueryType.Name;
            var types = schema.Types.Where(type => !type.Name.StartsWith("__")
                                                                && !BuiltInTypes.Contains(type.Name)
                                                                && queryType != type.Name).ToList();

            var enums = types.Where(type => type.Kind == TypeKind.Enum);
            var classes = types.Where(type => type.Kind == TypeKind.Object);
            var interfaces = types.Where(type => type.Kind == TypeKind.Interface);

            foreach (var enumInfo in enums)
            {
                var syntax = GenerateEnum(enumInfo, options.Namespace);
                FormatAndWriteToFile(syntax, options.OutputDirectory, enumInfo.Name);
            }

            foreach (var classInfo in classes)
            {
                var syntax = GenerateClass(classInfo, options.Namespace);
                FormatAndWriteToFile(syntax, options.OutputDirectory, classInfo.Name);
            }

            foreach (var interfaceInfo in interfaces)
            {
                var syntax = GenerateInterface(interfaceInfo, options.Namespace);
                FormatAndWriteToFile(syntax, options.OutputDirectory, interfaceInfo.Name);
            }

            var classesWithArgFields = classes.Where(type => type.Fields.Any(field => field.Args.Any())).ToList();

            var queryExtensions = GenerateQueryExtensions(classesWithArgFields, options);
            FormatAndWriteToFile(queryExtensions, options.OutputDirectory, "QueryExtensions");
        }

        private SyntaxNode GenerateQueryExtensions(List<Type> classesWithArgFields, CodeGenerationOptions options)
        {
            var exceptionMessage = SyntaxFactory.Literal("This method is not implemented. It exists solely for query purposes.");
            var argumentListSyntax = SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, exceptionMessage))));

            var notImplemented = SyntaxFactory.ThrowStatement(SyntaxFactory.ObjectCreationExpression(
                SyntaxFactory.IdentifierName("NotImplementedException"), argumentListSyntax, null));

            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(options.Namespace));

            var usings = new HashSet<string>();
            
            var declaration = SyntaxFactory.ClassDeclaration("QueryExtensions")
                                           .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                           .AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));

            foreach (var type in classesWithArgFields)
            {
                foreach (var field in type.Fields.Where(f => f.Args.Any()))
                {
                    var returnType = GetSharpTypeName(field.Type);
                    usings.Add(returnType.Item2);

                    var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(returnType.Item1), field.Name)
                                                            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                                            .AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));

                    var methodParameters = new List<ParameterSyntax>();
                    
                    var thisParameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(type.Name.ToCamelCase()))
                                                            .WithType(SyntaxFactory.ParseTypeName(type.Name))
                                                            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword)));
                    methodParameters.Add(thisParameter);

                    foreach (var arg in field.Args)
                    {
                        var argumentType = GetSharpTypeName(arg.Type);
                        usings.Add(argumentType.Item2);

                        var parameterSyntax = SyntaxFactory.Parameter(SyntaxFactory.Identifier(arg.Name)).WithType(SyntaxFactory.ParseTypeName(argumentType.Item1));
                        methodParameters.Add(parameterSyntax);
                    }

                    methodDeclaration = methodDeclaration.AddParameterListParameters(methodParameters.ToArray())
                        .WithBody(SyntaxFactory.Block(notImplemented));

                    declaration = declaration.AddMembers(methodDeclaration);
                }
            }

            foreach (var @using in usings.Where(s => !string.IsNullOrEmpty(s)))
            {
                namespaceDeclaration = namespaceDeclaration.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(@using)));
            }

            namespaceDeclaration = namespaceDeclaration.AddMembers(declaration);

            return namespaceDeclaration;
        }


        private SyntaxNode GenerateEnum(Type enumInfo, string @namespace)
        {
            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(@namespace));
            var declaration = SyntaxFactory.EnumDeclaration(enumInfo.Name).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            foreach (var enumValue in enumInfo.EnumValues)
            {
                declaration = declaration.AddMembers(SyntaxFactory.EnumMemberDeclaration(SyntaxFactory.Identifier(enumValue.Name)));
            }

            return namespaceDeclaration.AddMembers(declaration);
        }

        private SyntaxNode GenerateClass(Type classInfo, string @namespace)
        {
            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(@namespace));

            var usings = new HashSet<string>();

            var semicolonToken = SyntaxFactory.Token(SyntaxKind.SemicolonToken);

            var declaration = SyntaxFactory.ClassDeclaration(classInfo.Name)
                                            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

            foreach (var @interface in classInfo.Interfaces)
            {
                declaration = declaration.AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(@interface.Name)));
            }

            foreach (var field in classInfo.Fields)
            {
                var typeInfo = GetSharpTypeName(field.Type);
                usings.Add(typeInfo.Item2);

                var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(typeInfo.Item1), field.Name)
                                            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                property = property.AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                   .WithSemicolonToken(semicolonToken));

                property = property.AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                   .WithSemicolonToken(semicolonToken));

                declaration = declaration.AddMembers(property);
            }

            foreach (var @using in usings.Where(s => !string.IsNullOrEmpty(s)))
            {
                namespaceDeclaration = namespaceDeclaration.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(@using)));
            }

            namespaceDeclaration = namespaceDeclaration.AddMembers(declaration);

            return namespaceDeclaration;
        }

        private SyntaxNode GenerateInterface(Type interfaceInfo, string @namespace)
        {
            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(@namespace));
            var semicolonToken = SyntaxFactory.Token(SyntaxKind.SemicolonToken);

            var declaration = SyntaxFactory.InterfaceDeclaration(interfaceInfo.Name)
                                            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            foreach (var field in interfaceInfo.Fields)
            {
                var typeInfo = GetSharpTypeName(field.Type);

                var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(typeInfo.Item1), field.Name);

                property = property.AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                   .WithSemicolonToken(semicolonToken));

                property = property.AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                   .WithSemicolonToken(semicolonToken));

                declaration = declaration.AddMembers(property);
            }

            return namespaceDeclaration.AddMembers(declaration);
        }

        private static void FormatAndWriteToFile(SyntaxNode syntax, string directory, string name)
        {
            var fileName = Path.Combine(directory, name + ".cs");
            using (var streamWriter = File.CreateText(fileName))
            {
                Formatter.Format(syntax, Workspace).WriteTo(streamWriter);
            }
        }

        private static Tuple<string, string> GetSharpTypeName(FieldType fieldType)
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
                    typeName = $"List<{GetSharpTypeName(fieldType.OfType).Item1}>";
                    return Tuple.Create(typeName, "System.Collections.Generic");
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

            return Tuple.Create(typeName, "");
        }

        private static string GetMappedType(string name)
        {
            return TypeMapping.ContainsKey(name) ? TypeMapping[name] : name;
        }
    }
}