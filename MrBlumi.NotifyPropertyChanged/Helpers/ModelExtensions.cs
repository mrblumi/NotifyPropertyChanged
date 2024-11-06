using Test.Generator.NotifyPropertyChanged.Models;

namespace MrBlumi.NotifyPropertyChanged.Helpers;

public static class ModelExtensions
{
    public static string GetFileName(this FullTypeToGenerate type)
    {
        var hierarchy = type.TypeHierarchie
            .AsEnumerable()
            .Select(x => x.Name);

        return string.Join(".", hierarchy) + ".g.cs";
    }

    private static IEnumerable<TypeToGenerate> AsEnumerable(this TypeToGenerate? current)
    {
        while (current is not null)
        {
            yield return current;
            current = current.Child;
        }
    }
}