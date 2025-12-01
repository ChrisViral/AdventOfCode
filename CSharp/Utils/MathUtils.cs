using System;
using System.Collections.Generic;
using System.Numerics;
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
    /// Calculates the area of a polygon from its vertices using the Shoelace formula
    /// </summary>
    /// <typeparam name="T">Type of integer making up the vertices</typeparam>
    /// <param name="vertices">List of vertices</param>
    /// <returns>The total area of the polygon</returns>
    /// <exception cref="ArgumentOutOfRangeException">If there are not enough vertices to make a proper 2D polygon</exception>
    public static T Shoelace<T>(List<Vector2<T>> vertices) where T : IBinaryInteger<T>, IMinMaxValue<T>
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
    public static T Picks<T>(T interior, T border) where T : IBinaryInteger<T> => interior + (border / NumberUtils<T>.Two) + T.One;
}
