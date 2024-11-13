using MrBlumi.NotifyPropertyChanged.Extensions;
using MrBlumi.NotifyPropertyChanged.Models;

namespace MrBlumi.NotifyPropertyChanged.Helpers;

public class SourceCodeWriter(FullTypeToGenerate fullType)
{
    public string Write()
    {
        var builder = new IndentedStringBuilder();

        builder.AppendLine($"#nullable enable");
        builder.AppendLine();
        builder.AppendLine($"using System.ComponentModel;");
        builder.AppendLine($"using System.Runtime.CompilerServices;");
        builder.AppendLine();

        if (fullType.Namespace is not null)
        {
            builder.AppendLine($"namespace {fullType.Namespace};");
            builder.AppendLine();            
        }

        foreach (var type in fullType.TypeHierarchie.AsEnumerable())
        {
            builder.AppendLine($"{string.Join(" ", type.Modifiers)} {type.Keyword} {type.Name}");
            builder.OpenBlock();
        }
        
        builder.AppendLine($"public event PropertyChangedEventHandler? PropertyChanged;");
        builder.AppendLine();
        builder.AppendLine($"private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)");
        builder.OpenBlock();
        builder.AppendLine($"if (EqualityComparer<T>.Default.Equals(field, value)) return;");
        builder.AppendLine();
        builder.AppendLine($"field = value;");
        builder.AppendLine($"PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));");
        builder.CloseBlock();
        builder.AppendLine();

        foreach (var property in fullType.Properties)
        {
            builder.AppendLine($"{string.Join(" ", property.Modifiers)} {property.Type} {property.Name}");
            builder.OpenBlock();
            builder.AppendLine($"{string.Join(" ", [..property.Getter.Modifiers, property.Getter.Keyword])};");
            builder.AppendLine($"{string.Join(" ", [..property.Setter.Modifiers, property.Setter.Keyword])} => SetProperty(ref field, value);");
            builder.CloseBlock();
            builder.AppendLine();
        }

        while (builder.Indent > 0) builder.CloseBlock();
        builder.RemoveLine();
        
        return builder;
    }
}