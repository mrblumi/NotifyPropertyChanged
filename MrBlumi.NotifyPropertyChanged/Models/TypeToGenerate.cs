using System.Collections;

namespace Test.Generator.NotifyPropertyChanged.Models;

public record TypeToGenerate(
    IEnumerable<string> Modifiers,
    string Keyword,
    string Name,
    TypeToGenerate? Child = null)
    : IEnumerable<TypeToGenerate>
{
    public IEnumerator<TypeToGenerate> GetEnumerator()
    {
        var current = this;
        while (current is not null)
        {
            yield return current;
            current = current.Child;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}