using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Extensions.Delegates;

/// <summary>
/// Delegate function extensions
/// </summary>
[PublicAPI]
public static class DelegateExtensions
{
    /// <typeparam name="T">Predicate parameter type</typeparam>
    extension<T>([InstantHandle] Func<T, bool> predicate)
    {
        /// <summary>
        /// Inverts the result of a given predicate function
        /// </summary>
        /// <returns>A function where the result of the original predicate is inverted</returns>
        public Func<T, bool> Inverted => x => !predicate(x);
    }

    /// <typeparam name="T">Predicate parameter type</typeparam>
    extension<T>([InstantHandle] Func<T, int, bool> predicate)
    {
        /// <summary>
        /// Inverts the result of a given predicate function
        /// </summary>
        /// <returns>A function where the result of the original predicate is inverted</returns>
        public Func<T, int, bool> Inverted => (x, i) => !predicate(x, i);
    }

    /// <typeparam name="T">Predicate parameter type</typeparam>
    extension<T>([InstantHandle] Predicate<T> predicate)
    {
        /// <summary>
        /// Inverts the result of a given predicate function
        /// </summary>
        /// <returns>A function where the result of the original predicate is inverted</returns>
        public Predicate<T> Inverted => x => !predicate(x);
    }
}
