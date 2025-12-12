using System;
using System.Collections;
using AdventOfCode.Utils;
using JetBrains.Annotations;

namespace AdventOfCode.Solvers.Base;

/// <summary>
/// Solver base class
/// </summary>
[PublicAPI]
public abstract class Solver : ISolver
{
    /// <summary>
    /// Default split options
    /// </summary>
    protected const StringSplitOptions DEFAULT_OPTIONS = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    /// <summary>
    /// Default split characters
    /// </summary>
    private static readonly char[] DefaultSplitters = ['\n'];

    /// <summary>
    /// Input data
    /// </summary>
    protected string[] Data { get; }

    /// <summary>
    /// Creates a new <see cref="Solver"/> from the specified file
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="splitters">Splitting characters, defaults to newline only</param>
    /// <param name="options">Input parsing options, defaults to removing empty entries and trimming entries</param>
    protected Solver(string input, char[]? splitters = null, StringSplitOptions options = DEFAULT_OPTIONS)
    {
        if (splitters?.Length is 0)
        {
            this.Data = (options & StringSplitOptions.TrimEntries) is not 0 ? [input] : [input.Trim()];
        }
        else
        {
            this.Data = input.Split(splitters ?? DefaultSplitters, options);
        }
    }

    /// <summary>
    /// Runs the solver and starts the Part 1 stopwatch
    /// </summary>
    public void RunAndStartStopwatch()
    {
        AoCUtils.PartsWatch.Restart();
        Run();
    }

    /// <summary>
    /// Runs the solver on the problem input
    /// </summary>
    public abstract void Run();

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public virtual void Dispose() => GC.SuppressFinalize(this);
}

/// <summary>
/// Solver generic class
/// </summary>
/// <typeparam name="T">The fully parse input type</typeparam>
[PublicAPI]
public abstract class Solver<T> : Solver
{
    /// <summary>
    /// Parsed input data
    /// </summary>
    protected new T Data { get; }

    /// <summary>
    /// If the Solver has been disposed or not
    /// </summary>
    private bool IsDisposed { get; set; }

    /// <summary>
    /// Creates a new generic <see cref="Solver{T}"/> with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="splitters">Splitting characters, defaults to newline only</param>
    /// <param name="options">Input parsing options, defaults to removing empty entries and trimming entries</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <typeparamref name="T"/> fails</exception>
    protected Solver(string input, char[]? splitters = null, StringSplitOptions options = DEFAULT_OPTIONS) : base(input, splitters, options)
    {
#if DEBUG
        //Convert is intended to be a Pure function, therefore it should be safe to call in the constructor
        //ReSharper disable once VirtualMemberCallInConstructor
        this.Data = Convert(base.Data);
#else
        try
        {
            //Convert is intended to be a Pure function, therefore it should be safe to call in the constructor
            //ReSharper disable once VirtualMemberCallInConstructor
            this.Data = Convert(base.Data);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Could not convert the string array input to the {typeof(T)} type using the {nameof(Convert)} method.", e);
        }
#endif
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public override void Dispose()
    {
        if (this.IsDisposed) return;

        switch (this.Data)
        {
            case IDisposable disposable:
                disposable.Dispose();
                break;

            case IEnumerable enumerable:
                foreach (object obj in enumerable)
                {
                    (obj as IDisposable)?.Dispose();
                }
                break;
        }

        this.IsDisposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Input conversion function<br/>
    /// <b>NOTE</b>: This method <b>must</b> be pure as it initializes the base class
    /// </summary>
    /// <param name="rawInput">Input value</param>
    /// <returns>Target converted value</returns>
    [Pure]
    protected abstract T Convert(string[] rawInput);
}
