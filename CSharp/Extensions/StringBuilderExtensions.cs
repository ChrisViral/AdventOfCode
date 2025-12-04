using System;
using System.Text;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Extensions.StringBuilders;

/// <summary>
/// StringBuilder extension methods
/// </summary>
[PublicAPI]
public static class StringBuilderExtensions
{
    extension(StringBuilder stringBuilder)
    {
        /// <summary>
        /// Compiles the StringBuilder to it's contained value, then clears it
        /// </summary>
        /// <returns>The compiled string contained in this StringBuilder</returns>
        public string ToStringAndClear()
        {
            string toString = stringBuilder.ToString();
            stringBuilder.Clear();
            return toString;
        }

        /// <summary>
        /// Copies data from the start of the StringBuilder to fill the given span
        /// </summary>
        /// <param name="destination">Span to fill</param>
        public void CopyTo(Span<char> destination) => stringBuilder.CopyTo(0, destination, destination.Length);
    }
}
