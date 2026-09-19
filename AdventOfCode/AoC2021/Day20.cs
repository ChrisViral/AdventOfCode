using Challenge.Collections;
using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Utils.Extensions.Numbers;
using Challenge.Utils.Extensions.Ranges;
using Microsoft.Extensions.Logging;
using ZLinq;

namespace AdventOfCode.AoC2021;

/// <summary>
/// Solver for 2021 Day 20
/// </summary>
[Solver(2021, 20)]
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
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day20(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Grid<bool> image = this.Data.image;
        foreach (int i in ..PASSES)
        {
            image = ApplyAlgorithm(image, !i.IsEven);
        }
        LogAnswer(image.Count(b => b));

        foreach (int i in PASSES..LONG_PASSES)
        {
            image = ApplyAlgorithm(image, !i.IsEven);
        }
        LogAnswer(image.Count(b => b));
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

    /// <inheritdoc />
    protected override (string, Grid<bool>) Convert(string[] rawInput)
    {
        int width = rawInput[1].Length;
        int height = rawInput.Length - 1;
        Grid<bool> grid = new(width, height, rawInput[1..], line => line.Select(c => c is LIGHT).ToArray());
        return (rawInput[0], grid);
    }
}
