using System;
using System.Numerics;
using JetBrains.Annotations;

namespace AdventOfCode.Vectors;

/// <summary>
/// Vector interface
/// </summary>
/// <typeparam name="TComponent">Vector component type</typeparam>
/// <typeparam name="TSelf">Vector self type</typeparam>
[PublicAPI]
public interface IVector<out TComponent, TSelf> : INumber<TSelf>, IMinMaxValue<TSelf>
    where TSelf : IVector<TComponent, TSelf>
    where TComponent : IBinaryNumber<TComponent>, IMinMaxValue<TComponent>
{
    static abstract int Dimension { get; }

    TComponent this[int index] { get; }

    TComponent this[Index index] { get; }

    double Length { get; }

    TComponent ManhattanLength { get; }

    static abstract double Distance(TSelf a, TSelf b);

    static abstract TComponent ManhattanDistance(TSelf a, TSelf b);

    static abstract TComponent Dot(TSelf a, TSelf b);
}
