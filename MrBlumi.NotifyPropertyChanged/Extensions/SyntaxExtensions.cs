using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrBlumi.NotifyPropertyChanged.Helpers;
using MrBlumi.NotifyPropertyChanged.Models;

namespace MrBlumi.NotifyPropertyChanged.Extensions;

internal static class SyntaxExtensions
{
    public static string? GetNamespace(this BaseTypeDeclarationSyntax syntax)
    {
        var nameSpace = default(string);
        var potentialNameSpace = syntax.Parent;

        while (potentialNameSpace is not null and not BaseNamespaceDeclarationSyntax)
            potentialNameSpace = potentialNameSpace.Parent;

        while (potentialNameSpace is BaseNamespaceDeclarationSyntax parent)
        {
            nameSpace = $"{parent.Name}.{nameSpace}";
            potentialNameSpace = potentialNameSpace.Parent;
        }

        nameSpace = nameSpace?.Trim('.');

        return nameSpace is null or "" ? null : nameSpace;
    }

    public static TypeToGenerate GetTypeHierarchy(this TypeDeclarationSyntax syntax)
    {
        var typeToGenerate = new TypeToGenerate(
            Modifiers: syntax.Modifiers.GetModifiersArray(),
            Keyword: syntax.Keyword.Text,
            Name: syntax.Identifier.Text);

        while (syntax.Parent is TypeDeclarationSyntax parentSyntax)
        {
            syntax = parentSyntax;
            typeToGenerate = new TypeToGenerate(
                Modifiers: syntax.Modifiers.GetModifiersArray(),
                Keyword: syntax.Keyword.Text,
                Name: syntax.Identifier.Text,
                typeToGenerate);
        }

        return typeToGenerate;
    }

    public static PropertyToGenerate GetPropertyToGenerate(this PropertyDeclarationSyntax syntax, IPropertySymbol symbol)
    {
        var accessors = syntax.AccessorList?.Accessors
            .Select(x => new AccessorToGenerate(x.Modifiers.GetModifiersArray(), x.Keyword.Text))
            .ToDictionary(x => x.Keyword)
            ?? new();

        return new(
            Modifiers: syntax.Modifiers.GetModifiersArray(),
            Type: symbol.Type.ToFullyQualifiedDisplayString(),
            Name: syntax.Identifier.Text,
            Getter: accessors["get"],
            Setter: accessors["set"]);
    }

    private static ImmutableEquatableArray<string> GetModifiersArray(this SyntaxTokenList syntax) =>
        syntax.Select(x => x.Text).ToImmutableEquatableArray();
}