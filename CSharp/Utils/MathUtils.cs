using System;
using System.Linq;

namespace AdventOfCode.Utils;

public static class MathUtils
{
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
    /// Gets the minimum value between <paramref name="a"/> and <paramref name="b"/>
    /// </summary>
    /// <typeparam name="T">Type of number</typeparam>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>The minimum from both numbers</returns>
    public static T Min<T>(T a, T b) where T : INumber<T> => T.Min(a, b);

    /// <summary>
    /// Gets the minimum of all numbers passed
    /// </summary>
    /// <typeparam name="T">Type of numbers</typeparam>
    /// <param name="numbers">List of numbers to get the minimum of</param>
    /// <returns>The minimum of all the passed numbers</returns>
    public static T Min<T>(params T[] numbers) where T : INumber<T> => numbers.Aggregate(Min);

    /// <summary>
    /// Gets the maximum value between <paramref name="a"/> and <paramref name="b"/>
    /// </summary>
    /// <typeparam name="T">Type of number</typeparam>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>The maximum from both numbers</returns>
    public static T Max<T>(T a, T b) where T : INumber<T> => T.Max(a, b);

    /// <summary>
    /// Gets the maximum of all numbers passed
    /// </summary>
    /// <typeparam name="T">Type of numbers</typeparam>
    /// <param name="numbers">List of numbers to get the maximum of</param>
    /// <returns>The maximum of all the passed numbers</returns>
    public static T Max<T>(params T[] numbers) where T : INumber<T> => numbers.Aggregate(Max);
}
