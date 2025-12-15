using System.Collections.Immutable;
using System.ComponentModel;
using AdventOfCode.Collections;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 13
/// </summary>
public sealed class Day13 : Solver<(Grid<Day13.Track> tracks, List<Day13.Cart> carts)>
{
    public enum Track
    {
        NONE         = 0,
        EMPTY        = ' ',
        VERTICAL     = '|',
        HORIZONTAL   = '-',
        INTERSECTION = '+',
        CORNER_RIGHT = '/',
        CORNER_LEFT  = '\\'
    }

    public sealed class Cart(Vector2<int> position, Direction direction) : IComparable<Cart>
    {
        private static readonly ImmutableArray<Direction> IntersectionTurns = [Direction.LEFT, Direction.NONE, Direction.RIGHT];

        private int intersectionIndex;
        private Direction direction = direction;

        public Vector2<int> Position { get; private set; } = position;

        public void Update(Grid<Track> tracks)
        {
            this.Position += this.direction;
            Track track = tracks[this.Position];
            switch (track)
            {
                // Straight tracks
                case Track.HORIZONTAL:
                case Track.VERTICAL:
                    break;

                // Right corners
                case Track.CORNER_RIGHT:
                    this.direction = this.direction.IsVertical() ? this.direction.TurnRight() : this.direction.TurnLeft();
                    break;

                // Left corners
                case Track.CORNER_LEFT:
                    this.direction = this.direction.IsVertical() ? this.direction.TurnLeft() : this.direction.TurnRight();
                    break;

                //Intersection
                case Track.INTERSECTION:
                    this.direction = this.direction.TurnBy(IntersectionTurns[this.intersectionIndex++]);
                    this.intersectionIndex %= IntersectionTurns.Length;
                    break;

                // Off track
                case Track.NONE:
                case Track.EMPTY:
                    throw new InvalidOperationException("Cart has gone off track");

                // Invalid
                default:
                    throw new InvalidEnumArgumentException(nameof(track), (int)track, typeof(Track));
            }
        }

        /// <inheritdoc />
        public int CompareTo(Cart? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            int comp = this.Position.X.CompareTo(other.Position.X);
            return comp is 0 ? this.Position.Y.CompareTo(other.Position.Y) : comp;
        }
    }

    /// <summary>
    /// Creates a new <see cref="Day13"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day13(string input) : base(input, options: StringSplitOptions.RemoveEmptyEntries) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Vector2<int>? firstCrash;
        do
        {
            UpdateCarts(out firstCrash);
        }
        while (!firstCrash.HasValue);
        AoCUtils.LogPart1($"{firstCrash.Value.X},{firstCrash.Value.Y}");

        do
        {
            UpdateCarts(out _);
        }
        while (this.Data.carts.Count > 1);

        Vector2<int> lastCart = this.Data.carts[0].Position;
        AoCUtils.LogPart2($"{lastCart.X},{lastCart.Y}");
    }

    // ReSharper disable once CognitiveComplexity
    private void UpdateCarts(out Vector2<int>? crash)
    {
        this.Data.carts.Sort();
        crash = null;
        for (int i = 0; i < this.Data.carts.Count; i++)
        {
            Cart cart = this.Data.carts[i];
            cart.Update(this.Data.tracks);
            for (int j = 0; j < this.Data.carts.Count; j++)
            {
                if (i == j) continue;

                Cart otherCart = this.Data.carts[j];
                if (cart.Position == otherCart.Position)
                {
                    crash = cart.Position;
                    if (i < j)
                    {
                        this.Data.carts.RemoveAt(j);
                        this.Data.carts.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        this.Data.carts.RemoveAt(i);
                        this.Data.carts.RemoveAt(j);
                        i -= 2;
                    }
                    break;
                }
            }
        }
    }

    /// <inheritdoc />
    protected override (Grid<Track>, List<Cart>) Convert(string[] rawInput)
    {
        List<Cart> carts = [];
        Grid<Track> tracks = new(rawInput[0].Length, rawInput.Length, t => new string((char)t, 1));
        foreach (Vector2<int> position in tracks.Dimensions.Enumerate())
        {
            char track = rawInput[position.Y][position.X];
            switch (track)
            {
                case '<' or '>':
                    tracks[position] = Track.HORIZONTAL;
                    carts.Add(new Cart(position, Direction.Parse(track)));
                    break;

                case '^' or 'v':
                    tracks[position] = Track.VERTICAL;
                    carts.Add(new Cart(position, Direction.Parse(track)));
                    break;

                default:
                    tracks[position] = (Track)track;
                    break;
            }
        }

        return (tracks, carts);
    }
}
