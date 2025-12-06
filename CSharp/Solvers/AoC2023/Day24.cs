using System;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using Microsoft.Z3;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 24
/// </summary>
public sealed class Day24 : ArraySolver<Day24.Hail>
{
    public readonly struct Hail
    {
        public Vector3<long> V { get; }

        public Vector3<long> P { get; }

        public Hail(string data)
        {
            string[] splits = data.Split('@', DEFAULT_OPTIONS);
            this.V = Vector3<long>.Parse(splits[0]);
            this.P = Vector3<long>.Parse(splits[1]);
        }

        public static bool FindIntersection(in Hail h1, in Hail h2, out Vector2<double> result)
        {
            if (double.Approximately(h1.P.Y / (double)h1.P.X, h2.P.Y / (double)h2.P.X))
            {
                // Parallel
                result = Vector2<double>.Zero;
                return false;
            }

            (long a, long b, long c, long d) = (h1.P.X, h1.V.X, h2.P.X, h2.V.X);
            (long e, long f, long g, long h) = (h1.P.Y, h1.V.Y, h2.P.Y, h2.V.Y);

            double s = ((a * h) - (a * f) - (e * d) + e * b) / (double)((e * c) - (a * g));
            double t = ((c * s) + d - b) / a;

            if (s < 0d || t < 0d)
            {
                // Past
                result = Vector2<double>.Zero;
                return false;
            }

            double x = (a * t) + b;
            double y = (e * t) + f;
            result = (x, y);
            return true;
        }
    }

    private const long MIN = 200000000000000L;
    private const long MAX = 400000000000000L;
    private const int SAMPLE = 3;

    /// <summary>
    /// Creates a new <see cref="Day24"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Hail"/> fails</exception>
    public Day24(string input) : base(input) { }

    /// <inheritdoc cref="AdventOfCode.Solvers.Base.Solver.Run"/>
    public override void Run()
    {
        int collisions = 0;
        foreach (int i in ..(this.Data.Length - 1))
        {
            Hail a = this.Data[i];
            foreach (int j in (i + 1)..this.Data.Length)
            {
                Hail b = this.Data[j];
                if (Hail.FindIntersection(a, b, out Vector2<double> intersection)
                 && intersection is  { X: > MIN and < MAX, Y: > MIN and < MAX })
                {
                    collisions++;
                }
            }
        }

        AoCUtils.LogPart1(collisions);

        // Yeah, I hate using a package this way, but I am absolutely fucking not solving a system of nine unknown variables by hand
        // ReSharper disable once RedundantNameQualifier
        using Microsoft.Z3.Context ctx = new();

        IntExpr px = ctx.MkIntConst("px");
        IntExpr py = ctx.MkIntConst("py");
        IntExpr pz = ctx.MkIntConst("pz");
        IntExpr vx = ctx.MkIntConst("vx");
        IntExpr vy = ctx.MkIntConst("vy");
        IntExpr vz = ctx.MkIntConst("vz");

        Solver solver = ctx.MkSolver();
        foreach (int i in ..SAMPLE)
        {
            Hail hi     = this.Data[i];
            IntExpr ti  = ctx.MkIntConst($"t{i}");
            BoolExpr xi = ctx.MkEq((vx * ti) + px, (ctx.MkInt(hi.P.X) * ti) + ctx.MkInt(hi.V.X));
            BoolExpr yi = ctx.MkEq((vy * ti) + py, (ctx.MkInt(hi.P.Y) * ti) + ctx.MkInt(hi.V.Y));
            BoolExpr zi = ctx.MkEq((vz * ti) + pz, (ctx.MkInt(hi.P.Z) * ti) + ctx.MkInt(hi.V.Z));
            solver.Add(xi, yi, zi);
        }

        solver.Check();
        Expr result = solver.Model.Evaluate(px + py + pz);
        AoCUtils.LogPart2(result);
    }

    /// <inheritdoc />
    protected override Hail ConvertLine(string line) => new(line);
}
