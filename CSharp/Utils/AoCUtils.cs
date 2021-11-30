using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AdventOfCode.Utils;

/// <summary>
/// General Advent of Code utility methods
/// </summary>
public static class AoCUtils
{
    #region Static methods
    /// <summary>
    /// Combines input lines into sequences, separated by empty lines
    /// </summary>
    /// <param name="input">Input lines</param>
    /// <returns>An enumerable of the packed input</returns>
    public static IEnumerable<List<string>> CombineLines(string[] input)
    {
        List<string> pack = new();
        foreach (string line in input)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (pack.Count is not 0)
                {
                    yield return pack;
                    pack = new List<string>();
                }
            }
            else
            {
                pack.Add(line);
            }
        }

        if (pack.Count is not 0)
        {
            yield return pack;
        }
    }

    /// <summary>
    /// Logs the answer to Part 1 to the console and results file
    /// </summary>
    /// <param name="answer">Answer to log</param>
    public static void LogPart1(object answer) => Trace.WriteLine($"Part 1: {answer}");
        
    /// <summary>
    /// Logs the answer to Part 3 to the console and results file
    /// </summary>
    /// <param name="answer">Answer to log</param>
    public static void LogPart2(object answer) => Trace.WriteLine($"Part 2: {answer}");

    /// <summary>
    /// Iterates over all the permutations of the given array
    /// </summary>
    /// <typeparam name="T">Type of element in the array</typeparam>
    /// <param name="array">Array to get the permutations for</param>
    /// <returns>An enumerable returning all the permutations of the original array</returns>
    public static IEnumerable<T[]> Permutations<T>(T[] array)
    {
        static T[] Copy(T[] original, int size)
        {
            T[] copy = new T[original.Length];
            if (size is not 0)
            {
                Buffer.BlockCopy(original, 0, copy, 0, size * original.Length);
            }
            else
            {
                original.CopyTo(copy, 0);
            }

            return copy;
        }
            
        static IEnumerable<T[]> GetPermutations(T[] working, int k, int size)
        {
            if (k == working.Length - 1)
            {
                T[] perm = Copy(working, size);
                yield return perm;
            }
            else
            {
                for (int i = k; i < working.Length; i++)
                {
                    Swap(ref working[k], ref working[i]);
                    foreach (T[] perm in GetPermutations(working, k + 1, size))
                    {
                        yield return perm;
                    }
                    Swap(ref working[k], ref working[i]);
                }
            }
        }

        int size = typeof(T).IsPrimitive ? GetSizeOfPrimitive<T>() : 0;
        return GetPermutations(Copy(array, size), 0, size);
    }

    /// <summary>
    /// Swaps two values in memory
    /// </summary>
    /// <typeparam name="T">Type of value to swap</typeparam>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    public static void Swap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }

    /// <summary>
    /// Greatest Common Divisor function
    /// </summary>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>Gets the GCD of a and b</returns>
    public static int GCD(int a, int b)
    {
        a = Math.Abs(a);
        b = Math.Abs(b);
        while (a is not 0 && b is not 0)
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
    /// Greatest Common Divisor function
    /// </summary>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>Gets the GCD of a and b</returns>
    public static long GCD(long a, long b)
    {
        a = Math.Abs(a);
        b = Math.Abs(b);
        while (a is not 0L && b is not 0L)
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
    /// Least Common Multiple function
    /// </summary>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>The LCM of a and b</returns>
    public static int LCM(int a, int b) => a * b / GCD(a, b);

    /// <summary>
    /// Least Common Multiple function
    /// </summary>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>The LCM of a and b</returns>
    public static long LCM(long a, long b) => a * b / GCD(a, b);

    /// <summary>
    /// Least Common Multiple function
    /// </summary>
    /// <param name="numbers">Numbers to get the LCM for</param>
    /// <returns>LCM of all the numbers in the array</returns>
    public static int LCM(params int[] numbers) => numbers.Aggregate(1, LCM);
        
    /// <summary>
    /// Least Common Multiple function
    /// </summary>
    /// <param name="numbers">Numbers to get the LCM for</param>
    /// <returns>LCM of all the numbers in the array</returns>
    public static long LCM(params long[] numbers) => numbers.Aggregate(1L, LCM);

    /// <summary>
    /// Gets the size of the object in bytes for a given primitive type
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <returns>The size in int of the primitive type</returns>
    /// <exception cref="InvalidOperationException">If the type isn't a primitive</exception>
    public static int GetSizeOfPrimitive<T>()
    {
        Type type = typeof(T);
        if (!type.IsPrimitive) throw new InvalidOperationException($"Cannot get the size of a non primitive type {typeof(T).FullName}");

        //Manual overrides
        return type == typeof(bool) ? 1 : (type == typeof(char) ? 2 : Marshal.SizeOf<T>());
            
        //Normal behaviour
    }
    #endregion
}