using System.Text.RegularExpressions;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enumerables;
using AdventOfCode.Utils.Extensions.Regexes;
using ZLinq;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2016 Day 07
/// </summary>
public sealed partial class Day07 : Solver<Dictionary<string, Day07.LogicGate>>
{
    public abstract class LogicGate
    {
        protected readonly string outputWire;
        protected readonly string firstInput;
        private readonly ushort? firstValue;
        protected readonly string secondInput;
        private readonly ushort? secondValue;
        private ushort? value;

        protected LogicGate(string output, string first, string second)
        {
            this.outputWire = output;
            this.firstInput = first;
            this.secondInput = second;

            if (ushort.TryParse(first, out ushort result))
            {
                this.firstValue = result;
            }
            if (ushort.TryParse(second, out result))
            {
                this.secondValue = result;
            }
        }

        public ushort Evaluate(Dictionary<string, LogicGate> gates)
        {
            this.value ??= EvaluateInternal(gates);
            return this.value.Value;
        }

        public void Reset() => this.value = null;

        protected abstract ushort EvaluateInternal(Dictionary<string, LogicGate> gates);

        protected ushort GetFirstValue(Dictionary<string, LogicGate> gates) => this.firstValue ?? gates[this.firstInput].Evaluate(gates);

        protected ushort GetSecondValue(Dictionary<string, LogicGate> gates) => this.secondValue ?? gates[this.secondInput].Evaluate(gates);
    }

    private sealed class SetGate(string output, string input) : LogicGate(output, input, string.Empty)
    {
        protected override ushort EvaluateInternal(Dictionary<string, LogicGate> gates) => GetFirstValue(gates);

        public override string ToString() => $"{this.firstInput} -> {this.outputWire}";
    }

    private sealed class NotGate(string output, string input) : LogicGate(output, input, string.Empty)
    {
        protected override ushort EvaluateInternal(Dictionary<string, LogicGate> gates) => unchecked((ushort)~GetFirstValue(gates));

        public override string ToString() => $"NOT {this.firstInput} -> {this.outputWire}";
    }

    private sealed class AndGate(string output, string first, string second) : LogicGate(output, first, second)
    {
        protected override ushort EvaluateInternal(Dictionary<string, LogicGate> gates) => unchecked((ushort)(GetFirstValue(gates) & GetSecondValue(gates)));

        public override string ToString() => $"{this.firstInput} AND {this.secondInput} -> {this.outputWire}";
    }

    private sealed class OrGate(string output, string first, string second) : LogicGate(output, first, second)
    {
        protected override ushort EvaluateInternal(Dictionary<string, LogicGate> gates) => unchecked((ushort)(GetFirstValue(gates) | GetSecondValue(gates)));

        public override string ToString() => $"{this.firstInput} OR {this.secondInput} -> {this.outputWire}";
    }

    private sealed class LeftShiftGate(string output, string first, string second) : LogicGate(output, first, second)
    {
        protected override ushort EvaluateInternal(Dictionary<string, LogicGate> gates) => unchecked((ushort)(GetFirstValue(gates) << GetSecondValue(gates)));

        public override string ToString() => $"{this.firstInput} LSHIFT {this.secondInput} -> {this.outputWire}";
    }

    private sealed class RightShiftGate(string output, string first, string second) : LogicGate(output, first, second)
    {
        protected override ushort EvaluateInternal(Dictionary<string, LogicGate> gates) => unchecked((ushort)(GetFirstValue(gates) >> GetSecondValue(gates)));

        public override string ToString() => $"{this.firstInput} RSHIFT {this.secondInput} -> {this.outputWire}";
    }

    [GeneratedRegex(@"(?:([a-z\d]+) )?(?:([A-Z]+) )?([a-z\d]+) -> ([a-z]+)")]
    private static partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day07"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day07(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Get value at gate A
        LogicGate finalGate = this.Data["a"];
        ushort value = finalGate.Evaluate(this.Data);
        string result = value.ToString();
        AoCUtils.LogPart1(result);

        // Reset wires
        this.Data.Values.ForEach(g => g.Reset());
        // Set gate B to have value from first run
        SetGate newGate = new("b", result);
        this.Data["b"] = newGate;

        // Get value at gate A
        value = finalGate.Evaluate(this.Data);
        AoCUtils.LogPart2(value);
    }

    /// <inheritdoc />
    protected override Dictionary<string, LogicGate> Convert(string[] rawInput)
    {
        Dictionary<string, LogicGate> gates = new(rawInput.Length);
        Group[] capturesBuffer = new Group[4];
        foreach (string line in rawInput)
        {
            int count = Matcher.Match(line).CapturedGroups.CopyTo(capturesBuffer);
            string output = capturesBuffer[count - 1].Value;
            LogicGate gate = count switch
            {
                2 => new SetGate(output, capturesBuffer[0].Value),
                3 => new NotGate(output, capturesBuffer[1].Value),
                4 => capturesBuffer[1].Value switch
                {
                    "AND"    => new AndGate(output, capturesBuffer[0].Value, capturesBuffer[2].Value),
                    "OR"     => new OrGate(output, capturesBuffer[0].Value, capturesBuffer[2].Value),
                    "LSHIFT" => new LeftShiftGate(output, capturesBuffer[0].Value, capturesBuffer[2].Value),
                    "RSHIFT" => new RightShiftGate(output, capturesBuffer[0].Value, capturesBuffer[2].Value),
                    _        => throw new InvalidOperationException("Unknown gate type")
                },
                _ => throw new InvalidOperationException("Unknown gate type")
            };
            gates.Add(output, gate);
        }
        return gates;
    }
}
