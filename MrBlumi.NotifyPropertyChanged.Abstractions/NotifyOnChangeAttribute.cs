namespace MrBlumi.NotifyPropertyChanged.Abstractions;

[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class NotifyOnChangeAttribute : Attribute;