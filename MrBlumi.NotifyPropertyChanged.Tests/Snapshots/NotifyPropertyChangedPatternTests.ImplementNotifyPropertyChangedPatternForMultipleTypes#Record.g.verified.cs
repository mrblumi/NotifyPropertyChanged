//HintName: Record.g.cs
#nullable enable

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Test;

public partial record Record
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public partial string StringProperty
    {
        get => field;
        set => SetProperty(ref field, value);
    }
}