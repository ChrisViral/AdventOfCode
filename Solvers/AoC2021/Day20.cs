using AdventOfCode.Collections;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 20
/// </summary>
public sealed class Day20 : Solver<(string algorithm, Grid<bool> image)>
{
    private const int PASSES      = 2;
    private const int LONG_PASSES = 50;
    private const char LIGHT      = '#';
    private const int BUFFER      = 6;
    private static readonly Vector2<int> Offset = new(BUFFER / 2, BUFFER / 2);

    /// <summary>
    /// Creates a new <see cref="Day20"/> Solver for 2021 - 20 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day20(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Grid<bool> image = this.Data.image;
        foreach (int i in ..PASSES)
        {
            image = ApplyAlgorithm(image, !i.IsEven);
        }
        AoCUtils.LogPart1(image.Count(b => b));

        foreach (int i in PASSES..LONG_PASSES)
        {
            image = ApplyAlgorithm(image, !i.IsEven);
        }
        AoCUtils.LogPart2(image.Count(b => b));
    }

    private Grid<bool> ApplyAlgorithm(Grid<bool> image, bool externStatus)
    {
        Grid<bool> newImage = new(image.Width + BUFFER, image.Height + BUFFER);
        foreach (Vector2<int> position in Vector2<int>.EnumerateOver(image.Width, image.Height))
        {
            newImage[position + Offset] = image[position];
        }

        foreach (Vector2<int> position in Vector2<int>.EnumerateOver(newImage.Width, newImage.Height))
        {
            int n = 0;
            foreach (Vector2<int> adjacent in position.Adjacent(true, true))
            {
                n <<= 1;
                Vector2<int> matching = adjacent - Offset;
                if (image.WithinGrid(matching) ? image[matching] : externStatus)
                {
                    n |= 1;
                }
            }

            newImage[position] = this.Data.algorithm[n] is LIGHT;
        }

        return newImage;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (string, Grid<bool>) Convert(string[] rawInput)
    {
        int width = rawInput[1].Length;
        int height = rawInput.Length - 1;
        Grid<bool> grid = new(width, height, rawInput[1..], line => line.Select(c => c is LIGHT).ToArray());
        return (rawInput[0], grid);
    }
}
