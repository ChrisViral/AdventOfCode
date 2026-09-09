using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Regexes;
using AdventOfCode.Utils.Extensions.Spans;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 21
/// </summary>
public sealed partial class Day21 : ArraySolver<Day21.Instruction>
{
    public abstract record Instruction
    {
        public abstract void Execute(Span<char> data);

        public abstract void Undo(Span<char> data);
    }

    private sealed partial record SwapPositionsInstruction(int X, int Y) : Instruction
    {
        [GeneratedRegex(@"swap position (\d) with position (\d)")]
        public static partial Regex Matcher { get; }

        /// <inheritdoc />
        public override void Execute(Span<char> data) => AoCUtils.Swap(ref data[this.X], ref data[this.Y]);

        /// <inheritdoc />
        public override void Undo(Span<char> data) => Execute(data);
    }

    private sealed partial record SwapLettersInstruction(char X, char Y) : Instruction
    {
        [GeneratedRegex("swap letter ([a-z]) with letter ([a-z])")]
        public static partial Regex Matcher { get; }

        /// <inheritdoc />
        public override void Execute(Span<char> data)
        {
            int xIndex = data.IndexOf(this.X);
            int yIndex = data.IndexOf(this.Y);
            AoCUtils.Swap(ref data[xIndex], ref data[yIndex]);
        }

        /// <inheritdoc />
        public override void Undo(Span<char> data) => Execute(data);
    }

    private sealed partial record RotateStepsInstruction(int Steps) : Instruction
    {
        [GeneratedRegex(@"rotate (left|right) (\d) steps?")]
        public static partial Regex Matcher { get; }

        /// <inheritdoc />
        public override void Execute(Span<char> data) => data.Rotate(this.Steps);

        /// <inheritdoc />
        public override void Undo(Span<char> data) => data.Rotate(-this.Steps);
    }

    private sealed partial record RotatePositionInstruction(char X) : Instruction
    {
        [GeneratedRegex("rotate based on position of letter ([a-z])")]
        public static partial Regex Matcher { get; }

        /// <inheritdoc />
        public override void Execute(Span<char> data)
        {
            int index = data.IndexOf(this.X);
            int steps = GetSteps(index);
            data.Rotate(steps);
        }

        /// <inheritdoc />
        public override void Undo(Span<char> data)
        {
            int index = data.IndexOf(this.X);
            for (int i = 0; i < data.Length; i++)
            {
                int steps = GetSteps(i);
                int finalIndex = (i + steps) % data.Length;
                if (index == finalIndex)
                {
                    data.Rotate(-steps);
                    return;
                }
            }

            throw new UnreachableException("No reverse steps found");
        }

        private static int GetSteps(int index) => index < 4 ? index + 1 : index + 2;
    }

    private sealed partial record ReversePositionsInstruction(int X, int Y) : Instruction
    {
        [GeneratedRegex(@"reverse positions (\d) through (\d)")]
        public static partial Regex Matcher { get; }

        /// <inheritdoc />
        public override void Execute(Span<char> data) => data[this.X..(this.Y + 1)].Reverse();

        /// <inheritdoc />
        public override void Undo(Span<char> data) => Execute(data);
    }

    private sealed partial record MovePositionInstruction(int X, int Y) : Instruction
    {
        [GeneratedRegex(@"move position (\d) to position (\d)")]
        public static partial Regex Matcher { get; }

        /// <inheritdoc />
        public override void Execute(Span<char> data)
        {
            if (this.X > this.Y)
            {
                data[this.Y..(this.X + 1)].Rotate(1);
            }
            else
            {
                data[this.X..(this.Y + 1)].Rotate(-1);
            }
        }

        /// <inheritdoc />
        public override void Undo(Span<char> data)
        {
            if (this.X > this.Y)
            {
                data[this.Y..(this.X + 1)].Rotate(-1);
            }
            else
            {
                data[this.X..(this.Y + 1)].Rotate(1);
            }
        }
    }

    private const string TEST     = "abcdefgh";
    private const string PASSWORD = "fbgdceah";

    /// <summary>
    /// Creates a new <see cref="Day21"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day21(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Span<char> scrambled = stackalloc char[TEST.Length];
        TEST.CopyTo(scrambled);
        foreach (Instruction instruction in this.Data)
        {
            instruction.Execute(scrambled);
        }
        AoCUtils.LogPart1(scrambled.ToString());

        PASSWORD.CopyTo(scrambled);
        foreach (Instruction instruction in this.Data.Reversed())
        {
            instruction.Undo(scrambled);
        }
        AoCUtils.LogPart2(scrambled.ToString());
    }

    /// <inheritdoc />
    protected override Instruction ConvertLine(string line)
    {
        Match match;
        if ((match = SwapPositionsInstruction.Matcher.Match(line)).Success)
        {
            using RegexExtensions.CapturesEnumerator enumerator = match.CapturedGroups.Enumerator;
            enumerator.TryGetNext(out Group x);
            enumerator.TryGetNext(out Group y);
            return new SwapPositionsInstruction(int.Parse(x.ValueSpan), int.Parse(y.ValueSpan));
        }
        if ((match = SwapLettersInstruction.Matcher.Match(line)).Success)
        {
            using RegexExtensions.CapturesEnumerator enumerator = match.CapturedGroups.Enumerator;
            enumerator.TryGetNext(out Group x);
            enumerator.TryGetNext(out Group y);
            return new SwapLettersInstruction(x.ValueSpan[0], y.ValueSpan[0]);
        }

        if ((match = RotateStepsInstruction.Matcher.Match(line)).Success)
        {
            using RegexExtensions.CapturesEnumerator enumerator = match.CapturedGroups.Enumerator;
            enumerator.TryGetNext(out Group direction);
            enumerator.TryGetNext(out Group steps);
            int sign = direction.ValueSpan is "left" ? -1 : 1;
            return new RotateStepsInstruction(sign * int.Parse(steps.ValueSpan));
        }

        if ((match = RotatePositionInstruction.Matcher.Match(line)).Success)
        {
            using RegexExtensions.CapturesEnumerator enumerator = match.CapturedGroups.Enumerator;
            enumerator.TryGetNext(out Group x);
            return new RotatePositionInstruction(x.ValueSpan[0]);
        }

        if ((match = ReversePositionsInstruction.Matcher.Match(line)).Success)
        {
            using RegexExtensions.CapturesEnumerator enumerator = match.CapturedGroups.Enumerator;
            enumerator.TryGetNext(out Group x);
            enumerator.TryGetNext(out Group y);
            return new ReversePositionsInstruction(int.Parse(x.ValueSpan), int.Parse(y.ValueSpan));
        }

        if ((match = MovePositionInstruction.Matcher.Match(line)).Success)
        {
            using RegexExtensions.CapturesEnumerator enumerator = match.CapturedGroups.Enumerator;
            enumerator.TryGetNext(out Group x);
            enumerator.TryGetNext(out Group y);
            return new MovePositionInstruction(int.Parse(x.ValueSpan), int.Parse(y.ValueSpan));
        }

        throw new UnreachableException("No instruction matched");
    }
}
