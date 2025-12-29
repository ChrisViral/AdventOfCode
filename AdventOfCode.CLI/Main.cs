using System.Diagnostics;
using AdventOfCode.CLI;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using ZLinq;

Console.Title = "Advent of Code";
SolverData solverData;
try
{
    solverData = await SolverData.CreateData(args).ConfigureAwait(false);
}
catch (Exception e)
{
    //If any exception happens, immediately hop out
    ExitOnException("Exception while creating solverData", e);
    return;
}

ISolver solver;
Stopwatch parseWatch;
try
{
    //Helpful types
    Type   baseSolverType        = typeof(Solver);
    Type[] constructorParamTypes = [typeof(string)];

    //Making sure our solver types are valid
    Debug.Assert(typeof(ISolver).IsAssignableFrom(baseSolverType), $"{baseSolverType} does not inherit from {typeof(ISolver)}");

    //Get solver types
    Type? solverType = AppDomain.CurrentDomain
                                .GetAssemblies()
                                .Single(a => a.GetName().Name == nameof(AdventOfCode))
                                .GetTypes()
                                .Where(t => t is { IsAbstract: false, IsGenericType: false }
                                         && t.IsAssignableTo(baseSolverType)
                                         && t.GetConstructor(constructorParamTypes) is not null)
                               .SingleOrDefault(t => t.FullName == solverData.fullName);

    //Make sure the type exists
    if (solverType is null)
    {
        Exit($"Could not find a matching Solver for {solverData}", 1);
        return;
    }

    parseWatch = Stopwatch.StartNew();
    //Instantiate the solver
    solver = (ISolver)Activator.CreateInstance(solverType, solverData.input)!;  //Throw if cast fails
    parseWatch.Stop();
}
catch (Exception e)
{
    //If any exception happens, immediately hop out
    ExitOnException($"Exception while creating solver for {solverData}", e);
    return;
}

// Preheat logging methods for timing reasons
AoCUtils.LogPart1(0);
AoCUtils.LogPart2(0);
AoCUtils.Log(0);
AoCUtils.LogElapsed();

//Setup trace file
#if DEBUG
using TextWriterTraceListener textListener = new(File.CreateText(Path.Combine("..", "..", "..", "..", nameof(AdventOfCode), "results.txt")));
#else
using TextWriterTraceListener textListener = new(File.CreateText("results.txt"));
#endif
Trace.Listeners.Add(textListener);
using ConsoleTraceListener consoleListener = new();
Trace.Listeners.Add(consoleListener);
Trace.AutoFlush = true;

AoCUtils.Log($"Running Solver for {solverData}\n");
AoCUtils.LogParse(parseWatch);

#if DEBUG
//In debug mode we want to break at the exception location
solver.RunAndStartStopwatch();
solver.Dispose();
#else
try
{
    //Run solver
    solver.RunAndStartStopwatch();
}
catch (Exception e)
{
    //Log any exceptions that occur
    ExitOnException($"Exception while running solver {solver.GetType().Name}", e);
    return;
}
finally
{
    solver.Dispose();
}
#endif

//Write total timer
AoCUtils.LogElapsed();

//Cleanup and exit
Trace.Close();
Exit();
return;

static void ExitOnException(string message, Exception e) => Exit($"{message}\n[{e.GetType().Name}]: {e.Message}\n{e.StackTrace}\n", 1);

static void Exit(string? message = null, int exitCode = 0)
{
    //Log message if any
    if (!string.IsNullOrEmpty(message))
    {
        Console.WriteLine(message);
    }

    //Exit
    Environment.Exit(exitCode);
}
