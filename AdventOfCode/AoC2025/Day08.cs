using AdventOfCode.Collections;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2025;

/// <summary>
/// Solver for 2025 Day 08
/// </summary>
public sealed class Day08 : ArraySolver<Day08.Junction>
{
    /// <summary>
    /// Part 1 connection count
    /// </summary>
    private const int CONNECTION_COUNT = 1000;

    /// <summary>
    /// Junction struct
    /// </summary>
    /// <param name="Position">Junction position</param>
    public readonly record struct Junction(Vector3<int> Position);

    /// <summary>
    /// Connection struct
    /// </summary>
    /// <param name="A">First junction connection</param>
    /// <param name="B">Second junction connection</param>
    private readonly record struct Connection(Junction A, Junction B) : IComparable<Connection>
    {
        /// <summary>
        /// Distance between both junctions of this connection
        /// </summary>
        private double Distance { get; } = Vector3<int>.Distance(A.Position, B.Position);

        /// <inheritdoc />
        public int CompareTo(Connection other) => this.Distance.CompareTo(other.Distance);
    }

    /// <summary>
    /// Circuit container
    /// </summary>
    private sealed class Circuit : HashSet<Connection>;

    /// <summary>
    /// Creates a new <see cref="Day08"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day08(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // List out possible connections
        PriorityQueue<Connection> possibleConnections = new(this.Data.Length.PreviousTriangular);
        foreach (int i in ..(this.Data.Length - 1))
        {
            Junction a = this.Data[i];
            foreach (int j in (i + 1)..this.Data.Length)
            {
                possibleConnections.Enqueue(new Connection(a, this.Data[j]));
            }
        }

        // Do first 1000 joins
        int count = 0;
        Dictionary<Junction, Circuit> circuits = new(this.Data.Length);
        while (count++ < CONNECTION_COUNT && possibleConnections.TryDequeue(out Connection connection))
        {
            JoinCircuits(connection, circuits);
        }

        // Get result
        int result = circuits.Values
                             .Distinct()
                             .OrderByDescending(c => c.Count)
                             .Take(3)
                             .Multiply(c => c.Count + 1);
        AoCUtils.LogPart1(result);

        // Keep joining until everything is merged
        int mergedCount = this.Data.Length - 1;
        while (possibleConnections.TryDequeue(out Connection connection))
        {
            if (JoinCircuits(connection, circuits) && circuits[connection.A].Count == mergedCount)
            {
                result = connection.A.Position.X * connection.B.Position.X;
                break;
            }
        }
        AoCUtils.LogPart2(result);
    }

    /// <summary>
    /// Joins two circuits together at a connection, if possible
    /// </summary>
    /// <param name="connection">Connection to create between circuits</param>
    /// <param name="circuits">Circuits map</param>
    /// <returns><see langword="true"/> if a connection was created, otherwise <see langword="false"/></returns>
    private static bool JoinCircuits(in Connection connection, Dictionary<Junction, Circuit> circuits)
    {
        // Get circuits on both ends
        circuits.TryGetValue(connection.A, out Circuit? aCircuit);
        circuits.TryGetValue(connection.B, out Circuit? bCircuit);

        if (aCircuit is null)
        {
            if (bCircuit is null)
            {
                // Neither end is in a circuit
                Circuit newCircuit = [connection];
                circuits[connection.A] = newCircuit;
                circuits[connection.B] = newCircuit;
            }
            else
            {
                // Add A to B's circuit
                bCircuit.Add(connection);
                circuits[connection.A] = bCircuit;
            }
            return true;
        }

        if (bCircuit is null)
        {
            // Add B to A's circuit
            aCircuit.Add(connection);
            circuits[connection.B] = aCircuit;
            return true;
        }

        if (aCircuit != bCircuit)
        {
            // Put largest circuit into A
            if (aCircuit.Count < bCircuit.Count)
            {
                AoCUtils.Swap(ref aCircuit, ref bCircuit);
            }

            // Merge circuits
            aCircuit.Add(connection);
            aCircuit.UnionWith(bCircuit);
            foreach (Connection bConnection in bCircuit)
            {
                circuits[bConnection.A] = aCircuit;
                circuits[bConnection.B] = aCircuit;
            }
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override Junction ConvertLine(string line) => new(Vector3<int>.Parse(line));
}
