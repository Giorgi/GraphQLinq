using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace GrapQLClientGenerator
{
    class GraphQLClassesGenerator
    {
        private static readonly Dictionary<string, string> TypeMapping = new Dictionary<string, string>()
        {
            { "Int", "int"},
            { "Float", "float"},
            { "String", "string"},
            { "Boolean", "bool"},
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
            "Query",
            "Node"
        };

        private AdhocWorkspace workspace = new AdhocWorkspace();

        public void GenerateClasses(RootObject rootObject)
        {
            var types = rootObject.Data.Schema.Types.Where(type => !type.Name.StartsWith("__")
                                                                && !BuiltInTypes.Contains(type.Name)
                                                                && !ExceptionTypes.Contains(type.Name)).ToList();

            var enums = types.Where(type => type.Kind == TypeKind.Enum);
            var classes = types.Where(type => type.Kind == TypeKind.Object);

            foreach (var enumInfo in enums)
            {
                var enumSyntax = GenerateEnum(enumInfo);
                using (var streamWriter = File.CreateText(enumInfo.Name + ".cs"))
                {
                    enumSyntax.WriteTo(streamWriter);
                }
            }

            foreach (var classInfo in classes)
            {
                var enumSyntax = GenerateClass(classInfo);
                using (var streamWriter = File.CreateText(classInfo.Name + ".cs"))
                {
                    enumSyntax.WriteTo(streamWriter);
                }
            }
        }

        private SyntaxNode GenerateEnum(Type enumInfo)
        {
            var declaration = SyntaxFactory.EnumDeclaration(enumInfo.Name).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            foreach (var enumValue in enumInfo.EnumValues)
            {
                declaration = declaration.AddMembers(SyntaxFactory.EnumMemberDeclaration(SyntaxFactory.Identifier(enumValue.Name)));
            }

            return Formatter.Format(declaration, workspace);
        }

        private SyntaxNode GenerateClass(Type classInfo)
        {
            var usings = new HashSet<string>();
            var compilationUnit = SyntaxFactory.CompilationUnit();

            var semicolonToken = SyntaxFactory.Token(SyntaxKind.SemicolonToken);

            var declaration = SyntaxFactory.ClassDeclaration(classInfo.Name)
                                            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

            foreach (var field in classInfo.Fields)
            {
                var typeInfo = GetSharpTypeName(field);
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

            return Formatter.Format(compilationUnit, workspace);
        }

        private static Tuple<string, string> GetSharpTypeName(Field field)
        {
            var typeName = field.Type.Name;

            if (typeName == null)
            {
                if (field.Type.Kind == TypeKind.List)
                {
                    typeName = $"List<{GetMappedType(field.Type.OfType.Name)}>";
                    return Tuple.Create(typeName, "System.Collections.Generic");
                }

                if (field.Type.Kind == TypeKind.NonNull)
                {
                    typeName = "string";
                }
            }
            else
            {
                typeName = GetMappedType(field.Type.Name);
            }

            return Tuple.Create(typeName, "");
        }

        private static string GetMappedType(string name)
        {
            return TypeMapping.ContainsKey(name) ? TypeMapping[name] : name;
        }
    }
}