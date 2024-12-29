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
}