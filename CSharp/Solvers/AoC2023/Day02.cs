using System;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 02
/// </summary>
public class Day02 : Solver<Day02.Game[]>
{
    public readonly struct Set
    {
        private const string TURN_PATTERN = @"(\d+) (red|green|blue)";
        private static readonly Regex SetMatch = new(TURN_PATTERN, RegexOptions.Compiled);

        public int Red { get; init; }
        public int Green { get; init; }
        public int Blue { get; init; }

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
                this.turns[i] = new(stringTurns[i]);
            }
        }

        public bool IsValid(Set maxSet) => Array.TrueForAll(this.turns, t => t.IsValid(maxSet));
    }

    public const string GAME_PATTERN = @"Game (\d+): ([\w\s,;]+)";

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day02"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Game"/>[] fails</exception>
    public Day02(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        Set maxSet = new(12, 13, 14);
        int sum = this.Data.Where(g => g.IsValid(maxSet)).Sum(g => g.id);
        AoCUtils.LogPart1(sum);

        int powers = this.Data.Sum(g => g.Power);
        AoCUtils.LogPart2(powers);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Game[] Convert(string[] rawInput) => RegexFactory<Game>.ConstructObjects(GAME_PATTERN, rawInput, RegexOptions.Compiled);
    #endregion
}