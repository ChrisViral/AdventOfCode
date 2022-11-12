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
    private static readonly long microsecondsPerTick  = Stopwatch.Frequency / 1000000L;
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
                elapsed = $"{watch.ElapsedTicks / microsecondsPerTick}μs";
                break;

            case < 10L:
                (long ms, long remaining) = Math.DivRem(watch.ElapsedTicks, millisecondsPerTick);
                long us = remaining / microsecondsPerTick;
                elapsed = $"{ms}ms {us}μs";
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
    /// Swaps two values in memory
    /// </summary>
    /// <typeparam name="T">Type of value to swap</typeparam>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    public static void Swap<T>(ref T a, ref T b) => (a, b) = (b, a);
    #endregion
}