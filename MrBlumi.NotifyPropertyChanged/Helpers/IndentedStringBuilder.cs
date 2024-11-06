using System.Text;

namespace MrBlumi.NotifyPropertyChanged.Helpers;

public class IndentedStringBuilder
{
    private readonly StringBuilder builder = new();
    
    public int Indent { get; private set; }

    public void AppendLine(string? line = null) => builder.AppendLine(new string(' ', 4 * Indent) + line);

    public void OpenBlock()
    {
        AppendLine("{");
        Indent++;
    }

    public void CloseBlock()
    {
        Indent--;
        RemoveLine();
        builder.AppendLine();
        AppendLine("}");
    }

    public void RemoveLine()
    {
        while (char.IsWhiteSpace(builder[^1])) builder.Remove(builder.Length - 1, 1);
    }
    
    public static implicit operator string(IndentedStringBuilder self) => self.builder.ToString();
}