using MrBlumi.NotifyPropertyChanged.Helpers;

namespace MrBlumi.NotifyPropertyChanged.Models;

public record AccessorToGenerate(
    ImmutableEquatableArray<string> Modifiers,
    string Keyword);