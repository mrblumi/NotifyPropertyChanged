using MrBlumi.NotifyPropertyChanged.Helpers;

namespace MrBlumi.NotifyPropertyChanged.Models;

public record FullTypeToGenerate(
    string? Namespace,
    TypeToGenerate TypeHierarchie,
    ImmutableEquatableArray<PropertyToGenerate> Properties);