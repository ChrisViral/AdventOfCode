using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using AdventOfCode.Utils;
using JetBrains.Annotations;
using SpanLinq;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Extensions.Numbers;

/// <summary>
/// Number extensions
/// </summary>
[PublicAPI]
public static class NumberExtensions
{
    /// <param name="value">Number to count the digits of</param>
    /// <typeparam name="T">Number type</typeparam>
    extension<T>(T value) where T : IBinaryInteger<T>
    {
        /// <summary>
        /// Checks if an integer is even or not
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if <paramref name="value"/> is event, <see langword="false"/> otherwise
        /// </value>
        public bool IsEven
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value % NumberUtils<T>.Two == T.Zero;
        }

        /// <summary>
        /// Counts the digits in a given number
        /// </summary>
        /// <returns>The amount of digits in <paramref name="value"/></returns>
        public int DigitsCount()
        {
            if (value == T.Zero) return 1;

            value = T.Abs(value);
            int digits = 1;
            while (value >= NumberUtils<T>.Ten)
            {
                digits++;
                value /= NumberUtils<T>.Ten;
            }
            return digits;
        }

        /// <summary>
        /// Calculates the Nth power of 10
        /// </summary>
        /// <returns>Nth power of 10</returns>
        public T Pow10()
        {
            if (value < T.Zero) return T.Zero;
            if (value == T.Zero) return T.One;

            T pow = NumberUtils<T>.Ten;
            while (value --> T.One)
            {
                pow *= NumberUtils<T>.Ten;
            }

            return pow;
        }

        /// <summary>
        /// Concatenates the specified number at the end of the current number
        /// </summary>
        /// <param name="other">Number to concatenate</param>
        /// <returns>The concatenated result</returns>
        public T ConcatNum(T other)
        {
            T pow = T.One;
            while (pow <= other)
            {
                pow *= NumberUtils<T>.Ten;
            }
            return (value * pow) + other;
        }

        /// <summary>
        /// Gets the <paramref name="value"/>th triangular number
        /// </summary>
        /// <returns>The <paramref name="value"/>th triangular number</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Triangular() => (value * (value + T.One)) / NumberUtils<T>.Two;

        /// <summary>
        /// Gets the <paramref name="value"/>th - 1 triangular number
        /// </summary>
        /// <returns>The <paramref name="value"/>th - 1 triangular number</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PreviousTriangular() => (value * (value - T.One)) / NumberUtils<T>.Two;

        /// <summary>
        /// Checks if an integer is prime or not
        /// </summary>
        /// <returns><see langword="true"/> if the number is prime, otherwise <see langword="false"/></returns>
        public bool IsPrime()
        {
            // Check for one, zero, or negatives
            if (value <= T.One) return false;

            // Check low primes
            if (value == NumberUtils<T>.Two || value == NumberUtils<T>.Three || value == NumberUtils<T>.Five) return true;

            // Check factors for low primes
            if (value.IsEven || value.IsMultiple(NumberUtils<T>.Three) || value.IsMultiple(NumberUtils<T>.Five)) return false;

            // Get square root of n
            T limit = double.CeilToInt<double, T>(Math.Sqrt(double.CreateChecked(value)));
            for (T i = NumberUtils<T>.Seven; i <= limit; i += NumberUtils<T>.Six)
            {
                // We don't need to check anything that is a multiple of two, three, or five
                if (value.IsMultiple(i) || value.IsMultiple(i + NumberUtils<T>.Four))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if this integer is a multiple of the value
        /// </summary>
        /// <param name="n">Multiple value to check</param>
        /// <returns><see langword="true"/> if this is a multiple of the value, <see langword="false"/> otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMultiple(T n) => value % n == T.Zero;

        /// <summary>
        /// Check if this integer is a factor of <paramref name="n"/>
        /// </summary>
        /// <param name="n">Number to check against</param>
        /// <returns><see langword="true"/> if this is a factor of <paramref name="n"/>, <see langword="false"/> otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsFactor(T n) => n % value == T.Zero;

        /// <summary>
        /// Greatest Common Divisor function
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Gets the GCD of a and b</returns>
        /// ReSharper disable once MemberCanBePrivate.Global
        public static T GCD(T a, T b)
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
        public static T GCD(params Span<T> numbers) => numbers.Aggregate(GCD);

        /// <summary>
        /// Greatest Common Divisor of all passed numbers
        /// </summary>
        /// <param name="numbers">Numbers to get the GCD for</param>
        /// <returns>Gets the GCD of all the passed numbers</returns>
        public static T GCD([InstantHandle] params IEnumerable<T> numbers) => numbers.Aggregate(GCD);

        /// <summary>
        /// Least Common Multiple function
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>The LCM of a and b</returns>
        public static T LCM(T a, T b) => a * b / GCD(a, b);

        /// <summary>
        /// Least Common Multiple function
        /// </summary>
        /// <param name="numbers">Numbers to get the LCM for</param>
        /// <returns>LCM of all the numbers in the array</returns>
        public static T LCM(params Span<T> numbers) => numbers.Aggregate(LCM);

        /// <summary>
        /// Least Common Multiple function
        /// </summary>
        /// <param name="numbers">Numbers to get the LCM for</param>
        /// <returns>LCM of all the numbers in the array</returns>
        public static T LCM([InstantHandle] params IEnumerable<T> numbers) => numbers.Aggregate(LCM);
    }

    /// <typeparam name="T">Number type</typeparam>
    extension<T>(T n) where T : INumber<T>
    {
        /// <summary>
        /// True mod function (so clamped from [0, mod[
        /// </summary>
        /// <param name="mod">Mod value</param>
        /// <returns>The true mod of <paramref name="n"/> and <paramref name="mod"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Mod(T mod) => ((n % mod) + mod) % mod;

        /// <summary>
        /// Gets the maximum of all numbers passed
        /// </summary>
        /// <typeparam name="T">Type of numbers</typeparam>
        /// <param name="numbers">List of numbers to get the maximum of</param>
        /// <returns>The maximum of all the passed numbers</returns>
        public static T Max(params Span<T> numbers) => numbers.Aggregate(T.Max);

        /// <summary>
        /// Gets the maximum of all numbers passed
        /// </summary>
        /// <typeparam name="T">Type of numbers</typeparam>
        /// <param name="numbers">List of numbers to get the maximum of</param>
        /// <returns>The maximum of all the passed numbers</returns>
        public static T Max([InstantHandle] params IEnumerable<T> numbers) => numbers.Aggregate(T.Max);

        /// <summary>
        /// Gets the minimum of all numbers passed
        /// </summary>
        /// <typeparam name="T">Type of numbers</typeparam>
        /// <param name="numbers">List of numbers to get the minimum of</param>
        /// <returns>The minimum of all the passed numbers</returns>
        public static T Min(params Span<T> numbers) => numbers.Aggregate(T.Min);

        /// <summary>
        /// Gets the minimum of all numbers passed
        /// </summary>
        /// <typeparam name="T">Type of numbers</typeparam>
        /// <param name="numbers">List of numbers to get the minimum of</param>
        /// <returns>The minimum of all the passed numbers</returns>
        public static T Min([InstantHandle] params IEnumerable<T> numbers) => numbers.Aggregate(T.Min);
    }

    /// <typeparam name="T">Floating point type</typeparam>
    extension<T>(T) where T : IFloatingPoint<T>
    {
        /// <summary>
        /// Compares two floating point numbers to see if they are nearly identical
        /// </summary>
        /// <param name="a">First number to test</param>
        /// <param name="b">Second number to test</param>
        /// <returns><see langword="true"/> if <paramref name="a"/> and <paramref name="b"/> are approximately equal, otherwise <see langword="false"/></returns>
        public static bool Approximately(T a, T b) => T.Abs(a - b) <= FloatUtils<T>.Epsilon;

        /// <summary>
        /// Gets the ceiling of a floating point value as an integer number
        /// </summary>
        /// <typeparam name="TResult">Integer target type</typeparam>
        /// <param name="value">The value to get the ceiling for</param>
        /// <returns>The ceiling of <paramref name="value"/> as an integer</returns>
        public static TResult CeilToInt<TResult>(T value) where TResult : IBinaryInteger<TResult>
        {
            return TResult.CreateChecked(T.Ceiling(value));
        }

        /// <summary>
        /// Gets the floor of a floating point value as an integer number
        /// </summary>
        /// <typeparam name="TResult">Integer target type</typeparam>
        /// <param name="value">The value to get the floor for</param>
        /// <returns>The floor of <paramref name="value"/> as an integer</returns>
        public static TResult FloorToInt<TResult>(T value) where TResult : IBinaryInteger<TResult>
        {
            return TResult.CreateChecked(T.Floor(value));
        }
    }
}
