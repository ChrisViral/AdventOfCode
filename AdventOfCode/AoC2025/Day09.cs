using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2025;

/// <summary>
/// Solver for 2025 Day 09
/// </summary>
public sealed class Day09 : ArraySolver<Vector2<int>>
{
    /// <summary>
    /// Creates a new <see cref="Day09"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day09(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        long bestArea = 0L;
        foreach (int i in ..(this.Data.Length - 1))
        {
            Vector2<int> a = this.Data[i];
            int end = i is not 0 ? this.Data.Length : this.Data.Length - 1;
            foreach (int j in (i + 1)..end)
            {
                Vector2<int> diff = Vector2<int>.Abs(a - this.Data[j]) + Vector2<int>.One;
                long area = Math.Abs((long)diff.X * diff.Y);
                bestArea = Math.Max(bestArea, area);
            }
        }
        AoCUtils.LogPart1(bestArea);

        Vector2<int> previousCorner = this.Data[^1];
        Dictionary<Vector2<int>, bool> floor = new(this.Data.Length * 100);
        foreach (Vector2<int> corner in this.Data)
        {
            floor[corner] = true;
            DirectionVector<int> travel = (corner - previousCorner).ToDirectionVector();

            Direction direction = travel.X.direction is not Direction.NONE ? travel.X.direction : travel.Y.direction;
            for (Vector2<int> side = previousCorner + direction; side != corner; side += direction)
            {
                floor[side] = true;
            }
            previousCorner = corner;
        }

        bestArea = 0L;
        foreach (int i in ..(this.Data.Length - 1))
        {
            Vector2<int> a = this.Data[i];
            int end = i is not 0 ? this.Data.Length : this.Data.Length - 1;
            foreach (int j in (i + 1)..end)
            {
                Vector2<int> b = this.Data[j];
                Vector2<int> diff = Vector2<int>.Abs(a - b) + Vector2<int>.One;
                long area = Math.Abs((long)diff.X * diff.Y);
                if (bestArea >= area) continue;

                Vector2<int> min = Vector2<int>.Min(a, b);
                Vector2<int> max = Vector2<int>.Max(a, b);
                if (this.Data.Any(c => IsWithinBounds(c, min, max))) continue;

                if (GetSides(min, max).All(s => IsWithinPolygon(s, floor)))
                {
                    bestArea = area;
                }
            }
        }

        AoCUtils.LogPart2(bestArea);
    }

    private static bool IsWithinBounds(Vector2<int> corner, Vector2<int> min, Vector2<int> max)
    {
        return corner.X > min.X
            && corner.Y > min.Y
            && corner.X < max.X
            && corner.Y < max.Y;
    }

    private bool IsWithinPolygon(Vector2<int> position, Dictionary<Vector2<int>, bool> floor)
    {
        if (!floor.TryGetValue(position, out bool isTile))
        {
            isTile = MathUtils.IsInsideAxisAlignedPolygon(position, this.Data);
            floor[position] = isTile;
        }
        return isTile;
    }

    private static IEnumerable<Vector2<int>> GetSides(Vector2<int> min, Vector2<int> max)
    {
        // Corners first for optimization
        yield return min;
        yield return max;
        yield return new Vector2<int>(min.X, max.Y);
        yield return new Vector2<int>(max.X, min.Y);

        // Horizontal edges
        for (int x = min.X + 1; x < max.X; x++)
        {
            yield return new Vector2<int>(x, min.Y);
            yield return new Vector2<int>(x, max.Y);
        }

        // Vertical edges
        for (int y = min.Y + 1; y < max.Y; y++)
        {
            yield return new Vector2<int>(min.X, y);
            yield return new Vector2<int>(max.X, y);
        }
    }

    /// <inheritdoc />
    protected override Vector2<int> ConvertLine(string line) => Vector2<int>.Parse(line);
}
