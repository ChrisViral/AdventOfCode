using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance.Enumerables;
using ZLinq;

namespace AdventOfCode.Utils.ValueEnumerators;

/// <summary>
/// RefEnumerable Value Enumerator
/// </summary>
/// <param name="enumerable">Enumerable to create the enumerator for</param>
/// <typeparam name="T">Enumerable element type</typeparam>
public ref struct FromRefEnumerable<T>(RefEnumerable<T> enumerable) : IValueEnumerator<T>
{
    private readonly RefEnumerable<T> enumerable = enumerable;
    private RefEnumerable<T>.Enumerator enumerator = enumerable.GetEnumerator();

    /// <inheritdoc />
    public bool TryGetNext(out T current)
    {
        if (this.enumerator.MoveNext())
        {
            current = this.enumerator.Current;
            return true;
        }

        current = default!;
        return false;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetNonEnumeratedCount(out int count)
    {
        count = this.enumerable.Length;
        return true;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetSpan(out ReadOnlySpan<T> span)
    {
        span = default;
        return false;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(scoped Span<T> destination, Index offset)
    {
        int index = offset.GetOffset(this.enumerable.Length);
        return index is 0 && this.enumerable.TryCopyTo(destination);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() { }
}
