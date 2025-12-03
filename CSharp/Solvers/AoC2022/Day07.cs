#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 07
/// </summary>
public class Day07 : Solver<Day07.Directory>
{
    /// <summary>
    /// File record
    /// </summary>
    /// <param name="Name">File name</param>
    /// <param name="Size">File size</param>
    public record File(string Name, int Size);

    /// <summary>
    /// Directory helper class
    /// </summary>
    public class Directory : IEnumerable<Directory>
    {
        /// <summary>Root directory name</summary>
        private const string ROOT = "/";

        /// <summary>
        /// Directory name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Directory parent
        /// </summary>
        public Directory? Parent { get; }

        /// <summary>
        /// List of child directories
        /// </summary>
        private List<Directory> Children { get; } = new();

        /// <summary>
        /// List of files in this directory
        /// </summary>
        private List<File> Files { get; } = new();

        /// <summary>
        /// Size of this directory, in bytes
        /// </summary>
        public int Size => this.Files.Sum(f => f.Size) + this.Children.Sum(d => d.Size);

        /// <summary>
        /// Creates a new root directory
        /// </summary>
        public Directory() : this(ROOT, null!) { }

        /// <summary>
        /// Creates a new named directory with a given parent
        /// </summary>
        /// <param name="name">Name of the directory</param>
        /// <param name="parent">Parent directory</param>
        private Directory(string name, Directory parent)
        {
            this.Name   = name;
            this.Parent = parent;
        }

        /// <summary>
        /// Adds a new directory as a child of this one
        /// </summary>
        /// <param name="name">Name of the new directory</param>
        public void AddDirectory(string name) => this.Children.Add(new Directory(name, this));

        /// <summary>
        /// Adds a file to this directory
        /// </summary>
        /// <param name="name">Name of the file</param>
        /// <param name="size">Size of the file</param>
        public void AddFile(string name, int size) => this.Files.Add(new File(name, size));

        /// <summary>
        /// Gets a named child directory
        /// </summary>
        /// <param name="name">Name of the directory to get</param>
        /// <returns>The correct child directory, or <see langword="null"/> if none exists</returns>
        public Directory? GetChild(string name) => this.Children.Find(d => d.Name == name);

        /// <summary>
        /// String representation of this Directory
        /// </summary>
        /// <returns>A string containing the name and size of this directory</returns>
        public override string ToString() => $"{this.Name} ({this.Size})";

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public IEnumerator<Directory> GetEnumerator()
        {
            foreach (Directory child in this.Children)
            {
                foreach (Directory grandChild in child)
                {
                    yield return grandChild;
                }

                yield return child;
            }
        }

        /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>Command token</summary>
    private const string COMMAND  = "$";
    /// <summary>Directory token</summary>
    private const string DIR      = "dir";
    /// <summary>List command</summary>
    private const string LS       = "ls";
    /// <summary>Change Directory command</summary>
    private const string CD       = "cd";
    /// <summary>Parent folder name</summary>
    private const string PARENT   = "..";
    /// <summary>Max folder size filter</summary>
    private const int    MAX_SIZE = 100000;
    /// <summary>Maximum used disk space to make system operational</summary>
    private const int    MAX_USED = 40000000;

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day07"/> Solver for 2022 - 07 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day07(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver{T}.Run"/>
    public override void Run()
    {
        int smallDirsTotal = this.Data
                                 .Select(dir => dir.Size)
                                 .Where(size => size < MAX_SIZE)
                                 .Sum();
        AoCUtils.LogPart1(smallDirsTotal);

        int toFree   = this.Data.Size - MAX_USED;
        int toDeleteSize = this.Data
                               .Select(d => d.Size)
                               .OrderBy(s => s)
                               .First(s => s >= toFree);
        AoCUtils.LogPart2(toDeleteSize);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Directory Convert(string[] lines)
    {
        // Create the root
        Directory root = new();
        Directory current = root;
        // First two commands are always "$ cd /" and "$ ls", so we can ignore that
        foreach (string[] tokens in lines[2..].Select(line => line.Split(' ', DEFAULT_OPTIONS)))
        {
            string argument = tokens[1];
            switch (tokens[0])
            {
                case COMMAND when argument is LS:
                    // Nothing to do here really
                    break;

                case COMMAND when argument is CD:
                    // Find the next current directory
                    string target = tokens[2];
                    current = target is PARENT ? current.Parent! : current.GetChild(target)!;
                    break;

                case DIR:
                    // Add a new directory to the current
                    current.AddDirectory(argument);
                    break;

                case { } token when int.TryParse(token, out int size):
                    // Add a new file to the current
                    current.AddFile(argument, size);
                    break;
            }
        }

        return root;
    }
    #endregion
}
