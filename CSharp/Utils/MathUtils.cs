using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using AdventOfCode.Vectors;
using JetBrains.Annotations;

namespace AdventOfCode.Utils;

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
    /// Calculates the area of a polygon from its vertices using the Shoelace formula
    /// </summary>
    /// <typeparam name="T">Type of integer making up the vertices</typeparam>
    /// <param name="vertices">List of vertices</param>
    /// <returns>The total area of the polygon</returns>
    /// <exception cref="ArgumentOutOfRangeException">If there are not enough vertices to make a proper 2D polygon</exception>
    public static T Shoelace<T>(IList<Vector2<T>> vertices) where T : IBinaryInteger<T>, IMinMaxValue<T>
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
    public static bool IsInsidePolygon<T>(Vector2<T> position, IReadOnlyList<Vector2<T>> vertices, bool edgesInside = false) where T : IBinaryInteger<T>, IMinMaxValue<T>
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
    public static bool IsInsideAxisAlignedPolygon<T>(Vector2<T> position, IReadOnlyList<Vector2<T>> vertices, bool edgesInside = false) where T : IBinaryInteger<T>, IMinMaxValue<T>
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
