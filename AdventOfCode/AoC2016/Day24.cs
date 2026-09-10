using System.Runtime.CompilerServices;
using AdventOfCode.Collections.Search;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enumerables;
using AdventOfCode.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 24
/// </summary>
public sealed class Day24 : GridSolver<char>
{
    [InlineArray(LOCATIONS)]
    private struct Checks : IEquatable<Checks>
    {
        private bool element;

        /// <inheritdoc />
        public bool Equals(Checks other) => ((ReadOnlySpan<bool>)this).SequenceEqual(other);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Checks other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            HashCode hashCode = new();
            for (int i = 0; i < LOCATIONS; i++)
            {
                hashCode.Add(this[i]);
            }
            return hashCode.ToHashCode();
        }
    }

    private readonly record struct State(int Location, Checks Checks, bool UseLocation)
    {
        /// <inheritdoc />
        public bool Equals(State other) => this.Checks.Equals(other.Checks)
                                        && (!this.UseLocation || this.Location == other.Location);

        /// <inheritdoc />
        public override int GetHashCode() => this.UseLocation
                                                 ? HashCode.Combine(this.Location, this.Checks.GetHashCode())
                                                 : this.Checks.GetHashCode();
    }

    private const char WALL  = '#';
    private const int LOCATIONS = 10;

    private readonly List<Vector2<int>> locations = new(LOCATIONS);
    private readonly Dictionary<UnorderedPair<int>, int> distances = new(LOCATIONS * LOCATIONS);

    /// <summary>
    /// Creates a new <see cref="Day24"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day24(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Get locations from grid
        Span<Vector2<int>?> tempLocations = stackalloc Vector2<int>?[LOCATIONS];
        foreach (int i in ..LOCATIONS)
        {
            Vector2<int> position = this.Grid.PositionOf((char)('0' + i));
            if (position != -Vector2<int>.One)
            {
                tempLocations[i] = position;
            }
        }

        // Discard unused locations
        tempLocations.AsValueEnumerable()
                     .Where(l => l is not null)
                     .ForEach(l => this.locations.Add(l!.Value));

        // Precalculate distances across all locations in grid
        foreach (int i in ..(this.locations.Count - 1))
        {
            Vector2<int> from = this.locations[i];
            foreach (int j in ^i..this.locations.Count)
            {
                Vector2<int> to = this.locations[j];
                int distance = SearchUtils.GetPathLengthBFS(from, to, SearchNeighbours)!.Value;
                this.distances.Add((i, j), distance);
            }
        }

        // Setup locations checks
        Checks startChecks = new();
        startChecks[0] = true;
        Checks endChecks = new();
        endChecks[..this.locations.Count].Fill(true);

        // Setup start/end state
        State startState = new(0, startChecks, false);
        State endState = new(0, endChecks, false);

        // Search for a way to get to all locations
        SearchUtils.Search(startState, endState, null, SearchPath, MinSearchComparer<int>.Comparer, out int pathLength);
        AoCUtils.LogPart1(pathLength);

        // Switch to checking for current location for path completion
        startState = startState with { UseLocation = true };
        endState   = endState with { UseLocation = true };

        // Search again
        SearchUtils.Search(startState, endState, null, SearchPath, MinSearchComparer<int>.Comparer, out pathLength);
        AoCUtils.LogPart2(pathLength);
    }

    private IEnumerable<Vector2<int>> SearchNeighbours(Vector2<int> position) => position.AsAdjacentEnumerable()
                                                                                         .Where(a => this.Grid.TryGetPosition(a, out char e) && e is not WALL);

    private IEnumerable<MoveData<State, int>> SearchPath(State state)
    {
        int check = 0;
        for (int newLocation = 0; newLocation < this.locations.Count; newLocation++)
        {
            // If the location has been checked, we don't need to got here
            if (state.Checks[newLocation]) continue;

            // Mark the location as checked
            check++;
            Checks newChecks = state.Checks;
            newChecks[newLocation] = true;

            // Get distance between current and new location
            int distance = this.distances[(state.Location, newLocation)];

            // Create new state and add move distance
            State newState = state with { Location = newLocation, Checks = newChecks };
            yield return new MoveData<State, int>(newState, distance);
        }

        // If we must return to start location and we have no checks left
        if (state.UseLocation && check is 0)
        {
            // Get distance to start location
            int distance = this.distances[(0, state.Location)];

            // Create final state and add move distance
            State newState = state with { Location = 0 };
            yield return new MoveData<State, int>(newState, distance);
        }
    }

    /// <inheritdoc />
    protected override char[] LineConverter(string line) => line.ToCharArray();
}
