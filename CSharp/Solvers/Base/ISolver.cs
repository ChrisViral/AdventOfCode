using System;

namespace AdventOfCode.Solvers.Base;

/// <summary>
/// Solver interface
/// </summary>
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