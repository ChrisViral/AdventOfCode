using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AdventOfCode.Utils;

/// <summary>
/// Helper class to parallelize a process
/// </summary>
/// <typeparam name="TElement">Type of element being processed</typeparam>
/// <typeparam name="TData">Thread data container type</typeparam>
[PublicAPI]
public abstract class ParallelHelper<TElement, TData> where TData : class
{
    /// <summary>
    /// Iteration data stuct
    /// </summary>
    /// <param name="index">Current iteration index</param>
    /// <param name="state">Parallel loop state</param>
    /// <param name="data">Thread loop data</param>
    protected readonly ref struct IterationData(int index, ParallelLoopState state, TData data)
    {
        /// <summary>
        /// Iteration index if running a for loop<br/>
        /// Always 0 during foreach loops
        /// </summary>
        public readonly int index = index;
        /// <summary>
        /// Parallel loop state
        /// </summary>
        public readonly ParallelLoopState state = state;
        /// <summary>
        /// Thread data container
        /// </summary>
        public readonly TData data = data;
    }

    /// <summary>
    /// Sets up thread data
    /// </summary>
    /// <returns>A new thread data container instance</returns>
    protected abstract TData Setup();

    /// <summary>
    /// Processes an iteration of the loop on the given element
    /// </summary>
    /// <param name="element">Element being processed</param>
    /// <param name="iteration">Current iteration data</param>
    protected abstract void Process(TElement element, in IterationData iteration);

    /// <summary>
    /// Finalizes iteration work and processes thread data
    /// </summary>
    /// <param name="data">Thread data to finalize</param>
    protected abstract void Finalize(TData data);

    /// <summary>
    /// Runs a parallel for loop on the given list
    /// </summary>
    /// <param name="list">List to run the for loop over</param>
    /// <returns>The parallel loop result</returns>
    public ParallelLoopResult For(IList<TElement> list)
    {
        // Body function
        TData LoopBody(int i, ParallelLoopState state, TData data)
        {
            // Break out if needed
            if (state.ShouldExitCurrentIteration) return data;

            // Pass in element and state
            TElement element = list[i];
            Process(element, new IterationData(i, state, data));
            return data;
        }

        return Parallel.For(0, list.Count, Setup, LoopBody, Finalize);
    }

    /// <summary>
    /// Runs a parallel foreach loop on the given enumerable
    /// </summary>
    /// <param name="enumerable">Enumerable to run the foreach loop for</param>
    /// <returns>The parallel loop result</returns>
    public ParallelLoopResult ForEach(IEnumerable<TElement> enumerable)
    {
        // Body function
        TData LoopBody(TElement element, ParallelLoopState state, TData data)
        {
            // Break out if needed
            if (state.ShouldExitCurrentIteration) return data;

            Process(element, new IterationData(0, state, data));
            return data;
        }

        return Parallel.ForEach(enumerable, Setup, LoopBody, Finalize);
    }
}