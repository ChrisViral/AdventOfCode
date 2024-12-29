﻿using System;
using System.ComponentModel;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using SpanLinq;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 08
/// </summary>
public class Day08 : Solver<(Grid<Day08.Colour[]> image, int layerCount)>
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

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day08"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day08(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
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
            _                  => throw new InvalidEnumArgumentException(nameof(top), (int)top, typeof(Colour))
        };
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Grid<Colour[]> image, int layerCount) Convert(string[] rawInput)
    {
        // Create grid
        ReadOnlySpan<char> line = rawInput[0];
        int layerCount = line.Length / SIZE;
        Grid<Colour[]> image = new(WIDTH, HEIGHT, RenderPixel);

        // Fill grid with image data
        int pixelIndex = 0;
        foreach (Vector2<int> position in Vector2<int>.Enumerate(WIDTH, HEIGHT))
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
    #endregion
}