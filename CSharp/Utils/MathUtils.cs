using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Compares two floating point numbers to see if they are nearly identical
    /// </summary>
    /// <typeparam name="T">Floating point number type</typeparam>
    /// <param name="a">First number to test</param>
    /// <param name="b">Second number to test</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> and <paramref name="b"/> are approximately equal, otherwise <see langword="false"/></returns>
    public static bool Approximately<T>(T a, T b) where T : IFloatingPoint<T> => T.Abs(a - b) <= FloatUtils<T>.Epsilon;

    /// <summary>
    /// Gets the ceiling of a floating point value as an integer number
    /// </summary>
    /// <typeparam name="TSelf">Floating point source type</typeparam>
    /// <typeparam name="TResult">Integer target type</typeparam>
    /// <param name="value">The value to get the ceiling for</param>
    /// <returns>The ceiling of <paramref name="value"/> as an integer</returns>
    public static TResult CeilToInt<TSelf, TResult>(TSelf value) where TSelf   : IFloatingPoint<TSelf>
                                                                 where TResult : IBinaryInteger<TResult>
    {
        return TResult.CreateChecked(TSelf.Ceiling(value));
    }

    /// <summary>
    /// Gets the floor of a floating point value as an integer number
    /// </summary>
    /// <typeparam name="TSelf">Floating point source type</typeparam>
    /// <typeparam name="TResult">Integer target type</typeparam>
    /// <param name="value">The value to get the floor for</param>
    /// <returns>The floor of <paramref name="value"/> as an integer</returns>
    public static TResult FloorToInt<TSelf, TResult>(TSelf value) where TSelf   : IFloatingPoint<TSelf>
                                                                  where TResult : IBinaryInteger<TResult>
    {
        return TResult.CreateChecked(TSelf.Floor(value));
    }

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
    /// Greatest Common Divisor of all passed numbers
    /// </summary>
    /// <param name="numbers">Numbers to get the GCD for</param>
    /// <returns>Gets the GCD of all the passed numbers</returns>
    public static T GCD<T>(params T[] numbers) where T : IBinaryInteger<T> => numbers.Aggregate(GCD);

    /// <summary>
    /// Least Common Multiple function
    /// </summary>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>The LCM of a and b</returns>
    public static T LCM<T>(T a, T b) where T : IBinaryInteger<T> => a * b / GCD(a, b);

    /// <summary>
    /// Least Common Multiple function
    /// </summary>
    /// <param name="numbers">Numbers to get the LCM for</param>
    /// <returns>LCM of all the numbers in the array</returns>
    public static T LCM<T>(params T[] numbers) where T : IBinaryInteger<T> => numbers.Aggregate(LCM);

    /// <summary>
    /// Gets the maximum value between <paramref name="a"/> and <paramref name="b"/>
    /// </summary>
    /// <typeparam name="T">Type of number</typeparam>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>The maximum from both numbers</returns>
    /// ReSharper disable once MemberCanBePrivate.Global
    public static T Max<T>(T a, T b) where T : INumber<T> => T.Max(a, b);

    /// <summary>
    /// Gets the maximum of all numbers passed
    /// </summary>
    /// <typeparam name="T">Type of numbers</typeparam>
    /// <param name="numbers">List of numbers to get the maximum of</param>
    /// <returns>The maximum of all the passed numbers</returns>
    public static T Max<T>(params T[] numbers) where T : INumber<T> => numbers.Aggregate(Max);

    /// <summary>
    /// Gets the minimum value between <paramref name="a"/> and <paramref name="b"/>
    /// </summary>
    /// <typeparam name="T">Type of number</typeparam>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>The minimum from both numbers</returns>
    /// ReSharper disable once MemberCanBePrivate.Global
    public static T Min<T>(T a, T b) where T : INumber<T> => T.Min(a, b);

    /// <summary>
    /// Gets the minimum of all numbers passed
    /// </summary>
    /// <typeparam name="T">Type of numbers</typeparam>
    /// <param name="numbers">List of numbers to get the minimum of</param>
    /// <returns>The minimum of all the passed numbers</returns>
    public static T Min<T>(params T[] numbers) where T : INumber<T> => numbers.Aggregate(Min);

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
            area += (current.X * next.Y) - (next.X * current.Y);
            current = next;
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
