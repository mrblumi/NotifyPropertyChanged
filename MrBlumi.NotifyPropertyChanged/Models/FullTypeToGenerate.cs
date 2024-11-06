namespace Test.Generator.NotifyPropertyChanged.Models;

public record FullTypeToGenerate(
    string? Namespace,
    TypeToGenerate TypeHierarchie,
    IEnumerable<PropertyToGenerate> Properties);