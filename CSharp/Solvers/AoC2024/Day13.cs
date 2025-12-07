using System;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using JetBrains.Annotations;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 13
/// </summary>
public sealed class Day13 : Solver<Day13.ClawMachine[]>
{
    /// <summary>
    /// Claw machine data
    /// </summary>
    /// <param name="ax">Button A X parameter</param>
    /// <param name="ay">Button A Y parameter</param>
    /// <param name="bx">Button B X parameter</param>
    /// <param name="by">Button B Y parameter</param>
    /// <param name="px">Prize X parameter</param>
    /// <param name="py">Prize Y parameter</param>
    [UsedImplicitly]
    public sealed class ClawMachine(int ax, int ay, int bx, int by, int px, int py)
    {
        /// <summary>
        /// Button A movement
        /// </summary>
        public Vector2<long> ButtonA { get; } = (ax, ay);

        /// <summary>
        /// Button B movement
        /// </summary>
        public Vector2<long> ButtonB { get; } = (bx, by);

        /// <summary>
        /// Prize position
        /// </summary>
        public Vector2<long> Prize   { get; } = (px, py);

        /// <summary>
        /// Tries to get a linear combination of the button presses that reaches the prize
        /// </summary>
        /// <param name="result">The valid linear combination, if any</param>
        /// <param name="offset">Prize position offset</param>
        /// <returns><see langword="true"/> if a valid linear combination was found, otherwise <see langword="false"/></returns>
        public bool TryGetLinearCombination(out Vector2<long> result, long offset = 0L)
        {
            // Offset the prize position
            Vector2<long> prizePosition = this.Prize + (offset, offset);

            // Literally just a 2x2 matrix reduction
            long n = (this.ButtonA.X * this.ButtonB.Y)  - (this.ButtonB.X  * this.ButtonA.Y);
            long m = (this.ButtonA.X * prizePosition.Y) - (prizePosition.X * this.ButtonA.Y);
            (long a, long ra) = Math.DivRem((n * prizePosition.X) - (m * this.ButtonB.X), n * this.ButtonA.X);
            (long b, long rb) = Math.DivRem(m, n);

            // If any of the remainders aren't zero, we have a fractional button press which isn't allowed
            if (ra != 0L || rb != 0L)
            {
                result = Vector2<long>.Zero;
                return false;
            }

            result = (a, b);
            return true;
        }
    }

    /// <summary>
    /// Claw machine parse pattern
    /// </summary>
    private const string CLAW_MACHINE_PATTERN = @".+\+(\d{2}).+\+(\d{2}).+\+(\d{2}).+\+(\d{2}).+=(\d+).+=(\d+)";
    /// <summary>
    /// Part 2 offset
    /// </summary>
    private const long OFFSET = 10000000000000L;

    /// <summary>
    /// Creates a new <see cref="Day13"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="ClawMachine"/>[] fails</exception>
    public Day13(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        long totalPrice = 0L;
        foreach (ClawMachine machine in this.Data)
        {
            if (machine.TryGetLinearCombination(out Vector2<long> result))
            {
                totalPrice += (result.X * 3L) + result.Y;
            }
        }
        AoCUtils.LogPart1(totalPrice);

        totalPrice = 0L;
        foreach (ClawMachine machine in this.Data)
        {
            if (machine.TryGetLinearCombination(out Vector2<long> result, OFFSET))
            {
                totalPrice += (result.X * 3L) + result.Y;
            }
        }
        AoCUtils.LogPart2(totalPrice);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override ClawMachine[] Convert(string[] rawInput)
    {
        string[] machineClumps = rawInput.Chunk(3).Select(lines => string.Concat(lines)).ToArray();
        return RegexFactory<ClawMachine>.ConstructObjects(CLAW_MACHINE_PATTERN, machineClumps, RegexOptions.Compiled);
    }
}
