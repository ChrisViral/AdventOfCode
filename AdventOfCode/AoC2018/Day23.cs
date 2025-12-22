using System.Text.RegularExpressions;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using Microsoft.Z3;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 23
/// </summary>
public sealed partial class Day23 : RegexSolver<Day23.Nanobot>
{
    public sealed record Nanobot(Vector3<int> Position, int Radius);

    public sealed record VectorZ3(IntExpr X, IntExpr Y, IntExpr Z);

    /// <inheritdoc />
    [GeneratedRegex(@"pos=<(-?\d+,-?\d+,-?\d+)>, r=(\d+)")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day23"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day23(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Nanobot strongest = this.Data.MaxBy(n => n.Radius)!;
        int nanobotsInRange = this.Data.Count(n => Vector3<int>.ManhattanDistance(n.Position, strongest.Position) <= strongest.Radius);
        AoCUtils.LogPart1(nanobotsInRange);

        // It's Z3 time
        using Context context = new();

        // Variables
        IntExpr x = context.MkIntConst("x");
        IntExpr y = context.MkIntConst("y");
        IntExpr z = context.MkIntConst("z");

        // Create position and distance to origin expression
        VectorZ3 position = new(x, y, z);
        ArithExpr distanceToOrigin = ManhattanDistanceZ3(context, position, Vector3<int>.Zero);

        // Create total in range summation
        ArithExpr? inRange = null;
        foreach (Nanobot nanobot in this.Data)
        {
            // Get nanobots in range
            ArithExpr dist = ManhattanDistanceZ3(context, position, nanobot.Position);
            ArithExpr term = (ArithExpr)context.MkITE(context.MkLe(dist, context.MkInt(nanobot.Radius)),
                                                      context.MkInt(1),
                                                      context.MkInt(0));
            inRange = inRange is not null ? context.MkAdd(inRange, term) : term;
        }

        // Optimize
        Optimize optimize = context.MkOptimize();

        // Maximize in range, minimize distance to origin
        optimize.MkMaximize(inRange);
        optimize.MkMinimize(distanceToOrigin);

        // Run solver
        optimize.Check();
        Model model = optimize.Model;

        // Get final position out
        int px = ((IntNum)model.Evaluate(x)).Int;
        int py = ((IntNum)model.Evaluate(y)).Int;
        int pz = ((IntNum)model.Evaluate(z)).Int;
        AoCUtils.LogPart2(px + py + pz);
    }

    private static ArithExpr ManhattanDistanceZ3(Context context, VectorZ3 a, Vector3<int> b)
    {
        // Manhattan distance
        IntExpr dx = AbsZ3(context, context.MkSub(a.X, context.MkInt(b.X)));
        IntExpr dy = AbsZ3(context, context.MkSub(a.Y, context.MkInt(b.Y)));
        IntExpr dz = AbsZ3(context, context.MkSub(a.Z, context.MkInt(b.Z)));
        return context.MkAdd(dx, dy, dz);
    }

    private static IntExpr AbsZ3(Context context, ArithExpr value)
    {
        return (IntExpr)context.MkITE(context.MkGe(value, context.MkInt(0)),
                                      value,
                                      context.MkUnaryMinus(value));
    }
}
