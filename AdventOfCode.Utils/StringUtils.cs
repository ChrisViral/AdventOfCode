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
}
