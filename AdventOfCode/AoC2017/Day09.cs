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
        private Group? parent;
        private readonly List<Group> children = [];
        private int garbageCount;

        private int? score;
        private int Score => this.score ??= this.parent?.Score + 1 ?? 1;

        public int TotalScore => this.Score + this.children.Sum(c => c.TotalScore);

        public int TotalGarbage => this.garbageCount + this.children.Sum(c => c.TotalGarbage);

        public static Group ParseGroup(ref ReadOnlySpan<char> data)
        {
            Group group = new();
            bool isGarbage = false;
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
                            group.garbageCount++;
                            break;
                    }
                }
                else
                {
                    switch (data[i])
                    {
                        case '{':
                            data = data[i..];
                            Group child = ParseGroup(ref data);
                            child.parent = group;
                            group.children.Add(child);
                            i = -1;
                            break;

                        case '}':
                            data = data[(i + 1)..];
                            return group;

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
