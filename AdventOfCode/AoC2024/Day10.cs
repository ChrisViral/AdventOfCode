using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using ZLinq;

namespace AdventOfCode.AoC2024;

/// <summary>
/// Solver for 2024 Day 10
/// </summary>
[Solver(2024, 10)]
public sealed class Day10 : GridSolver<int>
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        HashSet<Vector2<int>> trailheadCache = new(100);
        Vector2<int> scores = this.Grid.Dimensions.Enumerate()
                                  .Where(p => this.Grid[p] is 0)
                                  .Sum(p => CalculateTrailheadScore(p, trailheadCache));
        LogAnswer(scores.X);
        LogAnswer(scores.Y);
    }

    private Vector2<int> CalculateTrailheadScore(Vector2<int> startPosition, HashSet<Vector2<int>> trailheads)
    {
        int FindTrailheads(Vector2<int> currentPosition)
        {
            int totalScore = 0;
            int targetHeight = this.Grid[currentPosition] + 1;
            foreach (Vector2<int> move in currentPosition.Adjacent())
            {
                if (!this.Grid.WithinGrid(move)) continue;

                int moveHeight = this.Grid[move];
                if (moveHeight != targetHeight) continue;

                if (moveHeight is 9)
                {
                    totalScore++;
                    trailheads.Add(move);
                }
                else
                {
                    totalScore += FindTrailheads(move);
                }
            }
            return totalScore;
        }

        int distinctScore = FindTrailheads(startPosition);
        int normalScore = trailheads.Count;
        trailheads.Clear();
        return (normalScore, distinctScore);
    }

    /// <inheritdoc />
    protected override int[] LineConverter(string line) => line.AsSpan().Select(c => c - '0').ToArray();
}
