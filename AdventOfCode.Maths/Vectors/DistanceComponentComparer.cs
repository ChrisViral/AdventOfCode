using System.Numerics;
using JetBrains.Annotations;

namespace AdventOfCode.Maths.Vectors;

/// <summary>
/// Distance per-component vector comparer, filters the closest vector to the provided point, and separates ties through per-component checks
/// </summary>
/// <param name="from">Vector to take the distance from</param>
/// <typeparam name="T">Vector component type</typeparam>
[PublicAPI]
public sealed class DistanceComponentComparer<T>(Vector2<T> from) : IComparer<Vector2<T>>
    where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
{
    private readonly Vector2<T> from = from;

    /// <inheritdoc />
    public int Compare(Vector2<T> x, Vector2<T> y)
    {
        T xDistance = Vector2<T>.ManhattanDistance(this.from, x);
        T yDistance = Vector2<T>.ManhattanDistance(this.from, y);

        // Check distance first
        int comp = xDistance.CompareTo(yDistance);
        if (comp is not 0) return comp;

        // Compare components after
        comp = x.Y.CompareTo(y.Y);
        return comp is 0 ? x.X.CompareTo(y.X) : comp;
    }
}
