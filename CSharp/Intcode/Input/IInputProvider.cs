using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AdventOfCode.Intcode.Input;

/// <summary>
/// Intcode input provider interface
/// </summary>
[PublicAPI]
public interface IInputProvider
{
    /// <summary>
    /// How many input values the provider has to offer
    /// </summary>
    int Count { get; }

    /// <summary>
    /// If this input provider has any input
    /// </summary>
    bool IsEmpty => this.Count is 0;

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
    /// Adds a given string line to the input
    /// </summary>
    /// <param name="line">Line to add</param>
    void WriteLine(params ReadOnlySpan<char> line)
    {
        foreach (char c in line)
        {
            AddInput(c);
        }
        AddInput('\n');
    }

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
