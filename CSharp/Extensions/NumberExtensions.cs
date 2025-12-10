using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        /// <value>The <paramref name="value"/>th triangular number</value>
        public T Triangular
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (value * (value + T.One)) / NumberUtils<T>.Two;
        }

        /// <summary>
        /// Gets the <paramref name="value"/>th - 1 triangular number
        /// </summary>
        /// <value>The <paramref name="value"/>th - 1 triangular number</value>
        public T PreviousTriangular
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (value * (value - T.One)) / NumberUtils<T>.Two;
        }

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
        /// Checks if the given value is within the defined range [min, max[
        /// </summary>
        /// <param name="min">Minimum value of the range, inclusive</param>
        /// <param name="max">Maximum value of the range, exclusive</param>
        /// <returns><see langword="true"/> if the value is within the range, otherwise <see langword="false"/></returns>
        /// <exception cref="ArgumentException">If <paramref name="max"/> is smaller than <paramref name="min"/></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInRange(T min, T max) => min <= max
                                                   ? value >= min && value < max
                                                   : throw new ArgumentException("Max value must be larger or equal to min value", nameof(max));

        /// <summary>
        /// Greatest Common Divisor function
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Gets the GCD of a and b</returns>
        /// ReSharper disable once MemberCanBePrivate.Global
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GCD(T a, T b) => MathUtils.GCD(a, b);

        /// <summary>
        /// Greatest Common Divisor of all passed numbers
        /// </summary>
        /// <param name="numbers">Numbers to get the GCD for</param>
        /// <returns>Gets the GCD of all the passed numbers</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GCD(params Span<T> numbers) => numbers.Aggregate(GCD);

        /// <summary>
        /// Greatest Common Divisor of all passed numbers
        /// </summary>
        /// <param name="numbers">Numbers to get the GCD for</param>
        /// <returns>Gets the GCD of all the passed numbers</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GCD([InstantHandle] params IEnumerable<T> numbers) => numbers.Aggregate(GCD);

        /// <summary>
        /// Least Common Multiple function
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>The LCM of a and b</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T LCM(T a, T b) => a * b / GCD(a, b);

        /// <summary>
        /// Least Common Multiple function
        /// </summary>
        /// <param name="numbers">Numbers to get the LCM for</param>
        /// <returns>LCM of all the numbers in the array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T LCM(params Span<T> numbers) => numbers.Aggregate(LCM);

        /// <summary>
        /// Least Common Multiple function
        /// </summary>
        /// <param name="numbers">Numbers to get the LCM for</param>
        /// <returns>LCM of all the numbers in the array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T LCM([InstantHandle] params IEnumerable<T> numbers) => numbers.Aggregate(LCM);
    }

    extension(int value)
    {
        /// <summary>
        /// Counts the digits in a given number
        /// </summary>
        public int DigitCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => int.Abs(value) switch
            {
                < 10            => 1,
                < 100           => 2,
                < 1_000         => 3,
                < 10_000        => 4,
                < 100_000       => 5,
                < 1_000_000     => 6,
                < 10_000_000    => 7,
                < 100_000_000   => 8,
                < 1_000_000_000 => 9,
                _               => 10
            };
        }

        /// <summary>
        /// Calculates the Nth power of 10
        /// </summary>
        /// <value>Nth power of 10 as an integer</value>
        public int Pow10
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value switch
            {
                < 0 => 0,
                0   => 1,
                1   => 10,
                2   => 100,
                3   => 1_000,
                4   => 10_000,
                5   => 100_000,
                6   => 1_000_000,
                7   => 10_000_000,
                8   => 100_000_000,
                9   => 1_000_000_000,
                _   => throw new OverflowException("Power exceeds 32bit integer range")
            };
        }

        /// <summary>
        /// Calculates the Nth power of 10
        /// </summary>
        /// <value>Nth power of 10 as a long</value>
        public long LongPow10
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value switch
            {
                < 0 => 0L,
                0   => 1L,
                1   => 10L,
                2   => 100L,
                3   => 1_000L,
                4   => 10_000L,
                5   => 100_000L,
                6   => 1_000_000L,
                7   => 10_000_000L,
                8   => 100_000_000L,
                9   => 1_000_000_000L,
                10  => 10_000_000_000L,
                11  => 100_000_000_000L,
                12  => 1_000_000_000_000L,
                13  => 10_000_000_000_000L,
                14  => 100_000_000_000_000L,
                15  => 1_000_000_000_000_000L,
                16  => 10_000_000_000_000_000L,
                17  => 100_000_000_000_000_000L,
                18  => 1_000_000_000_000_000_000L,
                _   => throw new OverflowException("Power exceeds 64bit integer range")
            };
        }
    }

    extension(long value)
    {
        /// <summary>
        /// Counts the digits in a given number
        /// </summary>
        /// ReSharper disable once CognitiveComplexity
        public int DigitCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => long.Abs(value) switch
            {
                < 10L                        => 1,
                < 100L                       => 2,
                < 1_000L                     => 3,
                < 10_000L                    => 4,
                < 100_000L                   => 5,
                < 1_000_000L                 => 6,
                < 10_000_000L                => 7,
                < 100_000_000L               => 8,
                < 1_000_000_000L             => 9,
                < 10_000_000_000L            => 10,
                < 100_000_000_000L           => 11,
                < 1_000_000_000_000L         => 12,
                < 10_000_000_000_000L        => 13,
                < 100_000_000_000_000L       => 14,
                < 1_000_000_000_000_000L     => 15,
                < 10_000_000_000_000_000L    => 16,
                < 100_000_000_000_000_000L   => 17,
                < 1_000_000_000_000_000_000L => 18,
                _                            => 19
            };
        }
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Max(params Span<T> numbers) => numbers.Aggregate(T.Max);

        /// <summary>
        /// Gets the maximum of all numbers passed
        /// </summary>
        /// <typeparam name="T">Type of numbers</typeparam>
        /// <param name="numbers">List of numbers to get the maximum of</param>
        /// <returns>The maximum of all the passed numbers</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Max([InstantHandle] params IEnumerable<T> numbers) => numbers.Aggregate(T.Max);

        /// <summary>
        /// Gets the minimum of all numbers passed
        /// </summary>
        /// <typeparam name="T">Type of numbers</typeparam>
        /// <param name="numbers">List of numbers to get the minimum of</param>
        /// <returns>The minimum of all the passed numbers</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Min(params Span<T> numbers) => numbers.Aggregate(T.Min);

        /// <summary>
        /// Gets the minimum of all numbers passed
        /// </summary>
        /// <typeparam name="T">Type of numbers</typeparam>
        /// <param name="numbers">List of numbers to get the minimum of</param>
        /// <returns>The minimum of all the passed numbers</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(T a, T b) => T.Abs(a - b) <= FloatUtils<T>.Epsilon;

        /// <summary>
        /// Gets the ceiling of a floating point value as an integer number
        /// </summary>
        /// <typeparam name="TResult">Integer target type</typeparam>
        /// <param name="value">The value to get the ceiling for</param>
        /// <returns>The ceiling of <paramref name="value"/> as an integer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult FloorToInt<TResult>(T value) where TResult : IBinaryInteger<TResult>
        {
            return TResult.CreateChecked(T.Floor(value));
        }
    }

    extension(ref BitVector32 vector)
    {
        /// <summary>
        /// Sets the bit at the given index
        /// </summary>
        /// <param name="index">Index to set the bit at</param>
        /// <param name="value">Whether the bit is on or off</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBit(int index, bool value) => vector[1 << index] = value;
    }
}
