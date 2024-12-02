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
    /// <summary>
    /// Compiles the StringBuilder to it's contained value, then clears it
    /// </summary>
    /// <param name="sb">The StringBuilder to get the value for</param>
    /// <returns>The compiled string contained in <paramref name="sb"/></returns>
    public static string ToStringAndClear(this StringBuilder sb)
    {
        string toString = sb.ToString();
        sb.Clear();
        return toString;
    }
}
