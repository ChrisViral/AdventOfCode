using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AdventOfCode.Utils;

/// <summary>
/// General Advent of Code utility methods
/// </summary>
public static class AoCUtils
{
    private static readonly long nanosecondsPerTick  = Stopwatch.Frequency / 1000000L;
    private static readonly long millisecondsPerTick = Stopwatch.Frequency / 1000L;

    #region Static methods
    /// <summary>
    /// Combines input lines into sequences, separated by empty lines
    /// </summary>
    /// <param name="input">Input lines</param>
    /// <returns>An enumerable of the packed input</returns>
    public static IEnumerable<List<string>> CombineLines(IEnumerable<string> input)
    {
        List<string> pack = new();
        foreach (string line in input)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (pack.Count is 0) continue;

                yield return pack;
                pack = new();
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
    /// Logs a message to the console and the log file
    /// </summary>
    /// <param name="message">Message to log</param>
    public static void Log(object message) => Trace.WriteLine(message);

    /// <summary>
    /// Logs the elapsed time on the stopwatch
    /// </summary>
    /// <param name="watch">Stopwatch to log the time for</param>
    public static void LogElapsed(Stopwatch watch)
    {
        string elapsed;
        switch (watch.ElapsedMilliseconds)
        {
            case <= 1L:
                elapsed = $"{watch.ElapsedTicks / nanosecondsPerTick}ns";
                break;

            case < 10L:
                (long ms, long remaining) = Math.DivRem(watch.ElapsedTicks, millisecondsPerTick);
                long ns = remaining / nanosecondsPerTick;
                elapsed = $"{ms}ms {ns}ns";
                break;

            case < 1000L:
                elapsed = $"{watch.ElapsedMilliseconds}ms";
                break;

            default:
                elapsed = $"{watch.Elapsed.Seconds}s {watch.Elapsed.Milliseconds}ms";
                break;
        }

        Trace.WriteLine($"Elapsed time: {elapsed}");
    }

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
                    (working[k], working[i]) = (working[i], working[k]);
                    foreach (T[] perm in GetPermutations(working, k + 1, size))
                    {
                        yield return perm;
                    }
                    (working[k], working[i]) = (working[i], working[k]);
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
    public static void Swap<T>(ref T a, ref T b) => (a, b) = (b, a);

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

    /// <summary>
    /// Enumerates all the elements in the passed enumerable, along with the enumeration index
    /// </summary>
    /// <typeparam name="T">Type of element to enumerate</typeparam>
    /// <param name="enumerable">Enumerable</param>
    /// <returns></returns>
    public static IEnumerable<(int index, T value)> Enumerate<T>(IEnumerable<T> enumerable)
    {
        int index = 0;
        foreach (T value in enumerable)
        {
            yield return (index++, value);
        }
    }
    #endregion
}