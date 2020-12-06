using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using AdventOfCode.Solvers.Base;

//Check for arguments
if (args.Length == 0)
{
    Exit("No day specified", 1);
}

//Make sure the file exists
string day = args[0];
FileInfo inputFile = new($@"Input\{day.ToLower()}.txt");
if (!inputFile.Exists)
{
    Exit($"Input file {day.ToLower()}.txt does not exist.", 1);
    return;
}

ISolver solver;
try
{
    //Helpful types
    Type   solverInterfaceType   = typeof(ISolver);
    Type   baseSolverType        = typeof(Solver);
    Type[] constructorParamTypes = { typeof(FileInfo) };
    
    //Making sure our solver types are valid
    Debug.Assert(solverInterfaceType.IsAssignableFrom(baseSolverType), $"{baseSolverType} does not inherit from {solverInterfaceType}");

    //Get solver types 
    Type? solverType = Assembly.GetCallingAssembly()
                               .GetTypes()
                               .Where(t => !t.IsAbstract
                                        && !t.IsGenericType
                                        && t.IsAssignableTo(baseSolverType)
                                        && t.GetConstructor(BindingFlags.Public, null, constructorParamTypes, null) is not null)
                               .SingleOrDefault(t => t.Name == day);

    //Make sure the type exists
    if (solverType is null)
    {
        Exit($"Could not find a matching Solver for {day}", 1);
        return;
    }

    //Instantiate the solver
    solver = (ISolver)Activator.CreateInstance(solverType, inputFile)!; //Throw if cast fails
}
catch (Exception e)
{
    //If any exception happens, immediately hop out
    ExitOnException($"Exception while creating solver for {day}", e);
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

//Cleanup and exit
Trace.Close();
Exit();


#region Methods
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
#endregion