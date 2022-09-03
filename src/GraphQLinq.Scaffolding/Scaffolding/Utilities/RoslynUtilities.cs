using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace GraphQLinq.Scaffolding;

static class RoslynUtilities
{
    internal static SyntaxNode GetTopLevelNode(string? @namespace)
    {
        return string.IsNullOrEmpty(@namespace)
            ? CompilationUnit()
            : NamespaceDeclaration(IdentifierName(@namespace));
    }

    internal static SyntaxNode AddUsings(this SyntaxNode node, UsingDirectiveSyntax usingDirectiveSyntax)
    {
        switch (node)
        {
            case CompilationUnitSyntax compilationUnit:
                compilationUnit = compilationUnit.AddUsings(usingDirectiveSyntax);
                return compilationUnit;
            case NamespaceDeclarationSyntax namespaceDeclaration:
                namespaceDeclaration = namespaceDeclaration.AddUsings(usingDirectiveSyntax);
                return namespaceDeclaration;
            default:
                return node;
        }
    }

    internal static SyntaxNode AddMembers(this SyntaxNode node, MemberDeclarationSyntax memberDeclarationSyntax)
    {
        switch (node)
        {
            case CompilationUnitSyntax compilationUnit:
                compilationUnit = compilationUnit.AddMembers(memberDeclarationSyntax);
                return compilationUnit;
            case NamespaceDeclarationSyntax namespaceDeclaration:
                namespaceDeclaration = namespaceDeclaration.AddMembers(memberDeclarationSyntax);
                return namespaceDeclaration;
            default:
                return node;
        }
    }
}
