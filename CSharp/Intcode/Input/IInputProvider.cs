using System.Collections.Generic;

namespace AdventOfCode.Intcode.Input;

/// <summary>
/// Intcode input provider interface
/// </summary>
public interface IInputProvider
{
    /// <summary>
    /// How many input values the provider has to offer
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Provides an input value to the Intcode VM
    /// </summary>
    /// <returns>The next provided Input value</returns>
    long Input();

    /// <summary>
    /// Adds the given value to the input
    /// </summary>
    /// <param name="value"></param>
    void AddInput(long value);

    /// <summary>
    /// Adds a set of values to the input
    /// </summary>
    /// <param name="values">Values to add</param>
    void FillInput(IEnumerable<long> values);
}