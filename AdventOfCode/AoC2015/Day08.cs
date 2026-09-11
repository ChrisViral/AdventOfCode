using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using Microsoft.CodeAnalysis.CSharp;
using ZLinq;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 08
/// </summary>
public sealed class Day08 : ArraySolver<string>
{
    /// <summary>
    /// Creates a new <see cref="Day08"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day08(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int codeLength = this.Data.AsValueEnumerable().Sum(l => l.Length);
        int valueLength = this.Data.AsValueEnumerable().Sum(CalculateValueLength);
        AoCUtils.LogPart1(codeLength - valueLength);

        int escapedLength = this.Data.AsValueEnumerable().Sum(l => SymbolDisplay.FormatLiteral(l, true).Length);
        AoCUtils.LogPart2(escapedLength - codeLength);
    }

    private static int CalculateValueLength(string code)
    {
        int length = code.Length - 2;
        for (int i = code.IndexOf('\\'); i is not -1; i = code.IndexOf('\\', i))
        {
            int escaped = code[i + 1] switch
            {
                '"'  => 1,
                '\\' => 1,
                'x'  => 3,
                _    => 0
            };
            length -= escaped;
            i += escaped + 1;
        }
        return length;
    }

    /// <inheritdoc />
    protected override string ConvertLine(string line) => line;
}
