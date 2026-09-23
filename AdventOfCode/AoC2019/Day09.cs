using AdventOfCode.AoC2019.Solvers;
using Challenge.Solvers;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 9
/// </summary>
[Solver(2019, 9)]
public sealed partial class Day09 : IntcodeSolver
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        this.VM.Input.AddValue(1L);
        this.VM.Run();
        LogAnswer(this.VM.Output.GetValue());

        this.VM.Reset();
        this.VM.Input.AddValue(2L);
        this.VM.Run();
        LogAnswer(this.VM.Output.GetValue());
    }
}
