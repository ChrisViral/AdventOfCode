﻿using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using AdventOfCode.Utils;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Extensions.Numbers;

/// <summary>
/// Number extensions
/// </summary>
[PublicAPI]
public static class NumberExtensions
{
    #region Extension methods
    /// <summary>
    /// Counts the digits in a given number
    /// </summary>
    /// <param name="n">Number to count the digits of</param>
    /// <typeparam name="T">Number type</typeparam>
    /// <returns>The amount of digits in <paramref name="n"/></returns>
    public static int DigitsCount<T>(this T n) where T : IBinaryInteger<T>
    {
        if (n == T.Zero) return 1;

        n = T.Abs(n);
        int digits = 1;
        while (n >= NumberUtils<T>.Ten)
        {
            digits++;
            n /= NumberUtils<T>.Ten;
        }
        return digits;
    }

    /// <summary>
    /// Calculates the Nth power of 10
    /// </summary>
    /// <param name="n">Nth power to get</param>
    /// <typeparam name="T">Number type</typeparam>
    /// <returns>Nth power of 10</returns>
    public static T Pow10<T>(this T n) where T : IBinaryInteger<T>
    {
        if (n < T.Zero) return T.Zero;
        if (n == T.Zero) return T.One;

        T pow = NumberUtils<T>.Ten;
        while (n --> T.One)
        {
            pow *= NumberUtils<T>.Ten;
        }

        return pow;
    }

    /// <summary>
    /// Concatenates the specified number at the end of the current number
    /// </summary>
    /// <param name="n">Current number</param>
    /// <param name="other">Number to concatenate</param>
    /// <typeparam name="T">Number type</typeparam>
    /// <returns>The concatenated result</returns>
    public static T ConcatNum<T>(this T n, T other) where T : IBinaryInteger<T>
    {
        T pow = T.One;
        while (pow <= other)
        {
            pow *= NumberUtils<T>.Ten;
        }
        return (n * pow) + other;
    }

    /// <summary>
    /// True mod function (so clamped from [0, mod[
    /// </summary>
    /// <typeparam name="T">Number type</typeparam>
    /// <param name="n">Number to mod</param>
    /// <param name="mod">Mod value</param>
    /// <returns>The true mod of <paramref name="n"/> and <paramref name="mod"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Mod<T>(this T n, T mod) where T : INumber<T> => ((n % mod) + mod) % mod;

    /// <summary>
    /// Gets the <paramref name="n"/>th triangular number
    /// </summary>
    /// <typeparam name="T">Type of integer</typeparam>
    /// <param name="n">Nth triangular number to get</param>
    /// <returns>The <paramref name="n"/>th triangular number</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Triangular<T>(this T n) where T : IBinaryInteger<T> => (n * (n + T.One)) / NumberUtils<T>.Two;

    /// <summary>
    /// Gets the <paramref name="n"/>th - 1 triangular number
    /// </summary>
    /// <typeparam name="T">Type of integer</typeparam>
    /// <param name="n">Nth - 1 triangular number to get</param>
    /// <returns>The <paramref name="n"/>th - 1 triangular number</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T PreviousTriangular<T>(this T n) where T : IBinaryInteger<T> => (n * (n - T.One)) / NumberUtils<T>.Two;

    /// <summary>
    /// Checks if an integer is prime or not
    /// </summary>
    /// <typeparam name="T">Type of number</typeparam>
    /// <param name="n">Number to check</param>
    /// <returns><see langword="true"/> if the number is prime, otherwise <see langword="false"/></returns>
    public static bool IsPrime<T>(this T n) where T : IBinaryInteger<T>
    {
        // Check for one, zero, or negatives
        if (n <= T.One) return false;

        // Check low primes
        if (n == NumberUtils<T>.Two || n == NumberUtils<T>.Three || n == NumberUtils<T>.Five) return true;

        // Check factors for low primes
        if (n.IsEven() || n.IsMultiple(NumberUtils<T>.Three) || n.IsMultiple(NumberUtils<T>.Five)) return false;

        // Get square root of n
        T limit = MathUtils.CeilToInt<double, T>(Math.Sqrt(double.CreateChecked(n)));
        for (T i = NumberUtils<T>.Seven; i <= limit; i += NumberUtils<T>.Six)
        {
            // We don't need to check anything that is a multiple of two, three, or five
            if (n.IsMultiple(i) || n.IsMultiple(i + NumberUtils<T>.Four))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if an integer is even or not
    /// </summary>
    /// <typeparam name="T">Integer type</typeparam>
    /// <param name="n">Number to check</param>
    /// <returns><see langword="true"/> if <paramref name="n"/> is event, <see langword="false"/> otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEven<T>(this T n) where T : IBinaryInteger<T> => n % NumberUtils<T>.Two == T.Zero;

    /// <summary>
    /// Check if this integer is a multiple of <paramref name="n"/>
    /// </summary>
    /// <typeparam name="T">Integer type</typeparam>
    /// <param name="num">Number to check</param>
    /// <param name="n">Multiple value to check</param>
    /// <returns><see langword="true"/> if this is a multiple of <paramref name="n"/>, <see langword="false"/> otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMultiple<T>(this T num, T n) where T : IBinaryInteger<T> => num % n == T.Zero;

    /// <summary>
    /// Check if this integer is a factor of <paramref name="n"/>
    /// </summary>
    /// <typeparam name="T">Integer type</typeparam>
    /// <param name="num">Factor to check</param>
    /// <param name="n">Number to check against</param>
    /// <returns><see langword="true"/> if this is a factor of <paramref name="n"/>, <see langword="false"/> otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFactor<T>(this T num, T n) where T : IBinaryInteger<T> => n % num == T.Zero;
    #endregion
}
