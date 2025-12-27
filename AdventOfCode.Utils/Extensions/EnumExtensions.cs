using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using FastEnumUtility;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Utils.Extensions.Enums;

/// <summary>
/// Enum extensions
/// </summary>
[PublicAPI]
public static class EnumExtensions
{
    /// <param name="value">Enum value</param>
    /// <typeparam name="T">Enum type</typeparam>
    extension<T>(T value) where T : unmanaged, Enum
    {
        /// <summary>
        /// Creates an <see cref="InvalidEnumArgumentException"/> for the given enum value
        /// </summary>
        /// <param name="name">Name of the variable being thrown</param>
        /// <returns>An exception for the given enum value ready to be thrown</returns>
        public InvalidEnumArgumentException Invalid([CallerArgumentExpression(nameof(value))] string name = "")
        {
            return new InvalidEnumArgumentException(name, value.ToInt32(), typeof(T));
        }

        /// <summary>
        /// Throw an <see cref="InvalidEnumArgumentException"/> for the given enum value
        /// </summary>
        /// <param name="name">Name of the variable being thrown</param>
        /// <exception cref="InvalidEnumArgumentException">Always thrown</exception>
        [DoesNotReturn, StackTraceHidden]
        public void ThrowInvalid([CallerArgumentExpression(nameof(value))] string name = "")
        {
            throw new InvalidEnumArgumentException(name, value.ToInt32(), typeof(T));
        }

        /// <summary>
        /// Checks if the enum value has the given flags set
        /// </summary>
        /// <param name="flags">Flags to check for</param>
        /// <typeparam name="TInteger">Check integer type</typeparam>
        /// <returns><see langword="true"/> if the flags are set in <see cref="value"/>, otherwise <see langword="false"/></returns>
        public bool HasFlags<TInteger>(T flags)
            where TInteger : unmanaged, IBinaryInteger<TInteger>
        {
            return (Unsafe.As<T, TInteger>(ref value) & Unsafe.As<T, TInteger>(ref flags)) != TInteger.Zero;
        }

        /// <summary>
        /// Checks if the enum value has the given flags set using <see cref="int"/> for checks
        /// </summary>
        /// <param name="flags">Flags to check for</param>
        /// <returns><see langword="true"/> if the flags are set in <see cref="value"/>, otherwise <see langword="false"/></returns>
        public bool HasFlags(T flags)
        {
            return (Unsafe.As<T, int>(ref value) & Unsafe.As<T, int>(ref flags)) != 0;
        }
    }
}
