#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using AdventOfCode.Solvers.Base;

namespace AdventOfCode
{
    public static class Program
    {
        private static readonly Type baseSolverType = typeof(Solver);
        private static readonly Type[] constructorTypes = { typeof(FileInfo) };

        private static void Main(string[] args)
        {
            //Check for arguments
            if (args.Length == 0)
            {
                Exit("No day specified", 1);
            }

            //Make sure the file exists
            string day = args[0];
            FileInfo inputFile = new FileInfo($@"Input\{day.ToLower()}.txt");
            if (!inputFile.Exists)
            {
                Exit($"Input file {day}.txt does not exist.", 1);
                return;
            }

            ISolver solver;
            try
            {
                //Get solver types
                Type? solverType = Assembly.GetCallingAssembly()
                                           .GetTypes()
                                           .Where(t => !t.IsAbstract
                                                    && !t.IsGenericType
                                                    && t.IsAssignableTo(baseSolverType)
                                                    && t.GetConstructor(constructorTypes) is not null)
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
                Exit($"Exception while creating solver for {day}\n[{e.GetType().Name}]: {e.Message}\n{e.StackTrace}\n", 1);
                return;
            }

            //Setup trace file
            #if DEBUG
            using TextWriterTraceListener textListener = new TextWriterTraceListener(File.CreateText(@"..\..\..\results.txt"));
            #else
            using TextWriterTraceListener textListener = new TextWriterTraceListener(File.CreateText("results.txt"));
            #endif
            Trace.Listeners.Add(textListener);
            using ConsoleTraceListener consoleListener = new ConsoleTraceListener();
            Trace.Listeners.Add(consoleListener);
            Trace.AutoFlush = true;

            solver.Run();

            Trace.Close();
            Exit();
        }

        private static void Exit(string? message = null, int exitCode = 0)
        {
            if (string.IsNullOrEmpty(message))
            {
                Console.WriteLine(message);
            }

            Environment.Exit(exitCode);
        }
    }
}