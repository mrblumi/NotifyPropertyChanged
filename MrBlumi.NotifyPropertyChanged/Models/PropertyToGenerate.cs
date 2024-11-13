using MrBlumi.NotifyPropertyChanged.Helpers;

namespace MrBlumi.NotifyPropertyChanged.Models;

public record PropertyToGenerate(
    ImmutableEquatableArray<string> Modifiers,
    string Type,
    string Name,
    AccessorToGenerate Getter,
    AccessorToGenerate Setter);