using System;
using AdventOfCode.Grids.Vectors;

namespace AdventOfCode.Grids
{
    [Flags]
    public enum Direction
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

    public static class DirectionExtensions
    {
        public static Vector2 ToVector(this Direction direction)
        {
            switch (direction)
            {
                case Direction.NONE:
                case Direction.VERTICAL:
                case Direction.HORIZONTAL:
                case Direction.ALL:
                    return Vector2.Zero;
                
                case Direction.UP:
                    return Vector2.Up;
                
                case Direction.DOWN:
                    return Vector2.Down;
                
                case Direction.LEFT:
                    return Vector2.Left;
                
                case Direction.RIGHT:
                    return Vector2.Right;
                
                default:
                    int x = (direction & Direction.HORIZONTAL) switch
                    {
                        Direction.LEFT  => Vector2.Left.X,
                        Direction.RIGHT => Vector2.Right.X,
                        _               => 0
                    };
                    int y = (direction & Direction.VERTICAL) switch
                    {
                        Direction.UP   => Vector2.Up.Y,
                        Direction.DOWN => Vector2.Down.Y,
                        _              => 0
                    };
                    return new Vector2(x, y);
            }
        }
    }
}