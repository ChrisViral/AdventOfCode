using System.Numerics;
using System.Runtime.CompilerServices;
using AdventOfCode.Utils.Extensions.Numbers;
using AdventOfCode.Maths.Vectors;
using CommunityToolkit.HighPerformance;
using JetBrains.Annotations;

namespace AdventOfCode.Maths;

/// <summary>
/// Mathematics utils
/// </summary>
[PublicAPI]
public static class MathUtils
{
    /// <summary>
    /// Greatest Common Divisor function
    /// </summary>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>Gets the GCD of a and b</returns>
    /// ReSharper disable once MemberCanBePrivate.Global
    public static T GCD<T>(T a, T b) where T : IBinaryInteger<T>
    {
        a = T.Abs(a);
        b = T.Abs(b);
        while (a != T.Zero && b != T.Zero)
        {
            if (a > b)
            {
                a %= b;
            }
            else
            {
                b %= a;
            }
        }

        return a | b;
    }

    /// <summary>
    /// Least Common Multiple function
    /// </summary>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>The LCM of a and b</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T LCM<T>(T a, T b) where T : IBinaryInteger<T> => a * b / GCD(a, b);

    /// <summary>
    /// Modular power function
    /// </summary>
    /// <param name="a">Base to exponentiate</param>
    /// <param name="b">Exponant</param>
    /// <param name="mod">Modulus</param>
    /// <typeparam name="T">Integer type</typeparam>
    /// <returns>The modular power <paramref name="a"/>^<paramref name="b"/> % <paramref name="mod"/></returns>
    public static T ModularPower<T>(T a, T b, T mod) where T : IBinaryInteger<T>
    {
        if (mod == T.One) return T.Zero;

        a %= mod;
        T result = T.One;
        while (b > T.Zero)
        {
            if (b.IsOdd)
            {
                result = (result * a) % mod;
            }
            b >>= 1;
            a = (a * a) % mod;
        }
        return result;
    }

    /// <summary>
    /// Modular inverse function
    /// </summary>
    /// <param name="n">Value to inverse</param>
    /// <param name="mod">Modulus</param>
    /// <typeparam name="T">Integer type</typeparam>
    /// <returns>The modular inverse <paramref name="n"/>^(-1) % <paramref name="mod"/></returns>
    public static T ModularInverse<T>(T n, T mod) where T : IBinaryInteger<T> => ModularPower(n, mod - NumberUtils<T>.Two, mod);

    /// <summary>
    /// 2x2 Matrix modular power
    /// </summary>
    /// <param name="matrix">Matrix to raise to a given power</param>
    /// <param name="result">Output result matrix</param>
    /// <param name="power">Power to raise the matrix at</param>
    /// <param name="mod">Modulus</param>
    /// <typeparam name="T">Integer type</typeparam>
    /// ReSharper disable once InconsistentNaming
    public static void Matrix2x2Power<T>(ReadOnlySpan2D<T> matrix, ref Span2D<T> result, T power, T mod) where T : unmanaged, IBinaryInteger<T>
    {
        Span2D<T> a = stackalloc T[4].AsSpan2D(2, 2);
        matrix.CopyTo(a);

        result[0, 0] = T.One;
        result[1, 1] = T.One;

        Span2D<T> mulResult = stackalloc T[4].AsSpan2D(2, 2);
        while (power > T.Zero)
        {
            if (power.IsOdd)
            {
                MatrixMultiplication(result, a, ref mulResult, mod);
                mulResult.CopyTo(result);
            }

            MatrixMultiplication(a, a, ref mulResult, mod);
            mulResult.CopyTo(a);
            power >>= 1;
        }
    }

    /// <summary>
    /// Matrix multiplication
    /// </summary>
    /// <param name="a">First matrix</param>
    /// <param name="b">Second matrix</param>
    /// <param name="result">Output result matrix</param>
    /// <param name="mod">Modulus</param>
    /// <typeparam name="T">Integer type</typeparam>
    public static void MatrixMultiplication<T>(ReadOnlySpan2D<T> a, ReadOnlySpan2D<T> b, ref Span2D<T> result, T mod) where T : unmanaged, IBinaryInteger<T>
    {
        int n = a.Height;
        int m = b.Width;
        int l = a.Width;
        foreach ((int i, int j) in Vector2<int>.EnumerateOver(n, m))
        {
            T cell = T.Zero;
            for (int k = 0; k < l; k++)
            {
                cell += (a[i, k] * b[k, j]) % mod;
                cell %= mod;
            }
            result[i, j] = cell;
        }
    }

    /// <summary>
    /// Calculates the area of a polygon from its vertices using the Shoelace formula
    /// </summary>
    /// <typeparam name="T">Type of integer making up the vertices</typeparam>
    /// <param name="vertices">List of vertices</param>
    /// <returns>The total area of the polygon</returns>
    /// <exception cref="ArgumentOutOfRangeException">If there are not enough vertices to make a proper 2D polygon</exception>
    public static T Shoelace<T>(IList<Vector2<T>> vertices) where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        // Make sure we have enough vertices
        if (vertices.Count <= 0) throw new ArgumentOutOfRangeException(nameof(vertices), vertices.Count, "A 2D polygon requires a minimum of 3 vertices");

        // Add the first vertex to the end if not already there
        if (vertices[0] != vertices[^1])
        {
            vertices.Add(vertices[0]);
        }

        T area = T.Zero;
        Vector2<T> current = vertices[0];
        for (int i = 1; i < vertices.Count; i++)
        {
            Vector2<T> next = vertices[i];
            area    += (current.X * next.Y) - (next.X * current.Y);
            current =  next;
        }

        return area / NumberUtils<T>.Two;
    }

    /// <summary>
    /// Calculates the area of a polygon using Pick's Theorem
    /// </summary>
    /// <typeparam name="T">Type of integer to calculate for</typeparam>
    /// <param name="interior">Number of interior points</param>
    /// <param name="border">Number of exterior points</param>
    /// <returns>The total area of the polygon</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Picks<T>(T interior, T border) where T : IBinaryInteger<T> => interior + (border / NumberUtils<T>.Two) + T.One;

    /// <summary>
    /// Checks whether a given point is inside or outside a polygon defined by the specified edges
    /// </summary>
    /// <param name="position">The position of the point to check</param>
    /// <param name="vertices">The list of the polygon's vertices</param>
    /// <param name="edgesInside">If the point is considered inside the polygon when it lies on an edge</param>
    /// <returns><see langword="true"/> when the <paramref name="position"/> is within the polygon, otherwise <see langword="false"/></returns>
    /// ReSharper disable once CognitiveComplexity
    public static bool IsInsidePolygon<T>(Vector2<T> position, IReadOnlyList<Vector2<T>> vertices, bool edgesInside = false) where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        bool IsInsidePolygonNoEdges()
        {
            bool isInside = false;
            Vector2<T> previous = vertices[^1];
            foreach (Vector2<T> current in vertices)
            {
                // Edge crossings
                if ((current.Y > position.Y) != (previous.Y > position.Y))
                {
                    T xIntersection = current.X + T.CreateChecked((long.CreateChecked(previous.X - current.X) * long.CreateChecked(position.Y - current.Y)) / long.CreateChecked(previous.Y - current.Y));
                    if (position.X < xIntersection)
                    {
                        isInside = !isInside;
                    }
                }
                previous = current;
            }

            return isInside;
        }

        bool IsInsidePolygonWithEdges()
        {
            bool isInside = false;
            Vector2<T> previous = vertices[^1];
            foreach (Vector2<T> current in vertices)
            {
                // Check if colinear to an edge
                long cross = Vector2<long>.Cross(Vector2<long>.CreateChecked(current - previous), Vector2<long>.CreateChecked(position - previous));
                if (cross is 0)
                {
                    // Check if within bounds of an edge
                    Vector2<T> min = Vector2<T>.Min(previous, current);
                    Vector2<T> max = Vector2<T>.Max(previous, current);
                    if (position.X >= min.X && position.X <= max.X && position.Y >= min.Y && position.Y <= max.Y)
                    {
                        return true;
                    }
                }

                // Edge crossings
                if ((current.Y > position.Y) != (previous.Y > position.Y))
                {
                    T xIntersection = current.X + T.CreateChecked((long.CreateChecked(previous.X - current.X) * long.CreateChecked(position.Y - current.Y)) / long.CreateChecked(previous.Y - current.Y));
                    if (position.X < xIntersection)
                    {
                        isInside = !isInside;
                    }
                }
                previous = current;
            }

            return isInside;
        }

        return edgesInside ? IsInsidePolygonWithEdges() : IsInsidePolygonNoEdges();
    }

    /// <summary>
    /// Checks whether a given point is inside or outside an axis-aligned polygon defined by the specified edges
    /// </summary>
    /// <param name="position">The position of the point to check</param>
    /// <param name="vertices">The list of the polygon's vertices</param>
    /// <param name="edgesInside">If the point is considered inside the polygon when it lies on an edge</param>
    /// <returns><see langword="true"/> when the <paramref name="position"/> is within the polygon, otherwise <see langword="false"/></returns>
    /// ReSharper disable once CognitiveComplexity
    public static bool IsInsideAxisAlignedPolygon<T>(Vector2<T> position, IReadOnlyList<Vector2<T>> vertices, bool edgesInside = false) where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        bool IsInsidePolygonNoEdges()
        {
            bool isInside = false;
            Vector2<T> previous = vertices[^1];
            foreach (Vector2<T> current in vertices)
            {
                // Edge crossings
                if (current.X == previous.X
                 && (current.Y > position.Y) != (previous.Y > position.Y)
                 && position.X < current.X)
                {
                    isInside = !isInside;
                }
                previous = current;
            }

            return isInside;
        }

        bool IsInsidePolygonWithEdges()
        {
            bool isInside = false;
            Vector2<T> previous = vertices[^1];
            foreach (Vector2<T> current in vertices)
            {
                // Check if on an edge
                Vector2<T> min = Vector2<T>.Min(previous, current);
                Vector2<T> max = Vector2<T>.Max(previous, current);
                if ((previous.Y == current.Y && position.Y == previous.Y && position.X >= min.X && position.X <= max.X)
                 || (previous.X == current.X && position.X == previous.X && position.Y >= min.Y && position.Y <= max.Y)) return true;

                // Edge crossings
                if (current.X == previous.X
                 && (current.Y > position.Y) != (previous.Y > position.Y)
                 && position.X < current.X)
                {
                    isInside = !isInside;
                }
                previous = current;
            }

            return isInside;
        }

        return edgesInside ? IsInsidePolygonWithEdges() : IsInsidePolygonNoEdges();
    }
}
