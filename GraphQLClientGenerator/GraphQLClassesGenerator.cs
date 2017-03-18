using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

        private static readonly List<string> ExceptionTypes = new List<string>
        {
            "Query"
        };

        private static readonly AdhocWorkspace Workspace = new AdhocWorkspace();

        public void GenerateClasses(Schema schema)
        {
            var types = schema.Types.Where(type => !type.Name.StartsWith("__")
                                                                && !BuiltInTypes.Contains(type.Name)
                                                                && !ExceptionTypes.Contains(type.Name)).ToList();

            var enums = types.Where(type => type.Kind == TypeKind.Enum);
            var classes = types.Where(type => type.Kind == TypeKind.Object);
            var interfaces = types.Where(type => type.Kind == TypeKind.Interface);

            foreach (var enumInfo in enums)
            {
                var syntax = GenerateEnum(enumInfo);
                FormatAndWriteToFile(syntax, enumInfo.Name);
            }

            foreach (var classInfo in classes)
            {
                var syntax = GenerateClass(classInfo);
                FormatAndWriteToFile(syntax, classInfo.Name);
            }

            foreach (var interfaceInfo in interfaces)
            {
                var syntax = GenerateInterface(interfaceInfo);
                FormatAndWriteToFile(syntax, interfaceInfo.Name);
            }
        }

        private SyntaxNode GenerateEnum(Type enumInfo)
        {
            var declaration = SyntaxFactory.EnumDeclaration(enumInfo.Name).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            foreach (var enumValue in enumInfo.EnumValues)
            {
                declaration = declaration.AddMembers(SyntaxFactory.EnumMemberDeclaration(SyntaxFactory.Identifier(enumValue.Name)));
            }

            return declaration;
        }

        private SyntaxNode GenerateClass(Type classInfo)
        {
            var usings = new HashSet<string>();
            var compilationUnit = SyntaxFactory.CompilationUnit();

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
                compilationUnit = compilationUnit.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(@using)));
            }

            compilationUnit = compilationUnit.AddMembers(declaration);

            return compilationUnit;
        }

        private SyntaxNode GenerateInterface(Type interfaceInfo)
        {
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

            return declaration;
        }

        private static void FormatAndWriteToFile(SyntaxNode syntax, string name)
        {
            using (var streamWriter = File.CreateText(name + ".cs"))
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