using MrBlumi.NotifyPropertyChanged.Helpers;

namespace Test.Generator.NotifyPropertyChanged.Models;

public record FullTypeToGenerate(
    string? Namespace,
    TypeToGenerate TypeHierarchie,
    ImmutableEquatableArray<PropertyToGenerate> Properties);