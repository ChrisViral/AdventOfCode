using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Numbers;

namespace AdventOfCode.AoC2025;

/// <summary>
/// Solver for 2025 Day 1
/// </summary>
[Solver(2025, 1)]
public sealed partial class Day01 : ArraySolver<int>
{
    private const int DIAL_SIZE = 100;
    private const int DIAL_START = 50;

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int zeroes = 0;
        int dial = DIAL_START;
        foreach (int move in this.Data)
        {
            dial = (dial + move).Mod(DIAL_SIZE);
            if (dial is 0) zeroes++;
        }
        LogAnswer(zeroes);

        zeroes = 0;
        dial   = DIAL_START;
        foreach (int move in this.Data)
        {
            int rawDial = dial + move;
            dial = rawDial.Mod(DIAL_SIZE);
            switch (rawDial)
            {
                case 0:
                    zeroes++;
                    break;

                case < 0:
                    zeroes += (-rawDial / DIAL_SIZE) + (rawDial != move ? 1 : 0);
                    break;

                case >= DIAL_SIZE:
                    zeroes += rawDial / DIAL_SIZE;
                    break;
            }
        }
        LogAnswer(zeroes);
    }

    /// <inheritdoc />
    protected override int ConvertLine(string line)
    {
        int length = int.Parse(line.AsSpan(1));
        return line[0] is 'L' ? -length : length;
    }
}
