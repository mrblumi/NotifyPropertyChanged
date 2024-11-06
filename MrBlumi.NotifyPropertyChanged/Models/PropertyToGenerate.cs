namespace Test.Generator.NotifyPropertyChanged.Models;

public record PropertyToGenerate(
    IEnumerable<string> Modifiers,
    string Type,
    string Name);