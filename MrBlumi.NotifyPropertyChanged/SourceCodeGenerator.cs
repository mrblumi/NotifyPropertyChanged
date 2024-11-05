using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrBlumi.NotifyPropertyChanged.Abstractions;
using System.Collections.Immutable;

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
            var containingSymbol = (INamedTypeSymbol)props[0].Symbol.ContainingSymbol;
            var containingSyntax = (TypeDeclarationSyntax)props[0].Node.Parent!;

            var source = $$"""
                #nullable enable

                using System.ComponentModel;
                using System.Runtime.CompilerServices;

                namespace {{containingSymbol!.ContainingNamespace}};

                {{string.Join(" ", containingSyntax!.Modifiers)}} {{containingSyntax.Keyword}} {{containingSyntax.Identifier.Text}}
                {
                    public event PropertyChangedEventHandler? PropertyChanged;

                    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
                    {
                        if (EqualityComparer<T>.Default.Equals(field, value)) return;
                        
                        field = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                    }

                {{string.Join("\r\n\r\n", props.Select(x => PropertyCode(x.Symbol, x.Node)))}}
                }
                """;

            ctxt.AddSource(containingSyntax.Identifier.Text + ".g.cs", source);
        });
    }

    private static string PropertyCode(IPropertySymbol symbol, PropertyDeclarationSyntax syntax) =>
        $$"""
            {{string.Join(" ", syntax.Modifiers)}} {{symbol.Type.ToDisplayString(FullyQualifiedFormat)}} {{symbol.Name}}
            {
                get { return field; }
                set { SetProperty(ref field, value); }
            }
        """;

    private static bool NodeIsPropertyDeclarationSyntax(SyntaxNode node, CancellationToken token) =>
        node is PropertyDeclarationSyntax { AttributeLists.Count: > 0 };

    private static readonly SymbolDisplayFormat FullyQualifiedFormat = new(
        globalNamespaceStyle: SymbolDisplayFormat.FullyQualifiedFormat.GlobalNamespaceStyle,
        typeQualificationStyle: SymbolDisplayFormat.FullyQualifiedFormat.TypeQualificationStyle,
        genericsOptions: SymbolDisplayFormat.FullyQualifiedFormat.GenericsOptions,
        miscellaneousOptions: SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions |
                              SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
}