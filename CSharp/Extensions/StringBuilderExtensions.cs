using System.Text;

namespace AdventOfCode.Extensions;

public static class StringBuilderExtensions
{
    public static string ToStringAndClear(this StringBuilder sb)
    {
        string toString = sb.ToString();
        sb.Clear();
        return toString;
    }
}
