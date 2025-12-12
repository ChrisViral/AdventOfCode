using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 13
/// </summary>
public sealed class Day13 : Solver<(int timestamp, int[] buses)>
{
    /// <summary>
    /// Out of service line no
    /// </summary>
    private const int NO_SERVICE = -1;

    /// <summary>
    /// Creates a new <see cref="Day13"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day13(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int shortestWait = int.MaxValue;
        int shortestId = 0;
        foreach (int id in this.Data.buses.Where(b => b is not NO_SERVICE))
        {
            int wait = ((int)Math.Ceiling(this.Data.timestamp / (double)id) * id) - this.Data.timestamp;
            if (wait < shortestWait)
            {
                shortestWait = wait;
                shortestId = id;
            }
        }
        AoCUtils.LogPart1(shortestId * shortestWait);

        long lastStart = 0L;
        long lastFreq = this.Data.buses[0];
        for (int i = NextBus(0); i < this.Data.buses.Length; i = NextBus(i))
        {
            long current = this.Data.buses[i];
            long freq = long.LCM(lastFreq, current);
            long pos = freq - lastFreq + lastStart + i;
            while (pos % current is not 0)
            {
                pos -= lastFreq;
            }

            lastStart = pos - i;
            lastFreq = freq;
        }

        AoCUtils.LogPart2(lastStart);
    }

    /// <summary>
    /// Gets the next valid bus line index
    /// </summary>
    /// <param name="index">Current bus line index</param>
    /// <returns>The next valid bus line index, or n + 1 if already at the last</returns>
    private int NextBus(int index)
    {
        while (++index < this.Data.buses.Length && this.Data.buses[index] is NO_SERVICE) { }
        return index;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (int, int[]) Convert(string[] rawInput) => (int.Parse(rawInput[0]),
                                                                   rawInput[1].Split(',').ConvertAll(s => s is not "x" ? int.Parse(s) : NO_SERVICE));
}
