using JetBrains.Annotations;

namespace AdventOfCode.Utils;

/// <summary>
/// By-Ref struct wrapper
/// </summary>
/// <param name="value">Struct value</param>
/// <typeparam name="T">Struct type</typeparam>
[PublicAPI]
public class Ref<T>(T value) where T : struct
{
    /// <summary>
    /// Wrapped value
    /// </summary>
    public T Value { get; set; } = value;

    /// <summary>
    /// Unwraps the struct to it's original value
    /// </summary>
    /// <param name="value">By-ref struct value</param>
    /// <returns>A copy of the struct's value</returns>
    public static implicit operator T(Ref<T> value) => value.Value;

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => this.Value.ToString()!;
}