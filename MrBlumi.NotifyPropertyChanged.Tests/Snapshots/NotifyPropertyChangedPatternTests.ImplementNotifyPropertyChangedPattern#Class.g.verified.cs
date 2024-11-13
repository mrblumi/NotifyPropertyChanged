//HintName: Class.g.cs
#nullable enable

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Test;

public partial class Class
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
        get;
        set => SetProperty(ref field, value);
    }
}