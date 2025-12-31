using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Enumerables;
using AdventOfCode.Utils.Extensions.Enums;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Enumerables;
using ZLinq;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 08
/// </summary>
public sealed partial class Day08 : RegexSolver<Day08.Instruction>
{
    public enum Kind
    {
        RECT,
        ROTATE
    }

    public enum RotateTarget
    {
        ROW,
        COLUMN
    }

    public sealed class Instruction
    {
        public Kind Kind { get; }

        public int Width { get; }

        public int Height { get; }

        public RotateTarget RotateTarget { get; }

        public int RotateIndex { get; }

        public int RotateBy { get; }

        public Instruction(int width, int height)
        {
            this.Kind = Kind.RECT;
            this.Width = width;
            this.Height = height;
        }

        public Instruction(RotateTarget rotateTarget, int rotateIndex, int rotateBy)
        {
            this.Kind = Kind.ROTATE;
            this.RotateTarget = rotateTarget;
            this.RotateIndex = rotateIndex;
            this.RotateBy = rotateBy;
        }

        public void ApplyToScreen(Grid<bool> screen)
        {
            switch (this.Kind)
            {
                case Kind.RECT:
                    screen.AsSpan2D(this.Width, this.Height).Fill(true);
                    break;

                case Kind.ROTATE:
                    ApplyRotate(screen);
                    break;

                default:
                    throw this.Kind.Invalid();
            }
        }

        private void ApplyRotate(Grid<bool> screen)
        {
            switch (this.RotateTarget)
            {
                case RotateTarget.ROW:
                    Span<bool> temp = stackalloc bool[screen.Width];
                    Span<bool> row = screen.GetRow(this.RotateIndex);
                    row[^this.RotateBy..].CopyTo(temp);
                    row[..^this.RotateBy].CopyTo(temp[this.RotateBy..]);
                    temp.CopyTo(row);
                    break;

                case RotateTarget.COLUMN:
                    temp = stackalloc bool[screen.Height];
                    RefEnumerable<bool> column = screen.GetColumn(this.RotateIndex);
                    int offset = screen.Height - this.RotateBy;
                    column.Take(offset).CopyTo(temp[this.RotateBy..]);
                    column.Skip(offset).CopyTo(temp);
                    temp.CopyTo(column);
                    break;

                default:
                    throw this.RotateTarget.Invalid();
            }
        }

        public override string ToString()
        {
            return this.Kind is Kind.RECT
                       ? $"rect {this.Width}x{this.Height}"
                       : $"rotate {this.RotateTarget.ToString().ToLowerInvariant()} {(this.RotateTarget is RotateTarget.ROW ? 'x' : 'y')}={this.RotateIndex} by {this.RotateBy}";
        }
    }

    /// <inheritdoc />
    [GeneratedRegex(@"rect (\d+)x(\d+)|rotate (row|column) [xy]=(\d+) by (\d+)")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day08"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day08(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Grid<bool> screen = new(50, 6, b => b ? "#" : ".");
        this.Data.ForEach(i => i.ApplyToScreen(screen));
        AoCUtils.LogPart1(screen.Count(true));
        AoCUtils.LogPart2("\n" + screen);
    }
}
