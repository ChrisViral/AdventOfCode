using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.Utils.Extensions;

/// <summary>
/// Number extensions
/// </summary>
public static class NumberExtensions
{
    /// <summary>
    /// Number constants storage
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private static class Numbers<T> where T : INumber<T>
    {
        /// <summary>
        /// Two constant
        /// </summary>
        public static T Two { get; }   = T.One + T.One;
        /// <summary>
        /// Three constant
        /// </summary>
        public static T Three { get; } = Two   + T.One;
        /// <summary>
        /// Four constant
        /// </summary>
        public static T Four { get; }  = Three + T.One;
        /// <summary>
        /// Five constant
        /// </summary>
        public static T Five { get; }  = Four  + T.One;
        /// <summary>
        /// Six constant
        /// </summary>
        public static T Six { get; }   = Five  + T.One;
        /// <summary>
        /// Seven constant
        /// </summary>
        public static T Seven { get; } = Six   + T.One;

        /// <summary>
        /// Creates a number of this type from another number
        /// </summary>
        public static T Create<TFrom>(TFrom from) where TFrom : INumber<TFrom> => T.Create(from);
    }

    #region Extension methods
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
    public static T Triangular<T>(this T n) where T : IBinaryInteger<T> => (n * (n + T.One)) / Numbers<T>.Two;

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
        if (n == Numbers<T>.Two || n == Numbers<T>.Three || n == Numbers<T>.Five) return true;
        // Check factors for low primes
        if (n % Numbers<T>.Two == T.Zero || n % Numbers<T>.Three == T.Zero || n % Numbers<T>.Five == T.Zero) return false;

        // Get square root of n
        T limit = T.Create(Math.Sqrt(Numbers<double>.Create(n)));
        for (T i = Numbers<T>.Seven; i <= limit; i += Numbers<T>.Six)
        {
            // We don't need to check anything that is a multiple of two, three, or five
            if (n % i == T.Zero || n % (i + Numbers<T>.Four) == T.Zero)
            {
                return false;
            }
        }

        return true;
    }
    #endregion
}
