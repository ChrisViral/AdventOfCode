using System.Numerics;
using JetBrains.Annotations;

namespace AdventOfCode.Maths.Vectors;

/// <summary>
/// Per-component vector comparer, compares the Y then X values
/// </summary>
/// <typeparam name="T">Vector component type</typeparam>
[PublicAPI]
public sealed class ComponentComparer<T> : IComparer<Vector2<T>>
    where T : unmanaged, IBinaryNumber<T>, IMinMaxValue<T>
{
    /// <summary>
    /// Comparer instance
    /// </summary>
    public static ComponentComparer<T> Instance { get; } = new();

    /// <summary>
    /// Prevents external initialisation
    /// </summary>
    private ComponentComparer() { }

    /// <inheritdoc />
    public int Compare(Vector2<T> x, Vector2<T> y)
    {
        int comp = x.Y.CompareTo(y.Y);
        return comp is 0 ? x.X.CompareTo(y.X) : comp;
    }
}
