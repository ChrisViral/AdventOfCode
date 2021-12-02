using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using AdventOfCode;
using AdventOfCode.Solvers.Base;

#region Main
Console.Title = "Advent of Code";
SolverData solverData;
try
{
    solverData = new(args);
}
catch (Exception e)
{
    //If any exception happens, immediately hop out
    ExitOnException("Exception while creating solverData", e);
    return;
}

ISolver solver;
try
{
    //Helpful types
    Type   solverInterfaceType   = typeof(ISolver);
    Type   baseSolverType        = typeof(Solver);
    Type[] constructorParamTypes = { typeof(string) };

    //Making sure our solver types are valid
    Debug.Assert(solverInterfaceType.IsAssignableFrom(baseSolverType), $"{baseSolverType} does not inherit from {solverInterfaceType}");

    //Get solver types
    Type? solverType = Assembly.GetCallingAssembly()
                               .GetTypes()
                               .Where(t => !t.IsAbstract
                                        && !t.IsGenericType
                                        && t.IsAssignableTo(baseSolverType)
                                        && t.GetConstructor(constructorParamTypes) is not null)
                               .SingleOrDefault(t => t.FullName == solverData.fullName);

    //Make sure the type exists
    if (solverType is null)
    {
        Exit($"Could not find a matching Solver for {solverData}", 1);
        return;
    }

    //Instantiate the solver
    solver = (ISolver)Activator.CreateInstance(solverType, solverData.input)!; //Throw if cast fails
}
catch (Exception e)
{
    //If any exception happens, immediately hop out
    ExitOnException($"Exception while creating solver for {solverData}", e);
    return;
}

//Setup trace file
#if DEBUG
using TextWriterTraceListener textListener = new(File.CreateText(@"..\..\..\results.txt"));
#else
using TextWriterTraceListener textListener = new(File.CreateText("results.txt"));
#endif
Trace.Listeners.Add(textListener);
using ConsoleTraceListener consoleListener = new();
Trace.Listeners.Add(consoleListener);
Trace.AutoFlush = true;

Trace.WriteLine("Running Solver for " + solverData);

try
{
    //Run solver
    solver.Run();
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

//Cleanup and exit
Trace.Close();
Exit();
#endregion

#region Methods
static void ExitOnException(string message, Exception e) => Exit($"{message}\n[{e.GetType().Name}]: {e.Message}\n{e.StackTrace}\n", 1);

static void Exit(string? message = null, int exitCode = 0)
{
    //Log message if any
    if (!string.IsNullOrEmpty(message))
    {
        Console.WriteLine(message);
    }

    #if !DEBUG
    //Wait for keypress
    Console.WriteLine("Press any key to continue...");
    Console.ReadKey(true);
    #endif

    //Exit
    Environment.Exit(exitCode);
}
#endregion