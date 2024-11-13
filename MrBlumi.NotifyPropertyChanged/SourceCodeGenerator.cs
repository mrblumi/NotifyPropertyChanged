using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrBlumi.NotifyPropertyChanged.Abstractions;
using System.Collections.Immutable;
using MrBlumi.NotifyPropertyChanged.Helpers;
using MrBlumi.NotifyPropertyChanged.Extensions;
using MrBlumi.NotifyPropertyChanged.Models;

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
            .SelectMany((x, _) => x.GroupBy(y => y.Symbol.ContainingType.ToFullyQualifiedDisplayString()))
            .Select((x, _) => x.ToImmutableArray());

        context.RegisterSourceOutput(properties, (ctxt, props) =>
        {
            var containingSyntax = (TypeDeclarationSyntax)props[0].Node.Parent!;
            var fullTypeToGenerate = new FullTypeToGenerate(
                Namespace: containingSyntax.GetNamespace(),
                TypeHierarchie: containingSyntax.GetTypeHierarchy(),
                Properties: props
                    .Select(x => x.Node.GetPropertyToGenerate(x.Symbol))
                    .ToImmutableEquatableArray());

            ctxt.AddSource(
                hintName: fullTypeToGenerate.GetFileName(),
                source: new SourceCodeWriter(fullTypeToGenerate).Write());
        });
    }

    private static bool NodeIsPropertyDeclarationSyntax(SyntaxNode node, CancellationToken token) =>
        node is PropertyDeclarationSyntax { AttributeLists.Count: > 0 };
}