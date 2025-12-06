using System;
using System.ComponentModel;
using AdventOfCode.Collections;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 13
/// </summary>
public sealed class Day13 : IntcodeSolver
{
    /// <summary>
    /// Arcade object
    /// </summary>
    private enum ArcadeObject
    {
        EMPTY  = 0,
        WALL   = 1,
        BLOCK  = 2,
        PADDLE = 3,
        BALL   = 4
    }

    /// <summary>
    /// Position of score output
    /// </summary>
    private static readonly Vector2<int> ScorePos = (-1, 0);    /// <summary>
    /// Creates a new <see cref="Day13"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
    public Day13(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int maxX = 0;
        int maxY = 0;
        int blocks = 0;
        this.VM.Run();
        while (!this.VM.Output.IsEmpty)
        {
            maxX = Math.Max(maxX, (int)this.VM.Output.GetOutput());
            maxY = Math.Max(maxY, (int)this.VM.Output.GetOutput());

            // We're only interested in the blocks
            if (this.VM.Output.GetOutput() == (long)ArcadeObject.BLOCK)
            {
                blocks++;
            }
        }
        AoCUtils.LogPart1(blocks);

        // Reset and "insert quarters"
        this.VM.Reset();
        this.VM[0] = 2L;

        // Create console view
        ConsoleView<ArcadeObject> arcade = new(maxX + 1, maxY + 1, ShowObject, Anchor.TOP_LEFT, fps: 60);

        // Keep score and position
        int score = 0;
        Vector2<int> ballPos   = Vector2<int>.Zero;
        Vector2<int> paddlePos = Vector2<int>.Zero;
        do
        {
            this.VM.Run();
            while (!this.VM.Output.IsEmpty)
            {
                // Get position
                int x = (int)this.VM.Output.GetOutput();
                int y = (int)this.VM.Output.GetOutput();
                Vector2<int> position = (x, y);

                // Check if we're receiving the score
                if (position == ScorePos)
                {
                    score = (int)this.VM.Output.GetOutput();
                    continue;
                }

                // Set position
                ArcadeObject currentObject = (ArcadeObject)this.VM.Output.GetOutput();
                arcade[position] = currentObject;

                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (currentObject)
                {
                    case ArcadeObject.BALL:
                        ballPos = position;
                        break;

                    case ArcadeObject.PADDLE:
                        paddlePos = position;
                        break;
                }
            }

            // Move towards ball
            this.VM.Input.AddInput(ballPos.X.CompareTo(paddlePos.X));

            arcade.PrintToConsole($"Score: {score}");
        }
        while (!this.VM.IsHalted);

        AoCUtils.LogPart2(score);
    }

    /// <summary>
    /// Gives character representation of arcade objects
    /// </summary>
    /// <param name="arcadeObject">Arcade object to show</param>
    /// <returns>The character representation of the object</returns>
    /// <exception cref="InvalidEnumArgumentException">For invalid values of <paramref name="arcadeObject"/></exception>
    private static char ShowObject(ArcadeObject arcadeObject) => arcadeObject switch
    {
        ArcadeObject.EMPTY  => ' ',
        ArcadeObject.WALL   => '▓',
        ArcadeObject.BLOCK  => '░',
        ArcadeObject.PADDLE => '_',
        ArcadeObject.BALL   => 'O',
        _                   => throw new InvalidEnumArgumentException(nameof(arcadeObject), (int)arcadeObject, typeof(ArcadeObject))
    };}
