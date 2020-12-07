using System;
using AdventOfCode.Grids.Vectors;

namespace AdventOfCode.Grids
{
    /// <summary>
    /// Cardinal Directions
    /// </summary>
    [Flags]
    public enum Directions
    {
        NONE       = 0b0000,
        UP         = 0b0001,
        DOWN       = 0b0010,
        LEFT       = 0b0100,
        RIGHT      = 0b1000,
        VERTICAL   = UP | DOWN,
        HORIZONTAL = LEFT | RIGHT,
        ALL        = VERTICAL | HORIZONTAL
    }

    /// <summary>
    /// Directions extension methods
    /// </summary>
    public static class DirectionsExtensions
    {
        #region Extension methods
        public static Vector2 ToVector(this Directions directions)
        {
            switch (directions)
            {
                case Directions.NONE:
                case Directions.VERTICAL:
                case Directions.HORIZONTAL:
                case Directions.ALL:
                    return Vector2.Zero;
                
                case Directions.UP:
                    return Vector2.Up;
                
                case Directions.DOWN:
                    return Vector2.Down;
                
                case Directions.LEFT:
                    return Vector2.Left;
                
                case Directions.RIGHT:
                    return Vector2.Right;
                
                default:
                    int x = (directions & Directions.HORIZONTAL) switch
                    {
                        Directions.LEFT  => Vector2.Left.X,
                        Directions.RIGHT => Vector2.Right.X,
                        _               => 0
                    };
                    int y = (directions & Directions.VERTICAL) switch
                    {
                        Directions.UP   => Vector2.Up.Y,
                        Directions.DOWN => Vector2.Down.Y,
                        _              => 0
                    };
                    return new Vector2(x, y);
            }
        }
        #endregion
    }
}