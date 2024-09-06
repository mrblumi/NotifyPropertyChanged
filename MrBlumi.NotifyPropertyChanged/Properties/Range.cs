using System.Runtime.CompilerServices;

namespace System;

public readonly struct Range(Index start, Index end) : IEquatable<Range>
{
    public Index Start { get; } = start;
    public Index End { get; } = end;

    public override bool Equals(object? value) => value is Range r && Equals(r);
    public bool Equals(Range other) => other.Start.Equals(Start) && other.End.Equals(End);

    public override int GetHashCode() => Start.GetHashCode() * 31 + End.GetHashCode();

    public override string ToString() => Start + ".." + End;

    public static Range StartAt(Index start) => new Range(start, Index.End);
    public static Range EndAt(Index end) => new Range(Index.Start, end);
    public static Range All => new Range(Index.Start, Index.End);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int Offset, int Length) GetOffsetAndLength(int length)
    {
        int start = Start.GetOffset(length);
        int end = End.GetOffset(length);

        if ((uint)end > (uint)length || (uint)start > (uint)end)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        return (start, end - start);
    }
}