using System.Linq;
using System.Numerics;

namespace AdventOfCode.Utils;

/// <summary>
/// Mathematics utils
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// Compares two floating point numbers to see if they are nearly identical
    /// </summary>
    /// <typeparam name="T">Floating point number type</typeparam>
    /// <param name="a">First number to test</param>
    /// <param name="b">Second number to test</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> and <paramref name="b"/> are approximately equal, otherwise <see langword="false"/></returns>
    public static bool Approximately<T>(T a, T b) where T : IFloatingPoint<T> => T.Abs(a - b) <= FloatUtils<T>.Epsilon;

    /// <summary>
    /// Gets the ceiling of a floating point value as an integer number
    /// </summary>
    /// <typeparam name="TSelf">Floating point source type</typeparam>
    /// <typeparam name="TResult">Integer target type</typeparam>
    /// <param name="value">The value to get the ceiling for</param>
    /// <returns>The ceiling of <paramref name="value"/> as an integer</returns>
    public static TResult CeilToInt<TSelf, TResult>(TSelf value) where TSelf   : IFloatingPoint<TSelf>
                                                                 where TResult : IBinaryInteger<TResult>
    {
        return TResult.CreateChecked(TSelf.Ceiling(value));
    }

    /// <summary>
    /// Gets the floor of a floating point value as an integer number
    /// </summary>
    /// <typeparam name="TSelf">Floating point source type</typeparam>
    /// <typeparam name="TResult">Integer target type</typeparam>
    /// <param name="value">The value to get the floor for</param>
    /// <returns>The floor of <paramref name="value"/> as an integer</returns>
    public static TResult FloorToInt<TSelf, TResult>(TSelf value) where TSelf   : IFloatingPoint<TSelf>
                                                                  where TResult : IBinaryInteger<TResult>
    {
        return TResult.CreateChecked(TSelf.Floor(value));
    }

    /// <summary>
    /// Greatest Common Divisor function
    /// </summary>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>Gets the GCD of a and b</returns>
    /// ReSharper disable once MemberCanBePrivate.Global
    public static T GCD<T>(T a, T b) where T : IBinaryInteger<T>
    {
        a = T.Abs(a);
        b = T.Abs(b);
        while (a != T.Zero && b != T.Zero)
        {
            if (a > b)
            {
                a %= b;
            }
            else
            {
                b %= a;
            }
        }

        return a | b;
    }

    /// <summary>
    /// Greatest Common Divisor of all passed numbers
    /// </summary>
    /// <param name="numbers">Numbers to get the GCD for</param>
    /// <returns>Gets the GCD of all the passed numbers</returns>
    public static T GCD<T>(params T[] numbers) where T : IBinaryInteger<T> => numbers.Aggregate(GCD);

    /// <summary>
    /// Least Common Multiple function
    /// </summary>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>The LCM of a and b</returns>
    public static T LCM<T>(T a, T b) where T : IBinaryInteger<T> => a * b / GCD(a, b);

    /// <summary>
    /// Least Common Multiple function
    /// </summary>
    /// <param name="numbers">Numbers to get the LCM for</param>
    /// <returns>LCM of all the numbers in the array</returns>
    public static T LCM<T>(params T[] numbers) where T : IBinaryInteger<T> => numbers.Aggregate(LCM);

    /// <summary>
    /// Gets the maximum value between <paramref name="a"/> and <paramref name="b"/>
    /// </summary>
    /// <typeparam name="T">Type of number</typeparam>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>The maximum from both numbers</returns>
    /// ReSharper disable once MemberCanBePrivate.Global
    public static T Max<T>(T a, T b) where T : INumber<T> => T.Max(a, b);

    /// <summary>
    /// Gets the maximum of all numbers passed
    /// </summary>
    /// <typeparam name="T">Type of numbers</typeparam>
    /// <param name="numbers">List of numbers to get the maximum of</param>
    /// <returns>The maximum of all the passed numbers</returns>
    public static T Max<T>(params T[] numbers) where T : INumber<T> => numbers.Aggregate(Max);

    /// <summary>
    /// Gets the minimum value between <paramref name="a"/> and <paramref name="b"/>
    /// </summary>
    /// <typeparam name="T">Type of number</typeparam>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>The minimum from both numbers</returns>
    /// ReSharper disable once MemberCanBePrivate.Global
    public static T Min<T>(T a, T b) where T : INumber<T> => T.Min(a, b);

    /// <summary>
    /// Gets the minimum of all numbers passed
    /// </summary>
    /// <typeparam name="T">Type of numbers</typeparam>
    /// <param name="numbers">List of numbers to get the minimum of</param>
    /// <returns>The minimum of all the passed numbers</returns>
    public static T Min<T>(params T[] numbers) where T : INumber<T> => numbers.Aggregate(Min);
}
