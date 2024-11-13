using MrBlumi.NotifyPropertyChanged.Helpers;
using MrBlumi.NotifyPropertyChanged.Models;

namespace Test.Generator.NotifyPropertyChanged.Models;

public record PropertyToGenerate(
    ImmutableEquatableArray<string> Modifiers,
    string Type,
    string Name,
    AccessorToGenerate Getter,
    AccessorToGenerate Setter);