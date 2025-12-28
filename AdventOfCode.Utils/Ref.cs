using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AdventOfCode.Utils;

/// <summary>
/// By-Ref struct wrapper
/// </summary>
/// <param name="value">Struct value</param>
/// <typeparam name="T">Struct type</typeparam>
[PublicAPI]
public sealed class Ref<T>(T value) where T : struct
{
    // ReSharper disable once ReplaceWithFieldKeyword
    private T value = value;
    /// <summary>
    /// Wrapped value
    /// </summary>
    public ref T Value => ref this.value;

    /// <summary>
    /// By-Ref struct wrapper
    /// </summary>
    public Ref() : this(default) { }

    /// <inheritdoc cref="object.ToString"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => this.Value.ToString()!;

    /// <summary>
    /// Unwraps the struct to it's original value
    /// </summary>
    /// <param name="value">By-ref struct value</param>
    /// <returns>A copy of the struct's value</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T(Ref<T> value) => value.Value;
}
