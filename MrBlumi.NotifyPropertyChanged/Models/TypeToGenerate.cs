using MrBlumi.NotifyPropertyChanged.Helpers;
namespace Test.Generator.NotifyPropertyChanged.Models;

public record TypeToGenerate(
    ImmutableEquatableArray<string> Modifiers,
    string Keyword,
    string Name,
    TypeToGenerate? Child = null);