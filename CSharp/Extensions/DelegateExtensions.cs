using System;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Extensions.Delegates;

/// <summary>
/// Delegate function extensions
/// </summary>
[PublicAPI]
public static class DelegateExtensions
{
    /// <summary>
    /// Inverts the result of a given predicate function
    /// </summary>
    /// <param name="predicate">Predicate function to invert</param>
    /// <typeparam name="T">Predicate parameter type</typeparam>
    /// <returns>A function where the result of the original predicate is inverted</returns>
    public static Func<T, bool> Invert<T>(this Func<T, bool> predicate) => x => !predicate(x);

    /// <summary>
    /// Inverts the result of a given predicate function
    /// </summary>
    /// <param name="predicate">Predicate function to invert</param>
    /// <typeparam name="T">Predicate parameter type</typeparam>
    /// <returns>A function where the result of the original predicate is inverted</returns>
    public static Func<T, int, bool> Invert<T>(this Func<T, int, bool> predicate) => (x, i) => !predicate(x, i);

    /// <summary>
    /// Inverts the result of a given predicate function
    /// </summary>
    /// <param name="predicate">Predicate function to invert</param>
    /// <typeparam name="T">Predicate parameter type</typeparam>
    /// <returns>A function where the result of the original predicate is inverted</returns>
    public static Predicate<T> Invert<T>(this Predicate<T> predicate) => x => !predicate(x);
}