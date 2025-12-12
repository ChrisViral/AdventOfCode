using System.Numerics;
using JetBrains.Annotations;

namespace AdventOfCode.Utils;

/// <summary>
/// Float number utils
/// </summary>
/// <typeparam name="T">Floating point number type</typeparam>
[PublicAPI]
public static class FloatUtils<T> where T : IFloatingPoint<T>
{
    /// <summary>
    /// Small value of <typeparamref name="T"/> equivalent to <c>1E-5</c> for use when doing equality tests on floating point numbers
    /// </summary>
    public static T Epsilon { get; } = T.CreateChecked(1E-5);
}
