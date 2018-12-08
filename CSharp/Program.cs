using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AdventOfCode.Tools;
using static System.Console;

namespace AdventOfCode
{
    /// <summary>
    /// Entry point class
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Console commands
        /// </summary>
        public enum Command
        {
            HELP,
            LIST,
            DAY,
            CLEAR,
            EXIT,
            NONE
        }

        #region Constants
        /// <summary>
        /// Challenge command splitter
        /// </summary>
        private static readonly char[] splitter = { ' ' };
        #endregion

        #region Static properties
        /// <summary>
        /// Challenges ID->Solver mapping dictionary
        /// </summary>
        private static Dictionary<int, Challenge> Challenges { get; } = new Dictionary<int, Challenge>(50);
        #endregion

        #region Main
        /// <summary>
        /// Application entry point
        /// </summary>
        private static void Main()
        {
            WriteLine("==== Advent Of Code 2018 ====\n");
            WriteLine("Initializing...");

            //Create solvers
            GetAllSolvers();
            WriteLine(Challenges.Count + " challenge solvers loaded\n");

            WriteLine("Enter 'help' for commands information\n");
            bool loop = true;
            while (loop)
            {
                Command command = GetCommand(out string text);
                switch (command)
                {
                    case Command.HELP:
                        WriteLine("Commands: ");
                        WriteLine("day {id} - Run the solver for the challenge of a given day");
                        WriteLine("clear - Clear the command window");
                        WriteLine("exit - Halts execution");
                        WriteLine("help - Displays command help and information");
                        WriteLine("list - Lists all days IDs that can be run\n");
                        break;

                    case Command.LIST:
                        WriteLine("Days:");
                        WriteLine(string.Join("\n", Challenges.Keys));
                        WriteLine();
                        break;

                    case Command.DAY:
                        if (!int.TryParse(text, out int i) || !Challenges.TryGetValue(i, out Challenge challenge)) { WriteLine("Invalid day ID"); break; }
                        challenge.Solve();
                        WriteLine();
                        break;

                    case Command.CLEAR:
                        Clear();
                        break;

                    case Command.EXIT:
                        loop = false;
                        break;

                    case Command.NONE:
                        WriteLine("Invalid command\n");
                        break;

                }
            }
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Fetches all loaders and creates new instances of them
        /// </summary>
        private static void GetAllSolvers()
        {
            Type challenge = typeof(Challenge);
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => challenge.IsAssignableFrom(t) && !t.IsAbstract && t.IsClass))
            {
                Challenge c = (Challenge)Activator.CreateInstance(type);
                Challenges.Add(c.ID, c);
            }
        }

        /// <summary>
        /// Gets the command entered by the user
        /// </summary>
        /// <param name="text">Text entered during the command</param>
        /// <returns>The command entered, or NONE if invalid</returns>
        private static Command GetCommand(out string text)
        {
            Write(">>>");
            string value = ReadLine()?.Trim().ToUpperInvariant();
            if (!string.IsNullOrEmpty(value))
            {
                string[] splits = value.Split(splitter, 2, StringSplitOptions.RemoveEmptyEntries);
                if (EnumUtils.TryGetValue(splits[0], out Command command))
                {
                    text = splits.Length > 1 ? splits[1] : null;
                    return command;
                }
            }

            text = null;
            return Command.NONE;
        }
        #endregion
    }
}
