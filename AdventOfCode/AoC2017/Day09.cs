using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2018 Day 09
/// </summary>
public sealed class Day09 : Solver<string>
{
    private sealed class Group
    {
        private readonly List<Group> children;

        public int TotalScore { get; }

        public int TotalGarbage { get; }

        private Group(int depth, int garbage, List<Group> children)
        {
            this.children     = children;
            this.TotalScore   = depth + this.children.Sum(c => c.TotalScore);
            this.TotalGarbage = garbage + this.children.Sum(c => c.TotalGarbage);
        }

        // ReSharper disable once CognitiveComplexity
        public static Group ParseGroup(ref ReadOnlySpan<char> data, int depth = 1)
        {
            int garbage = 0;
            bool isGarbage = false;
            List<Group> children = [];
            for (int i = 1; i < data.Length; i++)
            {
                if (isGarbage)
                {
                    switch (data[i])
                    {
                        case '!':
                            i++;
                            break;

                        case '>':
                            isGarbage = false;
                            break;

                        default:
                            garbage++;
                            break;
                    }
                }
                else
                {
                    switch (data[i])
                    {
                        case '{':
                            data = data[i..];
                            children.Add(ParseGroup(ref data, depth + 1));
                            i = -1;
                            break;

                        case '}':
                            data = data[(i + 1)..];
                            return new Group(depth, garbage, children);

                        case '<':
                            isGarbage = true;
                            break;

                        case ',':
                            break;

                        default:
                            throw new InvalidOperationException($"Unexpected non-garbage character : {data[i]}");
                    }
                }
            }

            throw new InvalidOperationException("Unterminated group");
        }

        public override string ToString() => $"{{{string.Join(',', this.children)}}}";
    }

    /// <summary>
    /// Creates a new <see cref="Day09"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day09(string input) : base(input) { }

    /// <inheritdoc />
    public override void Run()
    {
        ReadOnlySpan<char> data = this.Data;
        Group root = Group.ParseGroup(ref data);
        AoCUtils.LogPart1(root.TotalScore);
        AoCUtils.LogPart2(root.TotalGarbage);
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => rawInput[0];
}
