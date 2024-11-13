using Microsoft.CodeAnalysis;

namespace MrBlumi.NotifyPropertyChanged.Helpers;

public static class SymbolExtensions
{
    public static string ToFullyQualifiedDisplayString(this ITypeSymbol symbol) =>
        symbol.ToDisplayString(FullyQualifiedFormat);

    public static SymbolDisplayFormat FullyQualifiedFormat { get; } = new(
        globalNamespaceStyle: SymbolDisplayFormat.FullyQualifiedFormat.GlobalNamespaceStyle,
        typeQualificationStyle: SymbolDisplayFormat.FullyQualifiedFormat.TypeQualificationStyle,
        genericsOptions: SymbolDisplayFormat.FullyQualifiedFormat.GenericsOptions,
        miscellaneousOptions: SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions |
                              SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
}
