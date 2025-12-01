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
    extension(StringBuilder sb)
    {
        /// <summary>
        /// Compiles the StringBuilder to it's contained value, then clears it
        /// </summary>
        /// <returns>The compiled string contained in this StringBuilder</returns>
        public string ToStringAndClear()
        {
            string toString = sb.ToString();
            sb.Clear();
            return toString;
        }
    }
}
