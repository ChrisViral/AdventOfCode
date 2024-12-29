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
    /// <param name="input">Returned input</param>
    /// <returns><see langword="true"/> if an input was returned, otherwise <see langword="false"/></returns>
    bool TryGetInput(out long input);

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

    /// <summary>
    /// Clears the input provider
    /// </summary>
    void Clear();

    /// <summary>
    /// Creates a copy of the current input provider
    /// </summary>
    /// <returns>A shallow copy of the input provider</returns>
    IInputProvider Clone();
}