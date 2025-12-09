using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using AdventOfCode.Utils.Pooling;
using JetBrains.Annotations;

namespace AdventOfCode.Intcode.Output;

/// <summary>
/// Intcode output interface
/// </summary>
[PublicAPI]
public interface IOutputProvider
{
    /// <summary>
    /// How many output values the provider has to offer
    /// </summary>
    int Count { get; }

    /// <summary>
    /// If this output provider has any output
    /// </summary>
    bool IsEmpty => this.Count is 0;

    /// <summary>
    /// Receives an output value from the Intcode VM
    /// </summary>
    /// <param name="value">Outputted value</param>
    void AddOutput(long value);

    /// <summary>
    /// Gets the next output value and removes it
    /// </summary>
    /// <returns>The next output value</returns>
    long GetValue();

    /// <summary>
    /// Gets a newline terminated input line from the output
    /// </summary>
    /// <returns>The next input line</returns>
    string ReadLine()
    {
        using Pooled<StringBuilder> builder = StringBuilderObjectPool.Shared.Get();
        while (TryGetValue(out long value))
        {
            builder.Ref.Append((char)value);
            if (value is '\n') break;
        }

        string result = builder.Ref.ToString();
        return result;
    }

    /// <summary>
    /// Prints the next input line
    /// </summary>
    void PrintLine() => Trace.Write(ReadLine());

    /// <summary>
    /// Prints all input lines
    /// </summary>
    void PrintAllLines()
    {
        while (!this.IsEmpty)
        {
            PrintLine();
        }
    }

    /// <summary>
    /// Tries to get the next output value
    /// </summary>
    /// <param name="value">The output value, if found</param>
    /// <returns><see langword="true"/> if an output value was found, otherwise <see langword="false"/></returns>
    bool TryGetValue(out long value);

    /// <summary>
    /// Peeks the next output value without removing it
    /// </summary>
    /// <returns>The next output value</returns>
    long PeekValue();

    /// <summary>
    /// Tries to get the next output value without removing it
    /// </summary>
    /// <param name="value">The output value, if found</param>
    /// <returns><see langword="true"/> if an output value was found, otherwise <see langword="false"/></returns>
    bool TryPeekValue(out long value);

    /// <summary>
    /// Enumerates and removes all output values
    /// </summary>
    /// <returns>Enumerable of the output values</returns>
    IEnumerable<long> GetAllValues();

    /// <summary>
    /// Clears the output provider
    /// </summary>
    void Clear();

    /// <summary>
    /// Creates a copy of the current output provider
    /// </summary>
    /// <returns>A shallow copy of the output provider</returns>
    IOutputProvider Clone();
}
