using System.Numerics;
using JetBrains.Annotations;

namespace AdventOfCode.Maths.Vectors;

/// <summary>
/// Vector base interface
/// </summary>
[PublicAPI]
public interface IVector
{
    /// <summary>
    /// Vector dimension
    /// </summary>
    static abstract int Dimension { get; }

    /// <summary>
    /// Length of the Vector
    /// </summary>
    double Length { get; }

    /// <summary>
    /// Gets the vector's dimension
    /// </summary>
    /// <returns><see cref="Dimension"/></returns>
    int GetDimension();

    /// <summary>
    /// Tries to get the component in the specified type, with a checked conversion
    /// </summary>
    /// <param name="index">Component index</param>
    /// <param name="result">Resulting value</param>
    /// <typeparam name="T">Component output type</typeparam>
    /// <returns><see langword="true"/> if the componentn was successfully converted, otherwise <see langword="false"/></returns>
    bool TryGetComponentChecked<T>(int index, out T result) where T : unmanaged, IBinaryNumber<T>, IMinMaxValue<T>;

    /// <summary>
    /// Tries to get the component in the specified type, with a saturating conversion
    /// </summary>
    /// <param name="index">Component index</param>
    /// <param name="result">Resulting value</param>
    /// <typeparam name="T">Component output type</typeparam>
    /// <returns><see langword="true"/> if the componentn was successfully converted, otherwise <see langword="false"/></returns>
    bool TryGetComponentSaturating<T>(int index, out T result) where T : unmanaged, IBinaryNumber<T>, IMinMaxValue<T>;

    /// <summary>
    /// Tries to get the component in the specified type, with a truncating conversion
    /// </summary>
    /// <param name="index">Component index</param>
    /// <param name="result">Resulting value</param>
    /// <typeparam name="T">Component output type</typeparam>
    /// <returns><see langword="true"/> if the componentn was successfully converted, otherwise <see langword="false"/></returns>
    bool TryGetComponentTruncating<T>(int index, out T result) where T : unmanaged, IBinaryNumber<T>, IMinMaxValue<T>;

    /// <summary>
    /// Tries to convert the given components to a vector of the given type, using a checked conversion
    /// </summary>
    /// <param name="components">Components to convert</param>
    /// <param name="result">Output vector if successful</param>
    /// <typeparam name="T">Component input type</typeparam>
    /// <returns><see langword="true"/> if the components were made into a vector, otherwise <see langword="false"/></returns>
    bool TryMakeFromComponentsChecked<T>(ReadOnlySpan<T> components, out IVector? result)
        where T : unmanaged, IBinaryNumber<T>, IMinMaxValue<T>;

    /// <summary>
    /// Tries to convert the given components to a vector of the given type, using a saturating conversion
    /// </summary>
    /// <param name="components">Components to convert</param>
    /// <param name="result">Output vector if successful</param>
    /// <typeparam name="T">Component input type</typeparam>
    /// <returns><see langword="true"/> if the components were made into a vector, otherwise <see langword="false"/></returns>
    bool TryMakeFromComponentsSaturating<T>(ReadOnlySpan<T> components, out IVector? result)
        where T : unmanaged, IBinaryNumber<T>, IMinMaxValue<T>;

    /// <summary>
    /// Tries to convert the given components to a vector of the given type, using a truncating conversion
    /// </summary>
    /// <param name="components">Components to convert</param>
    /// <param name="result">Output vector if successful</param>
    /// <typeparam name="T">Component input type</typeparam>
    /// <returns><see langword="true"/> if the components were made into a vector, otherwise <see langword="false"/></returns>
    bool TryMakeFromComponentsTruncating<T>(ReadOnlySpan<T> components, out IVector? result)
        where T : unmanaged, IBinaryNumber<T>, IMinMaxValue<T>;
}

/// <summary>
/// Vector interface
/// </summary>
/// <typeparam name="T">Vector component type</typeparam>
[PublicAPI]
public interface IVector<out T> : IVector
    where T : unmanaged, IBinaryNumber<T>, IMinMaxValue<T>
{
    /// <summary>
    /// Gets the component at the given index
    /// </summary>
    /// <param name="index">Component's index</param>
    T this[int index] { get; }

    /// <summary>
    /// Gets the component at the given index
    /// </summary>
    /// <param name="index">Component's index</param>
    T this[Index index] { get; }

    /// <summary>
    /// Absolute length of both vector components summed
    /// </summary>
    T ManhattanLength { get; }
}

/// <summary>
/// Vector interface
/// </summary>
/// <typeparam name="TComponent">Vector component type</typeparam>
/// <typeparam name="TSelf">Vector self type</typeparam>
[PublicAPI]
public interface IVector<TSelf, out TComponent> : IVector<TComponent>, INumber<TSelf>, IMinMaxValue<TSelf>
    where TSelf : IVector<TSelf, TComponent>
    where TComponent : unmanaged, IBinaryNumber<TComponent>, IMinMaxValue<TComponent>
{
    /// <summary>
    /// Calculates the distance between two vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The distance between both vectors</returns>
    static abstract double Distance(TSelf a, TSelf b);

    /// <summary>
    /// The Manhattan distance between both vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>Tge straight line distance between both vectors</returns>
    static abstract TComponent ManhattanDistance(TSelf a, TSelf b);

    /// <summary>
    /// Calculates the cross product of both vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The cross product of both vectors</returns>
    static abstract TComponent Dot(TSelf a, TSelf b);
}

/// <summary>
/// Vector cross product operator interface
/// </summary>
/// <typeparam name="TVector">Vector type</typeparam>
/// <typeparam name="TComponent">Vector component type</typeparam>
/// <typeparam name="TResult">Cross product result type</typeparam>
[PublicAPI]
public interface ICrossProductOperator<in TVector, TComponent, out TResult>
    where TVector : IVector<TVector, TComponent>
    where TComponent : unmanaged, IBinaryNumber<TComponent>, IMinMaxValue<TComponent>
    where TResult : INumber<TResult>
{
    /// <summary>
    /// Calculates the cross product of both vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The cross product of both vectors</returns>
    static abstract TResult Cross(TVector a, TVector b);
}
