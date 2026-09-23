using AdventOfCode.AoC2019.Solvers;
using Challenge.Solvers;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 5
/// </summary>
[Solver(2019, 5)]
public sealed partial class Day05 : IntcodeSolver
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        this.VM.Input.AddValue(1L);
        this.VM.Run();
        LogAnswer(this.VM.Output.GetAllValues().Last());

        this.VM.Reset();
        this.VM.Input.AddValue(5L);
        this.VM.Run();
        LogAnswer(this.VM.Output.GetValue());
    }
}
