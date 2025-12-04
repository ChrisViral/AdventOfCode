using System.Collections.Generic;

namespace AdventOfCode.Intcode.Output;

/// <summary>
/// Intcode output interface
/// </summary>
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
    long GetOutput();

    /// <summary>
    /// Tries to get the next output value
    /// </summary>
    /// <param name="value">The output value, if found</param>
    /// <returns><see langword="true"/> if an output value was found, otherwise <see langword="false"/></returns>
    bool TryGetOutput(out long value);

    /// <summary>
    /// Peeks the next output value without removing it
    /// </summary>
    /// <returns>The next output value</returns>
    long PeekOutput();

    /// <summary>
    /// Tries to get the next output value without removing it
    /// </summary>
    /// <param name="value">The output value, if found</param>
    /// <returns><see langword="true"/> if an output value was found, otherwise <see langword="false"/></returns>
    bool TryPeekOutput(out long value);

    /// <summary>
    /// Enumerates and removes all output values
    /// </summary>
    /// <returns>Enumerable of the output values</returns>
    IEnumerable<long> GetAllOutput();

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
