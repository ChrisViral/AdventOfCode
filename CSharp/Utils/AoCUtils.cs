using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using TextCopy;

namespace AdventOfCode.Utils;

/// <summary>
/// General Advent of Code utility methods
/// </summary>
[PublicAPI]
public static class AoCUtils
{
    /// <summary>
    /// The Stopwatch for individual parts
    /// </summary>
    public static Stopwatch PartsWatch { get; } = new();

    #region Static methods
    /// <summary>
    /// Combines input lines into sequences, separated by empty lines
    /// </summary>
    /// <param name="input">Input lines</param>
    /// <returns>An enumerable of the packed input</returns>
    public static IEnumerable<List<string>> CombineLines([InstantHandle] IEnumerable<string> input)
    {
        List<string> pack = [];
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
    /// Logs the answer to Part 1 to the console and results file.<br/>
    /// This also adds the answer to the clipboard.
    /// </summary>
    /// <param name="answer">Answer to log</param>
    public static void LogPart1<T>(T answer) where T : notnull
    {
        PartsWatch.Stop();
        string text = answer.ToString() ?? string.Empty;
        if (!string.IsNullOrEmpty(text))
        {
            ClipboardService.SetText(text);
        }

        Trace.WriteLine($"Part 1: {text}\nin {GetElapsedString(PartsWatch.Elapsed)}\n");
        PartsWatch.Restart();
    }

    /// <summary>
    /// Logs the answer to Part 2 to the console and results file<br/>
    /// This also adds the answer to the clipboard.
    /// </summary>
    /// <param name="answer">Answer to log</param>
    public static void LogPart2<T>(T answer) where T : notnull
    {
        PartsWatch.Stop();
        string text = answer.ToString() ?? string.Empty;
        if (!string.IsNullOrEmpty(text))
        {
            ClipboardService.SetText(text);
        }
        Trace.WriteLine($"Part 2: {text}\nin {GetElapsedString(PartsWatch.Elapsed)}\n");
    }

    /// <summary>
    /// Logs a message to the console and the log file
    /// </summary>
    /// <param name="message">Message to log</param>
    public static void Log<T>(T message) where T : notnull => Trace.WriteLine(message);

    /// <summary>
    /// Logs the elapsed time on the stopwatch
    /// </summary>
    /// <param name="watch">Stopwatch to log the time for</param>
    public static void LogElapsed(Stopwatch watch) => Trace.WriteLine($"Total elapsed time: {GetElapsedString(watch.Elapsed)}\n");

    /// <summary>
    /// Produces a interval-based formatted time string
    /// </summary>
    /// <param name="timespan">Timespan to get the elapsed time string for</param>
    /// <returns>An elapsed time string whose units are based on the total elapsed duration</returns>
    public static string GetElapsedString(in TimeSpan timespan) => timespan switch
    {
        { TotalNanoseconds:  <  1000d } => $"{timespan.Nanoseconds}ns",
        { TotalMicroseconds: <  10d }   => $"{timespan.Microseconds}µs {timespan.Nanoseconds}ns",
        { TotalMicroseconds: <= 1000d } => $"{timespan.Microseconds}μs",
        { TotalMilliseconds: <  10d }   => $"{timespan.Milliseconds}ms {timespan.Microseconds}μs",
        { TotalMilliseconds: <= 1000d } => $"{timespan.Milliseconds}ms",
        { TotalSeconds:      <  10d }   => $"{timespan.Seconds}s {timespan.Milliseconds}ms",
        { TotalSeconds:      <= 60d }   => $"{timespan.Seconds}s",
        _                               => $"{timespan.Minutes}m {timespan.Seconds}s"
    };

    /// <summary>
    /// Swaps two values in memory
    /// </summary>
    /// <typeparam name="T">Type of value to swap</typeparam>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    public static void Swap<T>(ref T a, ref T b) => (a, b) = (b, a);

    /// <summary>
    /// Swaps two spans in memory
    /// </summary>
    /// <typeparam name="T">Type of element within the span</typeparam>
    /// <param name="a">First span</param>
    /// <param name="b">Second span</param>
    public static void SwapSpans<T>(ref Span<T> a, ref Span<T> b)
    {
        Span<T> temp = a;
        a = b;
        b = temp;
    }
    #endregion
}