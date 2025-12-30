using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using AdventOfCode.Collections.DebugViews;
using AdventOfCode.Maths.Vectors;
using JetBrains.Annotations;
using ZLinq;
using ZLinq.Linq;

[assembly: ZLinqDropInExternalExtension("AdventOfCode.Collections", "AdventOfCode.Collections.SparseGrid`1", "ZLinq.Linq.FromEnumerable`1", GenerateAsPublic = true)]

namespace AdventOfCode.Collections;

/// <summary>
/// Dictionary backed sparse grid
/// </summary>
/// <typeparam name="T">Grid element</typeparam>
[PublicAPI, DebuggerDisplay("Size = {Size}"), DebuggerTypeProxy(typeof(SparseGridDebugView<>))]
public sealed class SparseGrid<T> : IGrid<T>
{
    private static readonly EqualityComparer<T> Comparer = EqualityComparer<T>.Default;

    private readonly DefaultDictionary<Vector2<int>, T> grid;

    /// <summary>
    /// Size of the grid
    /// </summary>
    public int Size => this.grid.Count;

    /// <summary>
    /// Accesses an element in the grid
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <returns>The element at the specified position</returns>
    public T this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.grid[new Vector2<int>(x, y)];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.grid[new Vector2<int>(x, y)] = value;
    }

    /// <summary>
    /// Accesses an element in the grid
    /// </summary>
    /// <param name="vector">Position vector in the grid</param>
    /// <returns>The element at the specified position</returns>
    /// ReSharper disable once VirtualMemberNeverOverridden.Global
    public T this[Vector2<int> vector]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.grid[vector];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.grid[vector] = value;
    }

    /// <summary>
    /// Accesses an element in the grid
    /// </summary>
    /// <param name="tuple">Position tuple in the grid</param>
    /// <returns>The element at the specified position</returns>
    public T this[(int x, int y) tuple]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.grid[new Vector2<int>(tuple)];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.grid[new Vector2<int>(tuple)] = value;
    }

    /// <summary>
    /// Creates a new sparse grid
    /// </summary>
    /// <param name="defaultValue">Default value provided by the grid</param>
    public SparseGrid(T defaultValue) => this.grid = new DefaultDictionary<Vector2<int>, T>(defaultValue);

    /// <summary>
    /// Creates a new sparse grid with the specified capacity
    /// </summary>
    /// <param name="capacity">Grid initial capacity</param>
    /// <param name="defaultValue">Default value provided by the grid</param>
    public SparseGrid(int capacity, T defaultValue) => this.grid = new DefaultDictionary<Vector2<int>, T>(capacity, defaultValue);

    /// <summary>
    /// Grid copy constructor
    /// </summary>
    /// <param name="other">Other grid to create a copy of</param>
    public SparseGrid(SparseGrid<T> other) => this.grid = new DefaultDictionary<Vector2<int>, T>(other.grid);

    /// <inheritdoc />
    public void CopyFrom(IGrid<T> other)
    {
        foreach ((Vector2<int> position, T element) in other.EnumeratePositions())
        {
            this[position] = element;
        }
    }

    /// <summary>
    /// Tries to get a value in the grid at the given position
    /// </summary>
    /// <param name="position">Position to get the value for</param>
    /// <param name="value">The value, if it was found</param>
    /// <returns><see langword="true"/> if the value was found, otherwise <see langword="false"/></returns>
    public bool TryGetPosition(Vector2<int> position, [MaybeNullWhen(false)] out T value)
    {
        return this.grid.TryGetValue(position, out value);
    }

    /// <summary>
    /// Checks if a given position Vector2 is within the grid
    /// </summary>
    /// <param name="position">Position vector</param>
    /// <returns>True if the Vector2 is within the grid, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool WithinGrid(Vector2<int> position)
    {
        return this.grid.ContainsKey(position);
    }

    /// <summary>
    /// Gets the position of the given value in the grid, if it exists
    /// </summary>
    /// <param name="value">Value to find</param>
    /// <returns>The first position in the grid that the value is found at, or <c>(-1, -1)</c> if it wasn't</returns>
    public Vector2<int> PositionOf(T value)
    {
        foreach (KeyValuePair<Vector2<int>, T> pair in this.grid)
        {
            if (Comparer.Equals(pair.Value, value))
            {
                return pair.Key;
            }
        }

        throw new InvalidOperationException($"Value {value} could not be found");
    }

    /// <summary>
    /// Checks if the given value is in the grid or not
    /// </summary>
    /// <param name="value">Value to find</param>
    /// <returns><see langword="true"/> if the value was in the grid, otherwise <see langword="false"/></returns>
    public bool Contains(T value) => this.grid.ContainsValue(value);

    /// <summary>
    /// Clears this grid
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => this.grid.Clear();

    /// <summary>
    /// Copies the values of the grid to an array
    /// </summary>
    /// <param name="array">Array to copy to</param>
    /// <param name="arrayIndex">Target array starting index to copy to</param>
    public void CopyTo(KeyValuePair<Vector2<int>, T>[] array, int arrayIndex)
    {
        IDictionary<Vector2<int>, T> dictionary = this.grid;
        dictionary.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<GridPosition<T>> EnumeratePositions() => this.grid.Select(p => new GridPosition<T>(p.Key, p.Value));

    /// <summary>
    /// Converts this SparseGrid to a value enumerable
    /// </summary>
    /// <returns>ValueEnumerable over the grid</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueEnumerable<FromEnumerable<T>, T> AsValueEnumerable() => this.grid.Values.AsValueEnumerable();

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Dictionary<Vector2<int>, T>.ValueCollection.Enumerator GetEnumerator() => this.grid.Values.GetEnumerator();

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
