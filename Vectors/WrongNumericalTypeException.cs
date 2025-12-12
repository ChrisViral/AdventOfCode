using System;
using JetBrains.Annotations;

namespace AdventOfCode.Vectors;

[PublicAPI]
public sealed class WrongNumericalTypeException : Exception
{
    public enum NumericalType
    {
        INTEGER,
        FLOATING
    }

    /// <summary>
    /// Creates a new WrongNumericalType exception
    /// </summary>
    public WrongNumericalTypeException() { }

    /// <summary>
    /// Creates a new WrongNumericalType exception with the given message
    /// </summary>
    /// <param name="message">Exception message</param>
    public WrongNumericalTypeException(string message) : base(message) { }

    /// <summary>
    /// Creates a new WrongNumericalType exception with the given message and inner exception
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public WrongNumericalTypeException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Creates a new WrongNumericalType exception with a generated message based on the expected type
    /// </summary>
    /// <param name="expected">Expected numerical type</param>
    /// <param name="type">Type that was found instead</param>
    public WrongNumericalTypeException(NumericalType expected, Type type)
        : base($"Expected {expected.ToString().ToLower()} number, got {type.Name} instead") { }

    /// <summary>
    /// Creates a new WrongNumericalType exception with a generated message based on the expected type and inner exception
    /// </summary>
    /// <param name="expected">Expected numerical type</param>
    /// <param name="type">Type that was found instead</param>
    /// <param name="innerException">Inner exception</param>
    public WrongNumericalTypeException(NumericalType expected, Type type, Exception innerException)
        : base($"Expected {expected.ToString().ToLower()} number, got {type.Name} instead", innerException) { }
}
