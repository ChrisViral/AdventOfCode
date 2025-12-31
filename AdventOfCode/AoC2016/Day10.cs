using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Collections;
using AdventOfCode.Utils.Extensions.Enums;
using FastEnumUtility;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 10
/// </summary>
public sealed partial class Day10 : Solver<(ImmutableArray<Day10.Input> inputs, FrozenDictionary<int, Day10.Bot> bots, FrozenDictionary<int, Day10.Output> outputs)>
{
    private enum RecipientType
    {
        BOT,
        OUTPUT
    }

    public readonly record struct Input(int Chip, Bot Handler);

    public abstract class Recipient(int id)
    {
        protected readonly int id = id;

        public abstract void ReceiveChip(int chip);

        public sealed override string ToString() => $"{GetType().Name} {this.id}";
    }

    public sealed class Bot(int id) : Recipient(id)
    {
        private const int LOW_WATCH  = 17;
        private const int HIGH_WATCH = 61;

        public static int WatchID { get; private set; }

        private int? heldChip;

        public Recipient LowRecipient  { get; set; } = null!;

        public Recipient HighRecipient { get; set; } = null!;

        public override void ReceiveChip(int chip)
        {
            if (!this.heldChip.HasValue)
            {
                this.heldChip = chip;
                return;
            }

            int low, high;
            if (this.heldChip > chip)
            {
                low = chip;
                high = this.heldChip.Value;
            }
            else
            {
                low = this.heldChip.Value;
                high = chip;
            }

            if (low is LOW_WATCH && high is HIGH_WATCH)
            {
                WatchID = this.id;
            }

            this.heldChip = null;
            this.LowRecipient.ReceiveChip(low);
            this.HighRecipient.ReceiveChip(high);
        }
    }

    public sealed class Output(int id) : Recipient(id)
    {
        public int Bin { get; private set; }

        public override void ReceiveChip(int chip) => this.Bin = chip;
    }

    [GeneratedRegex(@"^value (\d+) goes to bot (\d+)$")]
    private static partial Regex InputMatcher { get; }

    [GeneratedRegex(@"^bot (\d+) gives low to (bot|output) (\d+) and high to (bot|output) (\d+)$")]
    private static partial Regex BotMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day10(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        foreach ((int chip, Bot recipient) in this.Data.inputs)
        {
            recipient.ReceiveChip(chip);
        }
        AoCUtils.LogPart1(Bot.WatchID);

        int result = this.Data.outputs[0].Bin * this.Data.outputs[1].Bin * this.Data.outputs[2].Bin;
        AoCUtils.LogPart2(result);
    }

    /// <inheritdoc />
    protected override (ImmutableArray<Input>, FrozenDictionary<int, Bot>, FrozenDictionary<int, Output>) Convert(string[] rawInput)
    {
        ImmutableArray<Input>.Builder inputs = ImmutableArray.CreateBuilder<Input>(rawInput.Length);
        Dictionary<int, Bot> bots = new(rawInput.Length);
        Dictionary<int, Output> outputs = new(rawInput.Length);
        foreach (string line in rawInput)
        {
            Match inputMatch = InputMatcher.Match(line);
            if (inputMatch.Success)
            {
                int chip  = int.Parse(inputMatch.Groups[1].ValueSpan);
                int botID = int.Parse(inputMatch.Groups[2].ValueSpan);
                Bot recipient = bots.GetOrCreate(botID, id => new Bot(id));
                inputs.Add(new Input(chip, recipient));
            }
            else
            {
                Match botMatch = BotMatcher.Match(line);
                int handlerID = int.Parse(botMatch.Groups[1].ValueSpan);

                RecipientType lowRecipientType = FastEnum.Parse<RecipientType>(botMatch.Groups[2].ValueSpan, ignoreCase: true);
                int lowRecipientID = int.Parse(botMatch.Groups[3].ValueSpan);

                RecipientType highRecipientType = FastEnum.Parse<RecipientType>(botMatch.Groups[4].ValueSpan, ignoreCase: true);
                int highRecipientID = int.Parse(botMatch.Groups[5].ValueSpan);

                Bot handler = bots.GetOrCreate(handlerID, id => new Bot(id));
                handler.LowRecipient = lowRecipientType switch
                {
                    RecipientType.BOT    => bots.GetOrCreate(lowRecipientID, id => new Bot(id)),
                    RecipientType.OUTPUT => outputs.GetOrCreate(lowRecipientID, id => new Output(id)),
                    _                    => throw lowRecipientType.Invalid()
                };
                handler.HighRecipient = highRecipientType switch
                {
                    RecipientType.BOT    => bots.GetOrCreate(highRecipientID, id => new Bot(id)),
                    RecipientType.OUTPUT => outputs.GetOrCreate(highRecipientID, id => new Output(id)),
                    _                    => throw highRecipientType.Invalid()
                };
            }
        }
        return (inputs.ToImmutable(), bots.ToFrozenDictionary(), outputs.ToFrozenDictionary());
    }
}
