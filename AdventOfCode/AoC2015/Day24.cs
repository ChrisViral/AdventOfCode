using AdventOfCode.Maths.Vectors.BitVectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 24
/// </summary>
public sealed class Day24 : ArraySolver<int>
{
    /// <summary>
    /// Creates a new <see cref="Day24"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day24(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        long entanglement = long.MaxValue;
        int totalWeight = this.Data.Sum();
        int targetWeight = totalWeight / 3;
        for (int maxGroupSize = 1; entanglement is long.MaxValue; maxGroupSize++)
        {
            entanglement = FindBestFirstGroup(new BitVector32(), 1, maxGroupSize, 0, targetWeight, 3);
        }
        AoCUtils.LogPart1(entanglement);

        entanglement = long.MaxValue;
        targetWeight = totalWeight / 4;
        for (int maxGroupSize = 1; entanglement is long.MaxValue; maxGroupSize++)
        {
            entanglement = FindBestFirstGroup(new BitVector32(), 1, maxGroupSize, 0, targetWeight, 4);
        }
        AoCUtils.LogPart2(entanglement);
    }

    // ReSharper disable once CognitiveComplexity
    private long FindBestFirstGroup(BitVector32 used, int groupSize, int maxGroupSize, int weightSoFar, int targetWeight, int groups)
    {
        long minEntanglement = long.MaxValue;
        foreach (int i in ..this.Data.Length)
        {
            if (used[i]) continue;

            int newWeight = this.Data[i] + weightSoFar;
            if (newWeight != targetWeight) continue;

            used[i] = true;
            if (HasValidOtherGroup(used, 0, targetWeight, groups - 2))
            {
                long entanglement = GetGroupEntanglement(used);
                minEntanglement = long.Min(minEntanglement, entanglement);
            }
            used[i] = false;
        }

        if (minEntanglement is not long.MaxValue) return minEntanglement;
        if (groupSize == maxGroupSize) return long.MaxValue;

        foreach (int i in ..this.Data.Length)
        {
            if (used[i]) continue;

            used[i] = true;
            int weight = this.Data[i];
            long entanglement = FindBestFirstGroup(used, groupSize + 1, maxGroupSize, weightSoFar + weight, targetWeight, groups);
            minEntanglement = long.Min(minEntanglement, entanglement);
            used[i] = false;
        }

        return minEntanglement;
    }

    // ReSharper disable once CognitiveComplexity
    private bool HasValidOtherGroup(BitVector32 used, int weightSoFar, int targetWeight, int groupsLeft)
    {
        foreach (int i in ..this.Data.Length)
        {
            if (used[i]) continue;

            int newWeight = this.Data[i] + weightSoFar;
            if (newWeight > targetWeight) continue;

            used[i] = true;
            if (newWeight == targetWeight)
            {
                if (groupsLeft is 1) return true;

                used[i] = true;
                if (HasValidOtherGroup(used, 0, targetWeight, groupsLeft - 1))
                {
                    used[i] = false;
                    return true;
                }
                return false;
            }

            used[i] = true;
            if (HasValidOtherGroup(used, newWeight, targetWeight, groupsLeft))
            {
                used[i] = false;
                return true;
            }

            used[i] = false;
        }

        return false;
    }

    private long GetGroupEntanglement(BitVector32 used)
    {
        long entanglement = 1L;
        foreach (int i in ..this.Data.Length)
        {
            if (used[i])
            {
                entanglement *= this.Data[i];
            }
        }
        return entanglement;
    }

    /// <inheritdoc />
    protected override int ConvertLine(string line) => int.Parse(line);
}
