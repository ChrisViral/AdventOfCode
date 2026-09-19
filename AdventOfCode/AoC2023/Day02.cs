using System.Text.RegularExpressions;
using Challenge.Solvers;
using Challenge.Utils;
using Challenge.Utils.Extensions.Ranges;
using Microsoft.Extensions.Logging;
using ZLinq;

namespace AdventOfCode.AoC2023;

/// <summary>
/// Solver for 2023 Day 02
/// </summary>
[Solver(2023, 2)]
public sealed partial class Day02 : Solver<Day02.Game[]>
{
    public readonly partial struct Set
    {
        [GeneratedRegex(@"(\d+) (red|green|blue)")]
        private static partial Regex SetMatch { get; }

        public int Red { get; private init; }
        public int Green { get; private init; }
        public int Blue { get; private init; }

        public Set(string turn)
        {
            foreach (Match match in SetMatch.Matches(turn))
            {
                int amount = int.Parse(match.Groups[1].Value);
                switch (match.Groups[2].Value)
                {
                    case "red":
                        this.Red = amount;
                        break;

                    case "green":
                        this.Green = amount;
                        break;

                    case "blue":
                        this.Blue = amount;
                        break;
                }
            }
        }

        public Set(int red, int green, int blue)
        {
            this.Red   = red;
            this.Green = green;
            this.Blue  = blue;
        }

        public bool IsValid(in Set maxSet) => this.Red <= maxSet.Red && this.Green <= maxSet.Green && this.Blue <= maxSet.Blue;

        public void ReduceSet(ref Set minimalSet)
        {
            if (minimalSet.Red < this.Red)
            {
                minimalSet = minimalSet with { Red = this.Red };
            }

            if (minimalSet.Green <this.Green)
            {
                minimalSet = minimalSet with { Green = this.Green };
            }

            if (minimalSet.Blue < this.Blue)
            {
                minimalSet = minimalSet with { Blue = this.Blue };
            }
        }
    }

    public readonly struct Game
    {
        public readonly int id;
        public readonly Set[] turns;

        public int Power
        {
            get
            {
                Set minimalSet = new(0, 0, 0);
                Array.ForEach(this.turns, t => t.ReduceSet(ref minimalSet));
                return minimalSet.Red * minimalSet.Green * minimalSet.Blue;
            }
        }

        public Game(int id, string allTurns)
        {
            this.id = id;
            string[] stringTurns = allTurns.Split(';', DEFAULT_OPTIONS);
            this.turns = new Set[stringTurns.Length];
            foreach (int i in ..this.turns.Length)
            {
                this.turns[i] = new Set(stringTurns[i]);
            }
        }

        public bool IsValid(Set maxSet) => Array.TrueForAll(this.turns, t => t.IsValid(maxSet));
    }

    [GeneratedRegex(@"Game (\d+): ([\w\s,;]+)")]
    private static partial Regex GameMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day02"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Game"/>[] fails</exception>
    public Day02(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Set maxSet = new(12, 13, 14);
        int sum = this.Data.Where(g => g.IsValid(maxSet)).Sum(g => g.id);
        LogAnswer(sum);

        int powers = this.Data.Sum(g => g.Power);
        LogAnswer(powers);
    }

    /// <inheritdoc />
    protected override Game[] Convert(string[] rawInput) => RegexFactory<Game>.ConstructObjects(GameMatcher, rawInput);
}
