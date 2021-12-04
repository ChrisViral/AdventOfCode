using System;

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
        public static T Two { get; } = T.One + T.One;
    }

    #region Extension methods
    /// <summary>
    /// True mod function (so clamped from [0, mod[
    /// </summary>
    /// <typeparam name="T">Number type</typeparam>
    /// <param name="n">Number to mod</param>
    /// <param name="mod">Mod value</param>
    /// <returns>The true mod of <paramref name="n"/> and <paramref name="mod"/></returns>
    public static T Mod<T>(this T n, T mod) where T : INumber<T> => ((n % mod) + mod) % mod;

    /// <summary>
    /// Gets the <paramref name="n"/>th triangular number
    /// </summary>
    /// <typeparam name="T">Type of integer</typeparam>
    /// <param name="n">Nth triangular number to get</param>
    /// <returns>The <paramref name="n"/>th triangular number</returns>
    public static T Triangular<T>(this T n) where T : IBinaryInteger<T> => (n * (n + T.One)) / Numbers<T>.Two;
    #endregion
}
