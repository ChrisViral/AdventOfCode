using System.Numerics;
using JetBrains.Annotations;

namespace AdventOfCode.Utils;

/// <summary>
/// Number value utils
/// </summary>
/// <typeparam name="T">Number type</typeparam>
[PublicAPI]
public static class NumberUtils<T> where T : INumber<T>
{
    /// <summary>
    /// Two constant
    /// </summary>
    public static T Two { get; }   = T.One + T.One;
    /// <summary>
    /// Three constant
    /// </summary>
    public static T Three { get; } = T.One + Two;
    /// <summary>
    /// Four constant
    /// </summary>
    public static T Four { get; }  = T.One + Three;
    /// <summary>
    /// Five constant
    /// </summary>
    public static T Five { get; }  = T.One + Four;
    /// <summary>
    /// Six constant
    /// </summary>
    public static T Six { get; }   = T.One + Five;
    /// <summary>
    /// Seven constant
    /// </summary>
    public static T Seven { get; } = T.One + Six;
    /// <summary>
    /// Ten constant
    /// </summary>
    public static T Ten { get; }   = Seven + Three;

    /// <summary>
    /// Creates a number of this type from another number
    /// </summary>
    public static T Create<TFrom>(TFrom from) where TFrom : INumber<TFrom> => T.CreateChecked(from);
}
