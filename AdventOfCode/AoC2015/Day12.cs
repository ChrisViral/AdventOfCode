using System.Buffers;
using System.Text.Json;
using System.Text.RegularExpressions;
using AdventOfCode.Collections.Pooling.Arrays;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Regexes;
using ZLinq;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 12
/// </summary>
public sealed partial class Day12 : Solver<string>
{
    [GeneratedRegex(@"-?\d+")]
    private static partial Regex NumberMatcher { get; }
    /// <summary>
    /// Creates a new <see cref="Day12"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day12(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int result = NumberMatcher.EnumerateMatches(this.Data)
                                  .Sum(m => int.Parse(this.Data.AsSpan(m.Index, m.Length)));
        AoCUtils.LogPart1(result);

        JsonElement root = JsonDocument.Parse(this.Data).RootElement;
        result = SumObject(root);
        AoCUtils.LogPart2(result);
    }

    // ReSharper disable once CognitiveComplexity
    private int SumObject(JsonElement obj)
    {
        int propertyCount = obj.GetPropertyCount();
        if (propertyCount is 0) return 0;

        int sum = 0;
        using FromArrayPool<JsonProperty> properties = ArrayPool<JsonProperty>.Shared.RentTracked(propertyCount);
        obj.EnumerateObject().AsValueEnumerable().CopyTo(properties.AsSpan);
        if (properties.AsSpan.Where(p => p.Value.ValueKind is JsonValueKind.String)
                      .Any(p => p.Value.GetString() is "red"))
        {
            return 0;
        }

        foreach (JsonElement element in properties.AsSpan.Select(p => p.Value))
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    sum += SumObject(element);
                    break;

                case JsonValueKind.Array:
                    sum += SumArray(element);
                    break;

                case JsonValueKind.Number:
                    sum += element.GetInt32();
                    break;

                case JsonValueKind.Undefined:
                case JsonValueKind.String:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                default:
                    break;
            }
        }

        return sum;
    }

    private int SumArray(JsonElement array)
    {
        int sum = 0;
        foreach (JsonElement element in array.EnumerateArray())
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    sum += SumObject(element);
                    break;

                case JsonValueKind.Array:
                    sum += SumArray(element);
                    break;

                case JsonValueKind.Number:
                    sum += element.GetInt32();
                    break;

                case JsonValueKind.Undefined:
                case JsonValueKind.String:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                default:
                    break;
            }
        }
        return sum;
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => rawInput[0];
}
