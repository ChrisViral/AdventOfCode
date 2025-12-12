using System.ComponentModel;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 11
/// </summary>
public sealed class Day11 : GridSolver<Day11.Seat>
{
    public enum Seat
    {
        FLOOR,
        EMPTY,
        TAKEN
    }

    /// <summary>
    /// All cardinal and diagonal direction vectors
    /// </summary>
    private static readonly Vector2<int>[] Directions =
    [
        Vector2<int>.Up + Vector2<int>.Left,
        Vector2<int>.Up,
        Vector2<int>.Up + Vector2<int>.Right,
        Vector2<int>.Left,
        Vector2<int>.Right,
        Vector2<int>.Left + Vector2<int>.Down,
        Vector2<int>.Down,
        Vector2<int>.Right + Vector2<int>.Down
    ];

    /// <summary>
    /// Creates a new <see cref="Day11"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day11(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int width = this.Grid.Width;
        int height = this.Grid.Height;
        Grid<Seat> current = new(this.Grid);
        Grid<Seat> previous = new(width, height, StringConversion);
        bool changes;
        do
        {
            previous.CopyFrom(current);
            changes = false;
            foreach (int j in ..height)
            {
                foreach (int i in ..width)
                {
                    Vector2<int> position = new(i, j);
                    switch (current[position])
                    {
                        case Seat.EMPTY:
                            if (position.AsAdjacentEnumerable(withDiagonals: true).Where(p => current.WithinGrid(p)).All(p => previous[p] is not Seat.TAKEN))
                            {
                                current[position] = Seat.TAKEN;
                                changes = true;
                            }
                            break;

                        case Seat.TAKEN:
                            int taken = 0;
                            if (position.AsAdjacentEnumerable(withDiagonals: true).Where(p => current.WithinGrid(p)).Any(p => (taken += previous[p] is Seat.TAKEN ? 1 : 0) is 4))
                            {
                                current[position] = Seat.EMPTY;
                                changes = true;
                            }
                            break;
                    }
                }
            }
        }
        while (changes);
        AoCUtils.LogPart1(current.Count(s => s is Seat.TAKEN));

        current = new Grid<Seat>(this.Grid);
        do
        {
            previous.CopyFrom(current);
            changes = false;
            foreach (int j in ..height)
            {
                foreach (int i in ..width)
                {
                    Vector2<int> position = new(i, j);
                    switch (current[position])
                    {
                        case Seat.EMPTY:
                            //If everything goes well we will take the seat
                            bool anyTaken = false;
                            foreach (Vector2<int> direction in Directions)
                            {
                                Vector2<int>? moved = position;
                                do
                                {
                                    moved = previous.MoveWithinGrid(moved.Value, direction);
                                }
                                while (moved.HasValue && previous[moved.Value] is Seat.FLOOR);

                                //Found a taken seat around, do not fill seat
                                if (moved.HasValue && previous[moved.Value] is Seat.TAKEN)
                                {
                                    anyTaken = true;
                                    break;
                                }
                            }

                            if (!anyTaken)
                            {
                                current[position] = Seat.TAKEN;
                                changes = true;
                            }
                            break;

                        case Seat.TAKEN:
                            int taken = 0;
                            foreach (Vector2<int> direction in Directions)
                            {
                                Vector2<int>? moved = position;
                                do
                                {
                                    moved = previous.MoveWithinGrid(moved.Value, direction);
                                }
                                while (moved.HasValue && previous[moved.Value] is Seat.FLOOR);

                                //Found too many people around, do not keep seat
                                if (moved.HasValue && previous[moved.Value] is Seat.TAKEN && ++taken is 5)
                                {
                                    current[position] = Seat.EMPTY;
                                    changes = true;
                                    break;
                                }
                            }
                            break;
                    }
                }
            }
        }
        while (changes);
        AoCUtils.LogPart2(current.Count(s => s is Seat.TAKEN));
    }

    /// <inheritdoc cref="GridSolver{T}.LineConverter"/>
    protected override Seat[] LineConverter(string line)
    {
        Seat[] seats = new Seat[line.Length];
        foreach (int i in ..line.Length)
        {
            seats[i] = line[i] switch
            {
                '.' => Seat.FLOOR,
                'L' => Seat.EMPTY,
                '#' => Seat.TAKEN,
                _   => throw new InvalidOperationException($"Invalid seat character ({line[i]})")
            };
        }
        return seats;
    }

    /// <inheritdoc cref="GridSolver{T}.StringConversion"/>
    protected override string StringConversion(Seat seat)
    {
        return seat switch
        {
            Seat.FLOOR => ".",
            Seat.EMPTY => "L",
            Seat.TAKEN => "#",
            _          => throw new InvalidEnumArgumentException(nameof(seat), (int)seat, typeof(Seat))
        };
    }
}
