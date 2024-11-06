using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrBlumi.NotifyPropertyChanged.Abstractions;
using System.Collections.Immutable;
using MrBlumi.NotifyPropertyChanged.Helpers;
using Test.Generator.NotifyPropertyChanged.Models;

namespace Test.Generator.NotifyPropertyChanged;

[Generator]
public class SourceCodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var properties = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: typeof(NotifyOnChangeAttribute).FullName,
                predicate: NodeIsPropertyDeclarationSyntax,
                transform: (ctxt, token) => new { Node = (PropertyDeclarationSyntax)ctxt.TargetNode, Symbol = (IPropertySymbol)ctxt.TargetSymbol })
            .Collect()
            .SelectMany((x, _) => x.GroupBy(y => y.Symbol.ContainingType.ToDisplayString(FullyQualifiedFormat)))
            .Select((x, _) => x.ToImmutableArray());

        context.RegisterSourceOutput(properties, (ctxt, props) =>
        {
            var containingSyntax = (TypeDeclarationSyntax)props[0].Node.Parent!;
            var fullTypeToGenerate = new FullTypeToGenerate(
                Namespace: GetNamespace(containingSyntax),
                TypeHierarchie: GetTypeHierarchy(containingSyntax),
                Properties: props.Select(x => new PropertyToGenerate(
                    Modifiers: x.Node.Modifiers.Select(y => y.Text),
                    Type: x.Symbol.Type.ToDisplayString(FullyQualifiedFormat),
                    Name: x.Symbol.Name)));
            
            ctxt.AddSource(
                hintName: fullTypeToGenerate.GetFileName(), 
                source: new SourceCodeWriter(fullTypeToGenerate).Write());
        });
    }

    private static bool NodeIsPropertyDeclarationSyntax(SyntaxNode node, CancellationToken token) =>
        node is PropertyDeclarationSyntax { AttributeLists.Count: > 0 };

    private static readonly SymbolDisplayFormat FullyQualifiedFormat = new(
        globalNamespaceStyle: SymbolDisplayFormat.FullyQualifiedFormat.GlobalNamespaceStyle,
        typeQualificationStyle: SymbolDisplayFormat.FullyQualifiedFormat.TypeQualificationStyle,
        genericsOptions: SymbolDisplayFormat.FullyQualifiedFormat.GenericsOptions,
        miscellaneousOptions: SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions |
                              SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
    
    private static string? GetNamespace(BaseTypeDeclarationSyntax syntax)
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

    private static TypeToGenerate GetTypeHierarchy(TypeDeclarationSyntax syntax)
    {
        var typeToGenerate = new TypeToGenerate(
            Modifiers: syntax.Modifiers.Select(x => x.Text),
            Keyword: syntax.Keyword.ToString(),
            Name: syntax.Identifier.Text);

        while (syntax.Parent is TypeDeclarationSyntax parentSyntax)
        {
            syntax = parentSyntax;
            typeToGenerate = new TypeToGenerate(
                Modifiers: syntax.Modifiers.Select(x => x.Text),
                Keyword: syntax.Keyword.ToString(),
                Name: syntax.Identifier.Text,
                typeToGenerate);
        }

        return typeToGenerate;
    }
}