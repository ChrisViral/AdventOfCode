using System.Diagnostics;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 16
/// </summary>
public sealed class Day16 : Solver<Day16.Move[]>
{
    private const int SIZE = 16;
    private const int LOOPS = 1_000_000_000;

    public abstract class Move
    {
        public abstract void ApplyMove(Span<char> programs);
    }

    [DebuggerDisplay("Spin {size}")]
    private sealed class Spin(int size) : Move
    {
        private readonly int size = size;

        public override void ApplyMove(Span<char> programs)
        {
            Span<char> temp = stackalloc char[SIZE];
            programs[^this.size..].CopyTo(temp);
            programs[..^this.size].CopyTo(temp[this.size..]);
            temp.CopyTo(programs);
        }
    }

    [DebuggerDisplay("Exchange {a} <-> {b}")]
    private sealed class Exchange(int a, int b) : Move
    {
        private readonly int a = a;
        private readonly int b = b;

        public override void ApplyMove(Span<char> programs)
        {
            AoCUtils.Swap(ref programs[this.a], ref programs[this.b]);
        }
    }

    [DebuggerDisplay("Partner {a.ToString()} <-> {b.ToString()}")]
    private sealed class Partner(char a, char b) : Move
    {
        private readonly char a = a;
        private readonly char b = b;

        public override void ApplyMove(Span<char> programs)
        {
            int indexA = programs.IndexOf(this.a);
            int indexB = programs.IndexOf(this.b);
            AoCUtils.Swap(ref programs[indexA], ref programs[indexB]);
        }
    }


    /// <summary>
    /// Creates a new <see cref="Day16"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day16(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Span<char> programs = stackalloc char[SIZE];
        StringUtils.ASCII_LOWER.AsSpan(0, SIZE).CopyTo(programs);

        foreach (Move move in this.Data)
        {
            move.ApplyMove(programs);
        }

        AoCUtils.LogPart1(programs.ToString());

        int loops = 0;
        string state = programs.ToString();
        OrderedDictionary<string, int> states = new(100);
        while (states.TryAdd(state, ++loops))
        {
            foreach (Move move in this.Data)
            {
                move.ApplyMove(programs);
            }
            state = programs.ToString();
        }

        int lastSeen = states[state];
        int cycle = loops - lastSeen;
        int finalIndex = LOOPS % cycle;
        string final = states.GetAt(finalIndex - 1).Key;
        AoCUtils.LogPart2(final);
    }

    /// <inheritdoc />
    protected override Move[] Convert(string[] rawInput)
    {
        ReadOnlySpan<char> input = rawInput[0];
        int count = input.Count(',') + 1;
        Span<Range> splits = stackalloc Range[count];
        count = input.Split(splits, ',');
        Move[] moves = new Move[count];
        foreach (int i in ..moves.Length)
        {
            ReadOnlySpan<char> line = input[splits[i]];
            switch (line[0])
            {
                case 's':
                    moves[i] = new Spin(int.Parse(line[1..]));
                    break;

                case 'x':
                    int dashIndex = line.IndexOf('/');
                    moves[i] = new Exchange(int.Parse(line[1..dashIndex]),
                                            int.Parse(line[(dashIndex + 1)..]));
                    break;

                case 'p':
                    moves[i] = new Partner(line[1], line[3]);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown move for {line[0]}");
            }
        }
        return moves;
    }
}
