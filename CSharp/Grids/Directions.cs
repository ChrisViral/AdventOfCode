using System.Collections.Generic;
using System.Collections.ObjectModel;
using AdventOfCode.Grids.Vectors;

namespace AdventOfCode.Grids
{
    /// <summary>
    /// Cardinal Directions
    /// </summary>
    public enum Directions
    {
        NONE  = 0,
        UP    = 1,
        DOWN  = 2,
        LEFT  = 3,
        RIGHT = 4,
        NORTH = UP,
        SOUTH = DOWN,
        EAST  = LEFT,
        WEST  = RIGHT
    }

    /// <summary>
    /// Directions extension methods
    /// </summary>
    public static class DirectionsUtils
    {
        #region Static properties
        /// <summary>
        /// All possible directions
        /// </summary>
        public static ReadOnlyCollection<Directions> AllDirections { get; } = new(new[] { Directions.UP, Directions.DOWN, Directions.LEFT, Directions.RIGHT });
        #endregion
        
        #region Extension methods
        /// <summary>
        /// Gets a Vector2 from a given Directions
        /// </summary>
        /// <param name="directions">Direction to get the vector from</param>
        /// <returns>The resulting vector</returns>
        public static Vector2 ToVector(this Directions directions)
        {
            return directions switch
            {
                Directions.UP    => Vector2.Up,
                Directions.DOWN  => Vector2.Down,
                Directions.LEFT  => Vector2.Left,
                Directions.RIGHT => Vector2.Right,
                _                => Vector2.Zero,
            };
        }

        /// <summary>
        /// Inverts the direction
        /// </summary>
        /// <param name="directions">Direction to invert</param>
        /// <returns>Reverse direction from the current one</returns>
        public static Directions Invert(this Directions directions)
        {
            return directions switch
            {
                Directions.UP    => Directions.DOWN,
                Directions.DOWN  => Directions.UP,
                Directions.LEFT  => Directions.RIGHT,
                Directions.RIGHT => Directions.LEFT,
                _                => directions
            };
        }
        #endregion
    }
}