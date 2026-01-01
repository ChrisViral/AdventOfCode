using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using AdventOfCode.Utils.Extensions.Strings;

namespace AdventOfCode.Utils;

/// <summary>
/// Register reference value, supports literalls or letter registers
/// </summary>
/// <param name="Value">Integer value</param>
/// <param name="IsRegister">Wether or not the value points to a register or is an immediate value</param>
public readonly record struct RegisterRef<T>(T Value, bool IsRegister) : IParsable<RegisterRef<T>>
    where T :IBinaryInteger<T>
{
    /// <summary>
    /// Gets the value for this reference
    /// </summary>
    /// <param name="registers">Program registers</param>
    /// <returns>The correct value this reference points to</returns>
    public T GetValue(ReadOnlySpan<T> registers) => this.IsRegister
                                                        ? registers[int.CreateChecked(this.Value)]
                                                        : this.Value;

    /// <summary>
    /// Gets the register value as a ref for this reference
    /// </summary>
    /// <param name="registers">Program registers</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">If this reference is not pointing to registers</exception>
    public ref T GetRegister(Span<T> registers)
    {
        if (!this.IsRegister) throw new InvalidOperationException("Cannot get register value for non-register ref");

        return ref registers[int.CreateChecked(this.Value)];
    }

    /// <inheritdoc />
    public static RegisterRef<T> Parse(string s, IFormatProvider? provider) => T.TryParse(s, NumberStyles.Integer, null, out T? value)
                                                                        ? new RegisterRef<T>(value, false)
                                                                        : new RegisterRef<T>(T.CreateChecked(s[0].AsIndex), true);

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out RegisterRef<T> result)
    {
        if (string.IsNullOrEmpty(s))
        {
            result = default;
            return false;
        }

        result = Parse(s, provider);
        return false;
    }

    /// <inheritdoc />
    public override string ToString() => this.IsRegister
                                             ? this.Value.AsAsciiLower.ToString()
                                             : this.Value.ToString()!;
}
