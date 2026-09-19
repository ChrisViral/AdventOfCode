using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Collections;
using Microsoft.Extensions.Logging;
using ZLinq;

namespace AdventOfCode.AoC2025;

/// <summary>
/// Solver for 2025 Day 04
/// </summary>
[Solver(2025, 4)]
public sealed class Day04 : GridSolver<bool>
{
    private const char ROLL = '@';

    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day04(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        List<Vector2<int>> accessible = new(100);
        HashSet<Vector2<int>> rollPositions = new(100);
        foreach (Vector2<int> position in this.Grid.Dimensions.Enumerate())
        {
            if (!this.Grid[position]) continue;

            int around = position.Adjacent(withDiagonals: true).Count(a => this.Grid.TryGetPosition(a, out bool hasRoll) && hasRoll);
            if (around < 4)
            {
                accessible.Add(position);
            }
            else
            {
                rollPositions.Add(position);
            }
        }
        LogAnswer(accessible.Count);

        int removed = accessible.Count;
        accessible.Clear();
        while (GetAccessible(rollPositions, accessible))
        {
            removed += RemoveRolls(rollPositions, accessible);
        }

        LogAnswer(removed);
    }

    private static bool GetAccessible(HashSet<Vector2<int>> rollPositions, List<Vector2<int>> accessible)
    {
        foreach (Vector2<int> position in rollPositions)
        {
            if (position.Adjacent(withDiagonals: true).Count(rollPositions.Contains) < 4)
            {
                accessible.Add(position);
            }
        }
        return !accessible.IsEmpty;
    }

    private static int RemoveRolls(HashSet<Vector2<int>> rollPositions, List<Vector2<int>> accessible)
    {
        rollPositions.ExceptWith(accessible);
        int count = accessible.Count;
        accessible.Clear();
        return count;
    }

    /// <inheritdoc />
    protected override bool[] LineConverter(string line) => line.Select(c => c is ROLL).ToArray();
}
