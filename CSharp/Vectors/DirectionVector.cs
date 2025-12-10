using System.Numerics;

namespace AdventOfCode.Vectors;

/// <summary>
/// Component-wise direction based vector
/// </summary>
/// <param name="X">X component</param>
/// <param name="Y">Y component</param>
/// <typeparam name="T">Integer type</typeparam>
public readonly record struct DirectionVector<T>((Direction direction, T length) X, (Direction direction, T length) Y) where T : IBinaryInteger<T>, IMinMaxValue<T>
{
    /// <summary>
    /// Converts a vector to a direction vector
    /// </summary>
    /// <param name="vector">Vector to convert</param>
    public DirectionVector(Vector2<T> vector) : this((Direction.NONE, T.Zero), (Direction.NONE, T.Zero))
    {
        switch (T.Sign(vector.X))
        {
            case < 0:
                X = (Direction.LEFT, T.Abs(vector.X));
                break;

            case > 0:
                X = (Direction.RIGHT, T.Abs(vector.X));
                break;
        }

        switch (T.Sign(vector.Y))
        {
            case < 0:
                Y = (Direction.UP, T.Abs(vector.Y));
                break;

            case > 0:
                Y = (Direction.DOWN, T.Abs(vector.Y));
                break;
        }
    }
}

/// <summary>
/// <see cref="DirectionVector{T}"/> extensions
/// </summary>
public static class DirectionVectorExtensions
{
    /// <typeparam name="T">Integer type</typeparam>
    extension<T>(Vector2<T> vector) where T : IBinaryInteger<T>, IMinMaxValue<T>
    {
        /// <summary>
        /// Converts a vector to a direction vector
        /// </summary>
        /// <returns>The converted <see cref="DirectionVector{T}"/></returns>
        public DirectionVector<T> ToDirectionVector() => new(vector);
    }
}
