using JetBrains.Annotations;

namespace AdventOfCode.Solvers.Base;

/// <summary>
/// Solver interface
/// </summary>
[PublicAPI, UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithInheritors)]
public interface ISolver : IDisposable
{
    /// <summary>
    /// Runs the solver and starts the Part 1 stopwatch
    /// </summary>
    void RunAndStartStopwatch();

    /// <summary>
    /// Run method, solves the problem from the solver state
    /// </summary>
    void Run();
}
