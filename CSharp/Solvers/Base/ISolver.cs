using System;

namespace AdventOfCode.Solvers.Base
{
    /// <summary>
    /// Solver interface
    /// </summary>
    public interface ISolver : IDisposable
    {
        /// <summary>
        /// Run method, solves the problem from the solver state
        /// </summary>
        void Run();
    }
}
