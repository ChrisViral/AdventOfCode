using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.BitVectors;
using Microsoft.Z3;

namespace AdventOfCode.Solvers.AoC2025;

/// <summary>
/// Solver for 2025 Day 10
/// </summary>
public sealed partial class Day10 : ArraySolver<Day10.Machine>
{
    public sealed record Machine(ImmutableArray<bool> Lights, ImmutableArray<Button> Buttons, ImmutableArray<int> Joltages)
    {
        public IEnumerable<MoveData<BitVector16, int>> GetUpdatedStates(BitVector16 currentState)
        {
            foreach (Button button in this.Buttons)
            {
                BitVector16 newState = currentState;
                foreach (int connection in button.Connections)
                {
                    newState.InvertBit(connection);
                }
                yield return new MoveData<BitVector16, int>(newState, 1);
            }
        }

        public override string ToString()
        {
            return $"[{string.Join(string.Empty, this.Lights.Select(l => l ? LIT : UNLIT))}] {string.Join(" ", this.Buttons)} {{{string.Join(',', this.Joltages)}}}";
        }
    }

    public readonly record struct Button(ImmutableArray<int> Connections)
    {
        public override string ToString() => $"({string.Join(',', this.Connections)})";
    }

    private sealed class JoltagePressHelper : ParallelHelper<Machine, object?>
    {
        /// <inheritdoc />
        protected override object? Setup() => null;

        /// <inheritdoc />
        protected override void Process(Machine element, in IterationData iteration)
        {
            int presses = GetMinimumPressesJoltages(element);
            Interlocked.Add(ref joltagePresses, presses);
        }

        /// <inheritdoc />
        protected override void Finalize(object? data) { }
    }

    private const char LIT = '#';
    private const char UNLIT = '.';

    [GeneratedRegex(@"\[([\.#]+)\] ([\d,() ]+) {([\d,]+)}")]
    private static partial Regex MachineMatcher { get; }

    [GeneratedRegex(@"[\d,]+")]
    private static partial Regex ButtonMatcher { get; }

    private static int joltagePresses;

    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day10(string input) : base(input) { }

    /// <inheritdoc cref="Base.Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int presses = this.Data.Sum(GetMinimumPresses);
        AoCUtils.LogPart1(presses);

        JoltagePressHelper helper = new();
        helper.ForEach(this.Data);
        AoCUtils.LogPart2(joltagePresses);
    }

    private static int GetMinimumPresses(Machine machine)
    {
        return SearchUtils.GetPathLength(new BitVector16(), BitVector16.FromBitArray(machine.Lights), null,
                                         machine.GetUpdatedStates, MinSearchComparer<int>.Comparer)!.Value;
    }

    private static int GetMinimumPressesJoltages(Machine machine)
    {
        // Create context
        using Context context = new();
        using Optimize optimize = context.MkOptimize();

        // Create presses constants
        ArithExpr[] presses = new ArithExpr[machine.Buttons.Length];
        foreach (int i in ..presses.Length)
        {
            IntExpr p = context.MkIntConst($"p{i}");
            optimize.Add(context.MkGe(p, context.MkInt(0)));
            presses[i] = p;
        }

        // Create equations for each joltage counter
        List<ArithExpr> affectingButtons = new(presses.Length);
        foreach (int i in ..machine.Joltages.Length)
        {
            // Identify which buttons affect this joltage counter
            foreach (int j in ..machine.Buttons.Length)
            {
                if (machine.Buttons[j].Connections.Contains(i))
                {
                    affectingButtons.Add(presses[j]);
                }
            }

            // Create equation
            ArithExpr sum = affectingButtons.Count > 1 ? context.MkAdd(affectingButtons) : affectingButtons[0];
            BoolExpr equality = context.MkEq(sum, context.MkInt(machine.Joltages[i]));
            optimize.Add(equality);
            affectingButtons.Clear();
        }

        // Solve for minimal total presses
        ArithExpr constraint = presses.Length > 1 ? context.MkAdd(presses) : presses[0];
        optimize.MkMinimize(constraint);
        optimize.Check();

        // Extract answer
        Model model = optimize.Model;
        return presses.Select(p => model.Evaluate(p, true)).Cast<IntNum>().Sum(i => i.Int);
    }

    /// <inheritdoc />
    protected override Machine ConvertLine(string line)
    {
        // Match entire line
        if (MachineMatcher.Match(line).Groups is not [_, { } lightsCapture, { } buttonsCapture, { } joltageCapture]) throw new InvalidOperationException("Parse error");

        // Parse lights
        ImmutableArray<bool>.Builder lights = ImmutableArray.CreateBuilder<bool>(lightsCapture.Length);
        foreach (char light in lightsCapture.ValueSpan)
        {
            lights.Add(light is LIT);
        }

        // Parse buttons
        MatchCollection buttonMatches = ButtonMatcher.Matches(buttonsCapture.Value);
        ImmutableArray<Button>.Builder buttons = ImmutableArray.CreateBuilder<Button>(buttonMatches.Count);
        foreach (int i in ..buttonMatches.Count)
        {
            buttons.Add(ParseButton(buttonMatches[i].ValueSpan));
        }

        // Parse joltages
        Span<Range> joltageSplits = stackalloc Range[joltageCapture.ValueSpan.Count(',') + 1];
        joltageCapture.ValueSpan.Split(joltageSplits, ',');
        ImmutableArray<int>.Builder joltages = ImmutableArray.CreateBuilder<int>(joltageSplits.Length);
        foreach (Range split in joltageSplits)
        {
            joltages.Add(int.Parse(joltageCapture.ValueSpan[split]));
        }

        return new Machine(lights.ToImmutable(), buttons.ToImmutable(), joltages.ToImmutable());
    }

    private static Button ParseButton(ReadOnlySpan<char> data)
    {
        Span<Range> buttonSplits = stackalloc Range[data.Count(',') + 1];
        data.Split(buttonSplits, ',');
        ImmutableArray<int>.Builder connections = ImmutableArray.CreateBuilder<int>(buttonSplits.Length);
        foreach (Range split in buttonSplits)
        {
            connections.Add(int.Parse(data[split]));
        }
        return new Button(connections.ToImmutable());
    }
}
