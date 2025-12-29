using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Collections;
using ZLinq;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 25
/// </summary>
public sealed class Day25 : ArraySolver<Day25.Star>
{
    public sealed class Star
    {
        public Vector4<int> Position { get; }

        public List<Star> Constellation { get; set; }

        public Star(Vector4<int> position)
        {
            this.Position = position;
            this.Constellation = [this];
        }
    }

    /// <summary>
    /// Creates a new <see cref="Day25"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day25(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        foreach ((Star a, Star b) in this.Data.EnumeratePairs())
        {
            if (a.Constellation == b.Constellation) continue;

            if (Vector4<int>.ManhattanDistance(a.Position, b.Position) <= 3)
            {
                a.Constellation.AddRange(b.Constellation);
                foreach (Star other in b.Constellation)
                {
                    other.Constellation = a.Constellation;
                }
            }
        }

        int constellations = this.Data
                                 .Select(s => s.Constellation)
                                 .Distinct()
                                 .Count();
        AoCUtils.LogPart1(constellations);
    }

    /// <inheritdoc />
    protected override Star ConvertLine(string line) => new(Vector4<int>.Parse(line));
}
