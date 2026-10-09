using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils;
using Challenge.Utils.Extensions.Collections;
using ZLinq;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 25
/// </summary>
[Solver(2018, 25)]
public sealed class Day25 : ArraySolver<Day25.Star>
{
    public sealed class Star : IEquatable<Star>
    {
        public Vector4<int> Position { get; }

        public List<Star> Constellation { get; set; }

        public Star(Vector4<int> position)
        {
            this.Position = position;
            this.Constellation = [this];
        }

        /// <inheritdoc />
        public bool Equals(Star? other) => other is not null && this.Position.Equals(other.Position);

        /// <inheritdoc />
        public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Star other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => this.Position.GetHashCode();
    }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        foreach (UnorderedPair<Star> pair in this.Data.EnumeratePairs())
        {
            if (pair.First!.Constellation == pair.Second!.Constellation) continue;

            if (Vector4<int>.ManhattanDistance(pair.First!.Position, pair.Second!.Position) <= 3)
            {
                pair.First!.Constellation.AddRange(pair.Second!.Constellation);
                foreach (Star other in pair.Second!.Constellation)
                {
                    other.Constellation = pair.First!.Constellation;
                }
            }
        }

        int constellations = this.Data
                                 .Select(s => s.Constellation)
                                 .Distinct()
                                 .Count();
        LogAnswer(constellations);
    }

    /// <inheritdoc />
    protected override Star ConvertLine(string line) => new(Vector4<int>.Parse(line));
}
