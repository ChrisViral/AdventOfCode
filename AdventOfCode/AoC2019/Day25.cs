using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Maths.Vectors.BitVectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 25
/// </summary>
public sealed partial class Day25 : IntcodeSolver
{
    private sealed class ProgramHaltedException(string reason) : Exception
    {
        public string Reason { get; } = reason;
    }

    [GeneratedRegex(@"- ([\w ]+)")]
    private static partial Regex ItemMatcher { get; }

    [GeneratedRegex(@"lighter|heavier|\d{2,}")]
    private static partial Regex SecurityMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day25"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day25(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        bool explored = false;
        List<string> inventory = new(10);
        HashSet<string> forbiddenItems = new(10) { "giant electromagnet", "infinite loop" };
        Stack<Direction> pathToSecurity = new(10);
        do
        {
            try
            {
                // Explore until the entire ship is seen and all items collected
                this.VM.Run();
                ExploreRoom(Direction.NONE, inventory, forbiddenItems, pathToSecurity);
                explored = true;
            }
            catch (ProgramHaltedException e)
            {
                // If a run-ending item is found, remember it and do not pick it up next time
                AoCUtils.Log("Robot halted, retrying...");
                inventory.Clear();
                forbiddenItems.Add(e.Reason);
                pathToSecurity.Clear();
                this.VM.Reset();
            }
        }
        while (!explored);

        // Navigate back to the security room
        while (pathToSecurity.Count > 1)
        {
            this.VM.Input.WriteLine(pathToSecurity.Pop().ToCardinalString());
        }

        // Drop all items
        inventory.ForEach(item => this.VM.Input.WriteLine($"drop {item}"));

        // Update VM
        this.VM.Run();
        this.VM.Output.PrintAllLines();

        // Get through security
        Direction throughSecurity = pathToSecurity.Pop();
        BitVector8 unavailable = new();
        TrySecurity(inventory, unavailable, throughSecurity.ToCardinalString(), out string password);

        // Log found password
        AoCUtils.LogPart1(password);
    }

    // ReSharper disable once CognitiveComplexity
    private bool ExploreRoom(Direction cameFrom, List<string> inventory, HashSet<string> forbiddenItems, Stack<Direction> pathToSecurity)
    {
        bool foundSecurity = false;
        List<string>? items = null;
        List<Direction> directions = [];
        while (!this.VM.Output.IsEmpty)
        {
            string line = this.VM.Output.ReadLine().Trim();
            AoCUtils.Log(line);
            if (string.IsNullOrWhiteSpace(line)) continue;

            switch (line)
            {
                case "Items here:":
                    items = GetItems(forbiddenItems);
                    break;

                case "Doors here lead:":
                    PopulateDirections(directions, cameFrom.Invert());
                    break;

                case "== Security Checkpoint ==":
                    // Indicate we've found security
                    foundSecurity = true;
                    break;
            }
        }

        if (foundSecurity)
        {
            pathToSecurity.Push(directions[0]);
            return true;
        }

        if (items is not null)
        {
            foreach (string item in items)
            {
                this.VM.Input.WriteLine($"take {item}");
                this.VM.Run();
                this.VM.Output.PrintAllLines();
                if (this.VM.IsHalted) throw new ProgramHaltedException(item);

                inventory.Add(item);
            }
        }

        foundSecurity = false;
        foreach (Direction direction in directions)
        {
            // Request move
            this.VM.Input.WriteLine(direction.ToCardinalString());
            this.VM.Run();

            // Explore next room
            if (ExploreRoom(direction, inventory, forbiddenItems, pathToSecurity))
            {
                // If we found security along the way, add to the path
                foundSecurity = true;
                pathToSecurity.Push(direction);
            }

            // Return to current room
            this.VM.Input.WriteLine(direction.Invert().ToCardinalString());
            this.VM.Run();
            this.VM.Output.PrintAllLines();
        }

        return foundSecurity;
    }

    private List<string> GetItems(HashSet<string> forbiddenItems)
    {
        List<string> items = [];
        string line = this.VM.Output.ReadLine().Trim();
        AoCUtils.Log(line);
        do
        {
            string item = ItemMatcher.Match(line).Groups[1].Value;
            if (!forbiddenItems.Contains(item))
            {
                items.Add(item);
            }
            line = this.VM.Output.ReadLine().Trim();
            AoCUtils.Log(line);
        }
        while (!string.IsNullOrWhiteSpace(line));

        return items;
    }

    private void PopulateDirections(List<Direction> directions, Direction returnDirection)
    {
        string line = this.VM.Output.ReadLine().Trim();
        AoCUtils.Log(line);
        do
        {
            Direction direction = Direction.Parse(ItemMatcher.Match(line).Groups[1].ValueSpan);
            if (direction != returnDirection)
            {
                directions.Add(direction);
            }
            line = this.VM.Output.ReadLine().Trim();
            AoCUtils.Log(line);
        }
        while (!string.IsNullOrWhiteSpace(line));
    }

    // ReSharper disable once CognitiveComplexity
    private bool TrySecurity(List<string> items, BitVector8 unavailable, string throughSecurity, out string password)
    {
        foreach (int i in ..items.Count)
        {
            // Skip checked items
            if (unavailable[i]) continue;

            // Try adding the item and going through security
            unavailable[i] = true;
            string item = items[i];
            this.VM.Input.WriteLine($"take {item}");
            this.VM.Run();
            this.VM.Output.PrintAllLines();

            this.VM.Input.WriteLine(throughSecurity);
            this.VM.Run();

            string securityCheck = string.Empty;
            while (!this.VM.Output.IsEmpty)
            {
                string line = this.VM.Output.ReadLine().Trim();
                AoCUtils.Log(line);
                Match checkMatch = SecurityMatcher.Match(line);
                if (checkMatch.Success)
                {
                    securityCheck = checkMatch.Value;
                }
            }

            switch (securityCheck)
            {
                case "heavier":
                    // If we are too light, we keep going and recurse
                    if (TrySecurity(items, unavailable, throughSecurity, out password))
                    {
                        return true;
                    }
                    break;

                case "lighter":
                    // If we were too heavy do not look further
                    break;

                default:
                    // If we made it through, return the password
                    password = securityCheck;
                    return true;
            }

            // Drop the item, but do not make it available
            this.VM.Input.WriteLine($"drop {item}");
            this.VM.Run();
            this.VM.Output.PrintAllLines();
        }

        // Could not find anything in this branch
        password = string.Empty;
        return false;
    }
}
