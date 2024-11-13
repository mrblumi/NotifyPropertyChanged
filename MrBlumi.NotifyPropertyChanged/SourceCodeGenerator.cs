using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrBlumi.NotifyPropertyChanged.Abstractions;
using System.Collections.Immutable;
using MrBlumi.NotifyPropertyChanged.Helpers;
using Test.Generator.NotifyPropertyChanged.Models;
using MrBlumi.NotifyPropertyChanged.Models;
using MrBlumi.NotifyPropertyChanged.Extensions;

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
                Namespace: containingSyntax.GetNamespace(),
                TypeHierarchie: containingSyntax.GetTypeHierarchy(),
                Properties: props
                    .Select(x => PropertyToGenerate(x.Node, x.Symbol))
                    .ToImmutableEquatableArray());
            
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
    

    private static PropertyToGenerate PropertyToGenerate(PropertyDeclarationSyntax syntax, IPropertySymbol symbol)
    {
        var accessors = syntax.AccessorList?.Accessors
            .Select(x => new AccessorToGenerate(x.Modifiers.Select(y => y.Text).ToImmutableEquatableArray(), x.Keyword.Text))
            .ToDictionary(x => x.Keyword)
            ?? new();

        return new(
            Modifiers: syntax.Modifiers.Select(x => x.Text).ToImmutableEquatableArray(),
            Type: symbol.Type.ToDisplayString(FullyQualifiedFormat),
            Name: syntax.Identifier.Text,
            Getter: accessors["get"],
            Setter: accessors["set"]);
    }
}