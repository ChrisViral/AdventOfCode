using AdventOfCode.Collections;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enums;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 18
/// </summary>
public sealed class Day18 : GridSolver<Day18.Lumber>
{
    public enum Lumber
    {
        NONE  = 0,
        OPEN  = '.',
        TREES = '|',
        YARD  = '#'
    }

    private const int PART1_CYCLES = 10;
    private const int PART2_CYCLES = 1_000_000_000;

    /// <summary>
    /// Creates a new <see cref="Day18"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day18(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Dictionary<string, int> lumberyardStates = new(1000);
        List<int> resourceValues = new(1000);
        DelayedGrid<Lumber> lumberyard = new(this.Data);
        foreach (int i in ..PART1_CYCLES)
        {
            foreach (Vector2<int> position in lumberyard.Dimensions.Enumerate())
            {
                HandleAcre(lumberyard, position);
            }
            lumberyard.Apply();
            resourceValues.Add(GetResourceValue(lumberyard));
            lumberyardStates.Add(lumberyard.ToString(), i);
        }
        AoCUtils.LogPart1(resourceValues[^1]);

        int finalIndex = -1;
        foreach (int i in PART1_CYCLES..PART2_CYCLES)
        {
            foreach (Vector2<int> position in lumberyard.Dimensions.Enumerate())
            {
                HandleAcre(lumberyard, position);
            }
            lumberyard.Apply();

            // Check if we've seen this state before
            string lumberyardState = lumberyard.ToString();
            if (lumberyardStates.TryGetValue(lumberyardState, out int previousIndex))
            {
                // Calculate the final index
                int cycleSize = i - previousIndex;
                finalIndex = previousIndex + ((PART2_CYCLES - i - 1) % cycleSize);
                break;
            }

            resourceValues.Add(GetResourceValue(lumberyard));
            lumberyardStates[lumberyardState] = i;
        }

        AoCUtils.LogPart2(resourceValues[finalIndex]);
    }

    private static int GetResourceValue(DelayedGrid<Lumber> lumberyard)
    {
        int trees = 0;
        int yards = 0;
        foreach (Lumber yard in lumberyard)
        {
            switch (yard)
            {
                case Lumber.TREES:
                    trees++;
                    continue;

                case Lumber.YARD:
                    yards++;
                    continue;

                case Lumber.OPEN:
                    break;

                case Lumber.NONE:
                    throw new InvalidOperationException("Lumberyard should never be NONE");

                default:
                    throw yard.Invalid();
            }
        }

        return trees * yards;
    }

    private static void HandleAcre(DelayedGrid<Lumber> lumberyard, Vector2<int> position)
    {
        Lumber acre = lumberyard[position];
        switch (acre)
        {
            case Lumber.OPEN:
                HandleOpen(lumberyard, position);
                return;

            case Lumber.TREES:
                HandleTrees(lumberyard, position);
                return;

            case Lumber.YARD:
                HandleYard(lumberyard, position);
                return;

            case Lumber.NONE:
                throw new InvalidOperationException("Lumberyard should never be NONE");

            default:
                acre.ThrowInvalid();
                return;
        }
    }

    private static void HandleOpen(DelayedGrid<Lumber> lumberyard, Vector2<int> position)
    {
        int trees = 0;
        foreach (Vector2<int> adjacent in position.Adjacent(withDiagonals: true))
        {
            if (!lumberyard.TryGetPosition(adjacent, out Lumber otherYard) || otherYard is not Lumber.TREES) continue;

            if (trees is 2)
            {
                lumberyard[position] = Lumber.TREES;
                return;
            }

            trees++;
        }
    }

    private static void HandleTrees(DelayedGrid<Lumber> lumberyard, Vector2<int> position)
    {
        int yards = 0;
        foreach (Vector2<int> adjacent in position.Adjacent(withDiagonals: true))
        {
            if (!lumberyard.TryGetPosition(adjacent, out Lumber otherYard) || otherYard is not Lumber.YARD) continue;

            if (yards is 2)
            {
                lumberyard[position] = Lumber.YARD;
                return;
            }

            yards++;
        }
    }

    private static void HandleYard(DelayedGrid<Lumber> lumberyard, Vector2<int> position)
    {
        bool hasTrees = false;
        bool hasYard = false;
        foreach (Vector2<int> adjacent in position.Adjacent(withDiagonals: true))
        {
            if (!lumberyard.TryGetPosition(adjacent, out Lumber otherYard)) continue;

            switch (otherYard)
            {
                case Lumber.YARD when hasTrees:
                    return;

                case Lumber.YARD:
                    hasYard = true;
                    continue;

                case Lumber.TREES when hasYard:
                    return;

                case Lumber.TREES:
                    hasTrees = true;
                    continue;

                case Lumber.OPEN:
                    continue;

                case Lumber.NONE:
                    throw new InvalidOperationException("Lumberyard should never be NONE");

                default:
                    otherYard.ThrowInvalid();
                    return;
            }
        }

        lumberyard[position] = Lumber.OPEN;
    }

    /// <inheritdoc />
    protected override Lumber[] LineConverter(string line) => line.Select(c => (Lumber)c).ToArray();

    /// <inheritdoc />
    protected override string StringConversion(Lumber yard) => yard switch
    {
        Lumber.OPEN  => ".",
        Lumber.TREES => "|",
        Lumber.YARD  => "#",
        Lumber.NONE  => throw new InvalidOperationException("Lumberyard should never be NONE"),
        _            => throw yard.Invalid()
    };
}
