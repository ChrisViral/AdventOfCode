using System;
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
            T limit = MathUtils.CeilToInt<double, T>(Math.Sqrt(double.CreateChecked(value)));
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
    }
    #endregion
}
