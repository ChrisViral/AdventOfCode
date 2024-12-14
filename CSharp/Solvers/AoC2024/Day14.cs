using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using SpanLinq;

namespace AdventOfCode.Solvers.AoC2024
{
    /// <summary>
    /// Solver for 2024 Day 14
    /// </summary>
    public class Day14 : Solver<Day14.Robot[]>
    {
        public class Robot(int px, int py, int vx, int vy)
        {
            public Vector2<int> Position { get; } = new(px, py);
            public Vector2<int> Velocity { get; } = new(vx, vy);
        }

        private const string ROBOT_PATTERN = @"p=(\d{1,3}),(\d{1,3}) v=(-?\d{1,2}),(-?\d{1,2})";

        private const int PART1_TIME = 100;
        private static readonly Vector2<int> SpaceSize = (101, 103);

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day14"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Robot"/>[] fails</exception>
        public Day14(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            int dangerLevel = GetDangerLevel(PART1_TIME);
            AoCUtils.LogPart1(dangerLevel);

            // The easter egg might not be *the* lowest danger time, so we'll take the best five and print them all
            IEnumerable<int> potentialTimes = (1..^10_000).AsEnumerable().OrderBy(GetDangerLevel).Take(5);
            Grid<bool> view = new(SpaceSize.X, SpaceSize.Y, toString: v => v ? @"█" : " ");

            // Print potential answers
            AoCUtils.LogPart2("One of the following times should have a christmas tree\n");
            foreach (int time in potentialTimes)
            {
                FillGrid(view, time);
                AoCUtils.Log($"Time: {time}");
                AoCUtils.Log(view + "\n");
                view.Clear();
            }
        }

        private int GetDangerLevel(int time)
        {
            Span<int> quadrants = stackalloc int[5];
            foreach (Robot robot in this.Data)
            {
                Vector2<int> finalPosition = robot.Position + (robot.Velocity * time);
                finalPosition = (finalPosition.X.Mod(SpaceSize.X), finalPosition.Y.Mod(SpaceSize.Y));
                quadrants[GetQuadrant(finalPosition)]++;
            }
            return quadrants[..4].Aggregate((a, b) => a * b);
        }

        private void FillGrid(Grid<bool> grid, int time)
        {
            foreach (Robot robot in this.Data)
            {
                Vector2<int> finalPosition = robot.Position + (robot.Velocity * time);
                finalPosition       = (finalPosition.X.Mod(SpaceSize.X), finalPosition.Y.Mod(SpaceSize.Y));
                grid[finalPosition] = true;
            }
        }

        private static int GetQuadrant(in Vector2<int> position)
        {
            return (position.X.CompareTo(SpaceSize.X / 2), position.Y.CompareTo(SpaceSize.Y / 2)) switch
            {
                (< 0, < 0) => 0,
                (> 0, < 0) => 1,
                (< 0, > 0) => 2,
                (> 0, > 0) => 3,
                _          => 4
            };
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override Robot[] Convert(string[] rawInput) => RegexFactory<Robot>.ConstructObjects(ROBOT_PATTERN, rawInput, RegexOptions.Compiled);
        #endregion
    }
}

