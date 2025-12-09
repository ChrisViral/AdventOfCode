using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using JetBrains.Annotations;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 17
/// </summary>
public partial class Day17 : IntcodeSolver
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private enum Element
    {
        NONE     = 0,
        EMPTY    = '.',
        SCAFFOLD = '#',
        UP       = '^',
        DOWN     = 'v',
        LEFT     = '<',
        RIGHT    = '>'
    }

    private const char END = '\n';
    // ReSharper disable once ConvertToConstant.Local
    private static readonly bool ShowFeed = true;

    [GeneratedRegex(@"[RL],\d+,")]
    private static partial Regex TokenMatcher { get; }

    [GeneratedRegex(@"^((?:[RL],\d+,)+)\1+$")]
    private static partial Regex SingleTokenMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day17"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day17(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Run VM and extract rows from image
        this.VM.Run();
        Vector2<int> startPosition = Vector2<int>.Zero;
        Direction startDirection   = Direction.NONE;
        List<List<Element>> rows   = GetRows(ref startPosition, ref startDirection);

        // Convert rows to grid
        ConsoleView<Element> grid = new(rows[0].Count, rows.Count, PrintFeed, Anchor.TOP_LEFT, Element.EMPTY);
        foreach (Vector2<int> pos in grid.Dimensions.Enumerate())
        {
            grid[pos] = rows[pos.Y][pos.X];
        }

        // Find intersections
        Vector2<int> search = grid.Dimensions - (2, 2);
        int alignment = search.AsEnumerable()
                              .Select(p => p + Vector2<int>.One)
                              .Where(p => p.Adjacent(includeSelf: true).All(adj => grid[adj] is Element.SCAFFOLD))
                              .Sum(p => p.X * p.Y);
        AoCUtils.LogPart1(alignment);

        // Extract routines
        string path = GetPath(grid, startPosition, startDirection);
        (string a, string b, string c) = ExtractRoutine(path);
        string main = GetMainRoutine(path, a, b, c);

        // Setup routines
        this.VM.Reset();
        this.VM[0] = 2L;
        this.VM.Run();

        // Skip initial print
        int skip = (grid.Width + 1) * grid.Height + 1;
        foreach (int _ in ..skip)
        {
            this.VM.Output.GetOutput();
        }

        // Setup inputs
        Prompt(main);
        Prompt(a);
        Prompt(b);
        Prompt(c);
        Prompt(ShowFeed ? "y" : "n");

        // Start robot
        this.VM.Run();
        this.VM.Output.GetOutput();
        PrintView(grid);

        // Video feed
        if (ShowFeed)
        {
            while (this.VM.Output.Count > grid.Size)
            {
                PrintView(grid);
            }
        }

        AoCUtils.LogPart2(this.VM.Output.GetOutput());
    }

    private List<List<Element>> GetRows(ref Vector2<int> startPosition, ref Direction startDirection)
    {
        List<List<Element>> rows = new(16);
        int y = 0;
        while (!this.VM.Output.IsEmpty)
        {
            int x = 0;
            List<Element> currentRow = new(rows.Count is not 0 ? rows[0].Count : 16);
            while (this.VM.Output.TryGetOutput(out long value) && value is not END)
            {
                Element current = (Element)value;
                switch (current)
                {
                    case Element.SCAFFOLD:
                    case Element.EMPTY:
                        currentRow.Add(current);
                        x++;
                        continue;

                    // Starting position
                    case Element.UP:
                    case Element.DOWN:
                    case Element.LEFT:
                    case Element.RIGHT:
                        startPosition  = (x++, y);
                        startDirection = Direction.Parse((char)current);
                        currentRow.Add(Element.SCAFFOLD);
                        continue;

                    case Element.NONE:
                    default:
                        throw new InvalidOperationException("Invalid scaffolding element detected");
                }
            }

            if (currentRow.IsEmpty) continue;

            rows.Add(currentRow);
            y++;
        }

        return rows;
    }

    private static char PrintFeed(Element element) => element switch
    {
        Element.SCAFFOLD => '▓',
        Element.EMPTY    => ' ',
        Element.NONE     => throw new InvalidOperationException("Cannot print element NONE"),
        _                => (char)element
    };

    private static string GetPath(Grid<Element> grid, Vector2<int> position, Direction direction)
    {
        // Find path through scaffolds
        StringBuilder pathBuilder = new(100);
        while (true)
        {
            // Check for left or right turn
            if (grid.TryMoveWithinGrid(position, direction.TurnLeft(), out Vector2<int> newPosition) && grid[newPosition] is Element.SCAFFOLD)
            {
                direction = direction.TurnLeft();
                pathBuilder.Append("L,");
            }
            else if (grid.TryMoveWithinGrid(position, direction.TurnRight(), out newPosition) && grid[newPosition] is Element.SCAFFOLD)
            {
                direction = direction.TurnRight();
                pathBuilder.Append("R,");
            }
            else
            {
                // At end of path, exit
                return pathBuilder.ToString();
            }

            // Keep moving until unable to
            int distance = 1;
            position = newPosition;
            while (grid.TryMoveWithinGrid(position, direction, out newPosition) && grid[newPosition] is Element.SCAFFOLD)
            {
                position = newPosition;
                distance++;
            }

            pathBuilder.Append(distance).Append(',');
        }
    }

    private static (string a, string b, string c) ExtractRoutine(string path)
    {
        // Tokenize the path
        string a = string.Empty;
        MatchCollection tokens = TokenMatcher.Matches(path);
        for (int i = 1; a.Length <= 17 ; i++)
        {
            // Extract the first routine
            Match aMatch = tokens[i];
            int aEnd = aMatch.Index + aMatch.Length;
            a = path[..aEnd];

            // Remove first routine from path
            string b = string.Empty;
            string remainderPath = Regex.Replace(path, a, string.Empty);
            MatchCollection reducedTokens = TokenMatcher.Matches(remainderPath);
            for (int j = 1; b.Length <= 17; j++)
            {
                // Extract the second routine
                Match bMatch = reducedTokens[j];
                int bEnd = bMatch.Index + bMatch.Length;
                b = remainderPath[..bEnd];
                if (b.Length > 20) break;

                // Remove second routine from path
                string finalPath = Regex.Replace(remainderPath, b, string.Empty);

                // Check if final path only contains third routine
                Match cMatch = SingleTokenMatcher.Match(finalPath);
                if (cMatch.Success)
                {
                    return (a.TrimEnd(','), b.TrimEnd(','), cMatch.Groups[1].Value.TrimEnd(','));
                }
            }
        }

        throw new InvalidOperationException("No routines found");
    }

    private static string GetMainRoutine(ReadOnlySpan<char> path, string a, string b, string c)
    {
        StringBuilder mainBuilder = new(20);
        while (!path.IsEmpty)
        {
            if (path.StartsWith(a))
            {
                mainBuilder.Append("A,");
                path = path[(a.Length + 1)..];
            }
            else if (path.StartsWith(b))
            {
                mainBuilder.Append("B,");
                path = path[(b.Length + 1)..];
            }
            else
            {
                mainBuilder.Append("C,");
                path = path[(c.Length + 1)..];
            }
        }

        mainBuilder.Length--;
        return mainBuilder.ToString();
    }

    private void Prompt(string line)
    {
        // Print prompt
        this.VM.Run();
        Console.Write(this.VM.Output.ReadLine());

        // Push answer
        this.VM.Input.WriteLine(line);
        AoCUtils.Log(line);
    }

    private void PrintView(ConsoleView<Element> grid)
    {
        foreach (int y in ..grid.Height)
        {
            foreach (int x in ..grid.Width)
            {
                grid[x, y] = (Element)this.VM.Output.GetOutput();
            }
            this.VM.Output.GetOutput();
        }
        this.VM.Output.GetOutput();
        grid.PrintToConsole();
    }
}
