using System.Diagnostics;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2024;

/// <summary>
/// Solver for 2024 Day 24
/// </summary>
public sealed class Day24 : Solver<Day24.Wire[]>
{
    /// <summary>
    /// Wire base
    /// </summary>
    /// <param name="id">Wire ID</param>
    public abstract class Wire(string id) : IEquatable<Wire>
    {
        /// <summary>
        /// Wire id
        /// </summary>
        public string ID { get; } = id;

        protected bool? value;

        /// <summary>
        /// Wire value
        /// </summary>
        public bool Value => this.value ??= Evaluate();

        /// <summary>
        /// Evaluates the value of the wire
        /// </summary>
        /// <returns>Value of the wire</returns>
        protected abstract bool Evaluate();

        /// <inheritdoc />
        public bool Equals(Wire? other) => this.ID == other?.ID;

        /// <inheritdoc />
        public sealed override bool Equals(object? obj) => obj is Wire wire && Equals(wire);

        /// <inheritdoc />
        public sealed override int GetHashCode() => this.ID.GetHashCode();

        public sealed override string ToString() => $"[{GetType().Name}] {this.ID}";
    }

    /// <summary>
    /// Constant value wire
    /// </summary>
    private sealed class ConstantWire : Wire
    {
        /// <summary>
        /// Creates a constant value wire
        /// </summary>
        /// <param name="id">Wire ID</param>
        /// <param name="value">Wire's value</param>
        public ConstantWire(string id, bool value) : base(id) => this.value = value;

        /// <inheritdoc />
        protected override bool Evaluate() => this.value!.Value;
    }

    /// <summary>
    /// Gate wire base
    /// </summary>
    /// <param name="id">Wire ID</param>
    private abstract class GateWire(string id) : Wire(id)
    {
        /// <summary>
        /// Left gate wire
        /// </summary>
        public Wire Left { get; set; } = null!;

        /// <summary>
        /// Right gate wire
        /// </summary>
        public Wire Right { get; set; } = null!;
    }

    /// <summary>
    /// And gate wire
    /// </summary>
    /// <param name="id">Wire ID</param>
    private sealed class AndWire(string id) : GateWire(id)
    {
        /// <inheritdoc />
        protected override bool Evaluate() => this.Left.Value & this.Right.Value;
    }

    /// <summary>
    /// Or gate wire
    /// </summary>
    /// <param name="id">Wire ID</param>
    private sealed class OrWire(string id) : GateWire(id)
    {
        /// <inheritdoc />
        protected override bool Evaluate() => this.Left.Value | this.Right.Value;
    }

    /// <summary>
    /// Xor gate wire
    /// </summary>
    /// <param name="id">Wire ID</param>
    private sealed class XorWire(string id) : GateWire(id)
    {
        /// <inheritdoc />
        protected override bool Evaluate() => this.Left.Value ^ this.Right.Value;
    }

    /// <summary>
    /// Creates a new <see cref="Day24"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Wire"/>[] fails</exception>
    public Day24(string input) : base(input, options: StringSplitOptions.TrimEntries) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Get z wires
        Wire[] zWires = this.Data.Where(w => w.ID[0] is 'z')
                            .OrderByDescending(w => w.ID)
                            .ToArray();

        // Calculate outputted value
        long number = 0L;
        foreach (Wire wire in zWires)
        {
            number <<= 1;
            if (wire.Value)
            {
                number |= 1;
            }
        }
        AoCUtils.LogPart1(number);

        // Prepare invalid gates set
        HashSet<GateWire> invalidWires = new(8);
        GateWire[] gates = this.Data.Where(w => w is GateWire)
                               .Cast<GateWire>()
                               .ToArray();

        // Loop through all gates
        foreach (GateWire wire in gates)
        {
            switch (wire)
            {
                case XorWire when wire.ID[0] is not 'z'
                               && wire.Left.ID[0] is not ('x' or 'y')
                               && wire.Right.ID[0] is not ('x' or 'y'):
                    invalidWires.Add(wire);
                    break;

                case XorWire when gates.Where(g => g is OrWire).Any(g => g.Left.Equals(wire) || g.Right.Equals(wire)):
                    invalidWires.Add(wire);
                    break;

                case not XorWire when wire.ID[0] is 'z' && wire.ID is not "z45":
                    invalidWires.Add(wire);
                    break;

                case AndWire when wire.Left.ID is not "x00"
                               && wire.Right.ID is not "x00"
                               && gates.Where(g => g is not OrWire).Any(g => g.Left.Equals(wire) || g.Right.Equals(wire)):
                    invalidWires.Add(wire);
                    break;
            }
        }

        AoCUtils.LogPart2(string.Join(',', invalidWires.Select(w => w.ID).Order()));
    }

    /// <inheritdoc />
    protected override Wire[] Convert(string[] rawInput)
    {
        Dictionary<string, Wire> wires = new(rawInput.Length);

        // Create all constant wires
        int constantsEnd = rawInput.IndexOf(string.Empty);
        Span<string> constants = rawInput.AsSpan(..constantsEnd);
        foreach (ReadOnlySpan<char> constant in constants)
        {
            string id = constant[..3].ToString();
            ConstantWire constantWire = new(id, constant[5] is '1');
            wires.Add(id, constantWire);
        }

        // Create all gate wires
        Span<string> gates = rawInput.AsSpan((constantsEnd + 1)..^1);
        foreach (ReadOnlySpan<char> gate in gates)
        {
            ReadOnlySpan<char> type = gate.Slice(4, 3).TrimEnd();
            string id = gate[14..].TrimStart().ToString();
            GateWire gateWire = type switch
            {
                "OR"  => new OrWire(id),
                "AND" => new AndWire(id),
                "XOR" => new XorWire(id),
                _     => throw new UnreachableException("Unknown gate type")
            };
            wires.Add(id, gateWire);
        }

        var wiresLookup = wires.GetAlternateLookup<ReadOnlySpan<char>>();

        // Hook up all gate wires
        foreach (ReadOnlySpan<char> gate in gates)
        {
            ReadOnlySpan<char> leftId  = gate[..3];
            ReadOnlySpan<char> rightId = gate.Slice(7, 4).Trim();
            ReadOnlySpan<char> gateId  = gate[14..].TrimStart();

            Wire leftWire  = wiresLookup[leftId];
            Wire rightWire = wiresLookup[rightId];
            GateWire gateWire = (GateWire)wiresLookup[gateId];

            gateWire.Left  = leftWire;
            gateWire.Right = rightWire;
        }

        return wires.Values.ToArray();
    }
}
