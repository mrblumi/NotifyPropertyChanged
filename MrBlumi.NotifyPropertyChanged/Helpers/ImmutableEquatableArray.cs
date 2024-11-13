using System.Collections;

namespace MrBlumi.NotifyPropertyChanged.Helpers;

public sealed class ImmutableEquatableArray<T> : IEquatable<ImmutableEquatableArray<T>>, IReadOnlyList<T> where T : IEquatable<T>
{
    private readonly T[] _values;
    public ImmutableEquatableArray(IEnumerable<T> values) => _values = values.ToArray();

    public T this[int index] => _values[index];
    public int Count => _values.Length;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)_values).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

    public override int GetHashCode() => _values.Aggregate(0, (current, item) => current ^ item.GetHashCode());
    public override bool Equals(object obj) => obj is ImmutableEquatableArray<T> other && Equals(other);

    public bool Equals(ImmutableEquatableArray<T> other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (Count != other.Count) return false;
        return _values.SequenceEqual(other._values);
    }
}

public static class ImmutableEquatableArray
{
    public static ImmutableEquatableArray<T> ToImmutableEquatableArray<T>(this IEnumerable<T> values) where T : IEquatable<T>
        => new ImmutableEquatableArray<T>(values);
}
