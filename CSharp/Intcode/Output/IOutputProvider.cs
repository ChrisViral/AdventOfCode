using System;
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
    bool HasOutput => this.Count > 0;

    /// <summary>
    /// Receives an output value from the Intcode VM
    /// </summary>
    /// <param name="value">Outputted value</param>
    void Output(long value);

    /// <summary>
    /// Gets the next output value
    /// </summary>
    /// <returns>the next output value</returns>
    long GetOutput();

    /// <summary>
    /// Enumerates all output values
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