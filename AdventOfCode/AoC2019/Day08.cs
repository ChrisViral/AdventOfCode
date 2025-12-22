using System.ComponentModel;
using AdventOfCode.Collections;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enums;
using SpanLinq;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 08
/// </summary>
public sealed class Day08 : Solver<(Grid<Day08.Colour[]> image, int layerCount)>
{
    /// <summary>
    /// Pixel colour
    /// </summary>
    public enum Colour : byte
    {
        BLACK       = 0,
        WHITE       = 1,
        TRANSPARENT = 2
    }

    /// <summary>
    /// Image width
    /// </summary>
    private const int WIDTH = 25;
    /// <summary>
    /// Image height
    /// </summary>
    private const int HEIGHT = 6;
    /// <summary>
    /// Image layer size
    /// </summary>
    private const int SIZE = WIDTH * HEIGHT;

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
        // Get the best layer index and the count the relevant values on it
        int bestLayer = (..this.Data.layerCount).AsEnumerable().MinBy(i => this.Data.image.Count(p => p[i] is Colour.BLACK));
        int checksum = this.Data.image.Count(p => p[bestLayer] is Colour.WHITE) * this.Data.image.Count(p => p[bestLayer] is Colour.TRANSPARENT);
        AoCUtils.LogPart1(checksum);

        // Just print the image
        AoCUtils.LogPart2("\n" + this.Data.image);
    }

    /// <summary>
    /// Renders a given image pixel
    /// </summary>
    /// <param name="pixel">Pixel to render</param>
    /// <returns>The string representation of this pixel</returns>
    /// <exception cref="InvalidEnumArgumentException">For invalid <see cref="Colour"/> values</exception>
    private static string RenderPixel(Colour[] pixel)
    {
        Colour top = pixel.AsSpan().FirstOrDefault(c => c is not Colour.TRANSPARENT, Colour.TRANSPARENT);
        return top switch
        {
            Colour.BLACK       => "░",
            Colour.WHITE       => "▓",
            Colour.TRANSPARENT => " ",
            _                  => throw top.Invalid()
        };
    }

    /// <inheritdoc />
    protected override (Grid<Colour[]> image, int layerCount) Convert(string[] rawInput)
    {
        // Create grid
        ReadOnlySpan<char> line = rawInput[0];
        int layerCount = line.Length / SIZE;
        Grid<Colour[]> image = new(WIDTH, HEIGHT, RenderPixel);

        // Fill grid with image data
        int pixelIndex = 0;
        foreach (Vector2<int> position in Vector2<int>.EnumerateOver(WIDTH, HEIGHT))
        {
            int sourceIndex = pixelIndex++;
            Colour[] pixel = new Colour[layerCount];
            foreach (int i in ..layerCount)
            {
                pixel[i]    =  (Colour)(line[sourceIndex] - '0');
                sourceIndex += SIZE;
            }
            image[position] = pixel;
        }
        return (image, layerCount);
    }
}
