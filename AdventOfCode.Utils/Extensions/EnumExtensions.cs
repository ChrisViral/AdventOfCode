using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    extension<T>(T value) where T : struct, Enum
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
    }
}
