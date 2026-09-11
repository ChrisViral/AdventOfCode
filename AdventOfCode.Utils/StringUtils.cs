using System.Buffers;
using JetBrains.Annotations;

namespace AdventOfCode.Utils;

/// <summary>
/// String utilities
/// </summary>
[PublicAPI]
public static class StringUtils
{
    /// <summary>
    /// Lowercase ASCII letters
    /// </summary>
    public const string ASCII_LOWER = "abcdefghijklmnopqrstuvwxyz";
    /// <summary>
    /// Uppercase ASCII letters
    /// </summary>
    public const string ASCII_UPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    /// <summary>
    /// ASCII digits
    /// </summary>
    public const string ASCII_DIGITS = "0123456789";
    /// <summary>
    /// Lowercase hex digits
    /// </summary>
    public const string HEX_LOWER = "0123456789abcdef";
    /// <summary>
    /// Uppercase hex digits
    /// </summary>
    public const string HEX_UPPER = "0123456789ABCDEF";
    /// <summary>
    /// Amount of ASCII letters
    /// </summary>
    public const int LETTER_COUNT = 26;
    /// <summary>
    /// Amount of ASCII digits
    /// </summary>
    public const int DIGIT_COUNT = 10;
    /// <summary>
    /// Amount of hex digits
    /// </summary>
    public const int HEX_COUNT = 16;

    /// <summary>
    /// SearchValues for lowercase ASCII letter pairs
    /// </summary>
    public static SearchValues<string> PairsLowercase { get; } = SearchValues.Create([
        "aa",
        "bb",
        "cc",
        "dd",
        "ee",
        "ff",
        "gg",
        "hh",
        "ii",
        "jj",
        "kk",
        "ll",
        "mm",
        "nn",
        "oo",
        "pp",
        "qq",
        "rr",
        "ss",
        "tt",
        "uu",
        "vv",
        "ww",
        "xx",
        "yy",
        "zz"
    ], StringComparison.Ordinal);

    /// <summary>
    /// SearchValues for uppercase ASCII letter pairs
    /// </summary>
    public static SearchValues<string> PairsUppercase { get; } = SearchValues.Create([
        "AA",
        "BB",
        "CC",
        "DD",
        "EE",
        "FF",
        "GG",
        "HH",
        "II",
        "JJ",
        "KK",
        "LL",
        "MM",
        "NN",
        "OO",
        "PP",
        "QQ",
        "RR",
        "SS",
        "TT",
        "UU",
        "VV",
        "WW",
        "XX",
        "YY",
        "ZZ"
    ], StringComparison.Ordinal);
}
