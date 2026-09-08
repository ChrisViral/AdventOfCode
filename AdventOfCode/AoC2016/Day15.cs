using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using Microsoft.Z3;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 15
/// </summary>
public sealed partial class Day15 : RegexSolver<Day15.Disc>
{
    public readonly record struct Disc(int Index, int Positions, int Start)
    {
        public BoolExpr GetDiscExpr(Context context, IntExpr t)
        {
            // pos = (t + index + start) % positions
            ArithExpr discPosition = context.MkMod((IntExpr)(t + context.MkInt(this.Index + this.Start)), context.MkInt(this.Positions));
            // pos == 0
            return context.MkEq(discPosition, context.MkInt(0));
        }
    }

    /// <inheritdoc />
    [GeneratedRegex(@"Disc #(\d+) has (\d+) positions; at time=0, it is at position (\d+)")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day15"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day15(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Create optimizer
        using Context context = new();
        using Optimize optimize = context.MkOptimize();

        // Create time variable and add a constraint for it to be greater than zer0
        IntExpr t = context.MkIntConst("t");
        // t > 0
        BoolExpr constraint = context.MkGe(t, context.MkInt(0));
        optimize.Add(constraint);

        // Add all input discs
        // ReSharper disable once AccessToDisposedClosure
        optimize.Add(this.Data.AsEnumerable().Select(d => d.GetDiscExpr(context, t)));

        // Minimize for time
        optimize.MkMinimize(t);

        // Evaluate answer
        optimize.Check();
        AoCUtils.LogPart1(optimize.Model.Evaluate(t));

        // Add final disc and evaluate again
        Disc finalDisc = new(this.Data.Length + 1, 11, 0);
        optimize.Add(finalDisc.GetDiscExpr(context, t));
        optimize.Check();
        AoCUtils.LogPart2(optimize.Model.Evaluate(t));
    }
}
