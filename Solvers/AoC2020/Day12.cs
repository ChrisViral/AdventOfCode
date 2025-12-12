using System;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 12
/// </summary>
public sealed partial class Day12 : Solver<Day12.Navigation[]>
{
    /// <summary>
    /// Navigation instruction object
    /// </summary>
    public sealed partial record Navigation
    {
        /// <summary>
        /// Navigation instructions
        /// </summary>
        public enum Instructions
        {
            NORTH,
            SOUTH,
            EAST,
            WEST,
            LEFT,
            RIGHT,
            FORWARD
        }

        /// <summary>
        /// Navigation match pattern
        /// </summary>
        [GeneratedRegex(@"(N|S|E|W|L|R|F)(\d+)")]
        public static partial Regex Matcher { get; }

        /// <summary>Navigation instruction</summary>
        public Instructions Instruction { get; }

        /// <summary>Instruction value</summary>
        public int Value { get; }

        /// <summary>
        /// Creates a new navigation object with the specified instruction
        /// </summary>
        /// <param name="instruction">Instruction to create the object from</param>
        /// <param name="value">Value of the instruction</param>
        public Navigation(char instruction, int value)
        {
            this.Instruction = instruction switch
            {
                'N' => Instructions.NORTH,
                'S' => Instructions.SOUTH,
                'E' => Instructions.EAST,
                'W' => Instructions.WEST,
                'L' => Instructions.LEFT,
                'R' => Instructions.RIGHT,
                'F' => Instructions.FORWARD,
                _   => throw new ArgumentException("Invalid instruction ({instruction}) could not be parsed", nameof(instruction))
            };

            this.Value = value;
        }

        /// <summary>
        /// Executes the instruction as ship directions commands
        /// </summary>
        /// <param name="position">Position of the ship</param>
        /// <param name="direction">Direction of the ship</param>
        public void Execute(ref Vector2<int> position, ref Vector2<int> direction)
        {
            switch (this.Instruction)
            {
                case Instructions.NORTH:
                    position += Vector2<int>.Up * this.Value;
                    return;
                case Instructions.SOUTH:
                    position += Vector2<int>.Down * this.Value;
                    return;
                case Instructions.EAST:
                    position += Vector2<int>.Right * this.Value;
                    return;
                case Instructions.WEST:
                    position += Vector2<int>.Left * this.Value;
                    return;
                case Instructions.LEFT:
                    direction = Vector2<int>.Rotate(direction, -this.Value);
                    return;
                case Instructions.RIGHT:
                    direction = Vector2<int>.Rotate(direction, this.Value);
                    return;
                case Instructions.FORWARD:
                    position += direction * this.Value;
                    return;
            }
        }

        /// <summary>
        /// Executes the instruction as ship waypoint commands
        /// </summary>
        /// <param name="position">Position of the ship</param>
        /// <param name="waypoint">Position of the waypoint around the ship</param>
        public void ExecuteWaypoint(ref Vector2<int> position, ref Vector2<int> waypoint)
        {
            switch (this.Instruction)
            {
                case Instructions.NORTH:
                    waypoint += Vector2<int>.Up * this.Value;
                    return;
                case Instructions.SOUTH:
                    waypoint += Vector2<int>.Down * this.Value;
                    return;
                case Instructions.EAST:
                    waypoint += Vector2<int>.Right * this.Value;
                    return;
                case Instructions.WEST:
                    waypoint += Vector2<int>.Left * this.Value;
                    return;
                case Instructions.LEFT:
                    waypoint = Vector2<int>.Rotate(waypoint, -this.Value);
                    return;
                case Instructions.RIGHT:
                    waypoint = Vector2<int>.Rotate(waypoint, this.Value);
                    return;
                case Instructions.FORWARD:
                    position += waypoint * this.Value;
                    return;
            }
        }
    }

    /// <summary>
    /// Creates a new <see cref="Day12"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Navigation"/>[] fails</exception>
    public Day12(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Vector2<int> position = Vector2<int>.Zero;
        Vector2<int> direction = Vector2<int>.Right;
        this.Data.ForEach(i => i.Execute(ref position, ref direction));
        AoCUtils.LogPart1(Math.Abs(position.X) + Math.Abs(position.Y));

        Vector2<int> ship = Vector2<int>.Zero;
        Vector2<int> waypoint = (Vector2<int>.Right * 10) + Vector2<int>.Up;
        this.Data.ForEach(i => i.ExecuteWaypoint(ref ship, ref waypoint));
        AoCUtils.LogPart2(Math.Abs(ship.X) + Math.Abs(ship.Y));
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Navigation[] Convert(string[] rawInput) => RegexFactory<Navigation>.ConstructObjects(Navigation.Matcher, rawInput);
}
