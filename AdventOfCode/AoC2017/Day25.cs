using System.Collections.Frozen;
using System.Diagnostics;
using AdventOfCode.Collections;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enumerables;
using AdventOfCode.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 25
/// </summary>
public sealed class Day25 : Solver<(char start, int steps, FrozenDictionary<char, Day25.State> states)>
{
    [DebuggerDisplay("Write {Write}, Move {Move == 1 ? \"right\" : \"left\"}, Next {Next}")]
    public sealed class Action(ReadOnlySpan<string> input)
    {
        public bool Write { get; } = input[0][^2] is '1';

        public int Move { get; } = input[1].EndsWith("right.") ? 1 : -1;

        public char Next { get; } = input[2][^2];
    }

    [DebuggerDisplay("State: {ID}")]
    public sealed class State(ReadOnlySpan<string> input) : IEquatable<State>
    {
        public char ID { get; } = input[0][^2];

        public Action FalseAction { get; } = new(input[2..5]);

        public Action TrueAction { get;  } = new(input[6..]);

        /// <inheritdoc />
        public bool Equals(State? other) => other is not null && this.ID == other.ID;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is State other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => this.ID.GetHashCode();
    }

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
        int position = 0;
        DefaultDictionary<int, bool> tape = new(1000, false);
        State state = this.Data.states[this.Data.start];
        foreach (int _ in ..this.Data.steps)
        {
            Action action = tape[position] ? state.TrueAction : state.FalseAction;
            tape[position] = action.Write;
            position += action.Move;
            state = this.Data.states[action.Next];
        }
        AoCUtils.LogPart1(tape.Values.AsValueEnumerable().Count(true));
    }

    /// <inheritdoc />
    protected override (char, int, FrozenDictionary<char, State>) Convert(string[] rawInput)
    {
        ReadOnlySpan<string> input = rawInput;
        char start = input[0][^2];
        int steps = int.Parse(input[1][36..^7]);

        int stateCount = (input.Length - 2) / 9;
        Dictionary<char, State> states = new(stateCount);
        for (int i = 2; i < input.Length; i += 9)
        {
            State state = new(input.Slice(i, 9));
            states.Add(state.ID, state);
        }
        return (start, steps, states.ToFrozenDictionary());
    }
}
