using System.Diagnostics.CodeAnalysis;
using AdventOfCode.Maths.Vectors;
using JetBrains.Annotations;

namespace AdventOfCode.Collections;

/// <summary>
/// Grid position struct
/// </summary>
/// <param name="Position">Vector position in grid</param>
/// <param name="Element">Element value at position</param>
/// <typeparam name="T">Element type</typeparam>
[PublicAPI]
public readonly record struct GridPosition<T>(Vector2<int> Position, T Element);

/// <summary>
/// Grid base interface
/// </summary>
/// <typeparam name="T">Grid element type</typeparam>
[PublicAPI]
public interface IGrid<T> : IEnumerable<T>
{
    /// <summary>
    /// Size of the grid
    /// </summary>
    int Size { get; }

    /// <summary>
    /// Accesses an element in the grid
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <returns>The element at the specified position</returns>
    T this[int x, int y] { get; set; }

    /// <summary>
    /// Accesses an element in the grid
    /// </summary>
    /// <param name="vector">Position vector in the grid</param>
    /// <returns>The element at the specified position</returns>
    /// ReSharper disable once VirtualMemberNeverOverridden.Global
    T this[Vector2<int> vector] { get; set; }

    /// <summary>
    /// Accesses an element in the grid
    /// </summary>
    /// <param name="tuple">Position tuple in the grid</param>
    /// <returns>The element at the specified position</returns>
    T this[(int x, int y) tuple] { get; set; }

    /// <summary>
    /// Copies the data from another Grid into this one
    /// </summary>
    /// <param name="other">Other grid to copy from</param>
    void CopyFrom(IGrid<T> other);

    /// <summary>
    /// Tries to get a value in the grid at the given position
    /// </summary>
    /// <param name="position">Position to get the value for</param>
    /// <param name="value">The value, if it was found</param>
    /// <returns><see langword="true"/> if the value was found, otherwise <see langword="false"/></returns>
    bool TryGetPosition(Vector2<int> position, [MaybeNullWhen(false)] out T value);

    /// <summary>
    /// Checks if a given position Vector2 is within the grid
    /// </summary>
    /// <param name="position">Position vector</param>
    /// <returns>True if the Vector2 is within the grid, false otherwise</returns>
    bool WithinGrid(Vector2<int> position);

    /// <summary>
    /// Gets the position of the given value in the grid, if it exists
    /// </summary>
    /// <param name="value">Value to find</param>
    /// <returns>The first position in the grid that the value is found at, or <c>(-1, -1)</c> if it wasn't</returns>
    Vector2<int> PositionOf(T value);

    /// <summary>
    /// Checks if the given value is in the grid or not
    /// </summary>
    /// <param name="value">Value to find</param>
    /// <returns><see langword="true"/> if the value was in the grid, otherwise <see langword="false"/></returns>
    bool Contains(T value);

    /// <summary>
    /// Clears this grid
    /// </summary>
    void Clear();

    /// <summary>
    /// Enumerates the contents of the grid as a position/element pair
    /// </summary>
    /// <returns>An enumerable which has all the positions and elements in the grid</returns>
    IEnumerable<GridPosition<T>> EnumeratePositions();
}
