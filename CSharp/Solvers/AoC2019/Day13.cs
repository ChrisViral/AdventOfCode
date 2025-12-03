using System;
using System.Collections.Generic;
using AdventOfCode.Collections;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Solvers.Specialized;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 13
/// </summary>
public class Day13 : IntcodeSolver
{
    private enum Blocks
    {
        EMPTY  = 0,
        WALL   = 1,
        BLOCK  = 2,
        PADDLE = 3,
        BALL   = 4
    }

    /// <summary>
    /// Intcode Arcade game
    /// </summary>
    private class Arcade : ConsoleView<Blocks>
    {
            /// <summary>
        /// ID to character mapping
        /// </summary>
        private static readonly Dictionary<Blocks, char> toChar = new(5)
        {
            [Blocks.EMPTY]  = ' ',
            [Blocks.WALL]   = '▓',
            [Blocks.BLOCK]  = '░',
            [Blocks.PADDLE] = '═',
            [Blocks.BALL]   = 'O'
        };
    
            private readonly IntcodeVM software;
    
            private int Score { get; set; }
    
            /// <summary>
        /// Creates and setups a new Arcade from the given software
        /// </summary>
        /// <param name="width">Width of the view</param>
        /// <param name="height">Height of the view</param>
        /// <param name="software">Software to run the arcade on</param>
        public Arcade(int width, int height, IntcodeVM software) : base(width, height, b => toChar[b], Anchor.TOP_LEFT)
        {
            this.software = software;
        }
    
            /// <summary>
        /// Play the game until it's over and display the screen on the console
        /// </summary>
        /// <returns>The final score of the game</returns>
        public int Play()
        {
            //Hide cursor during play
            Console.CursorVisible = false;
            //Setup
            long ball = 0L, paddle = 0L;
            this.software[0] = 2L;
            while (!this.software.IsHalted)
            {
                this.software.Run();
                while (this.software.HasOutputs)
                {
                    (long x, long y, long id) = this.software;
                    if (x is -1L && y is 0L)
                    {
                        this.Score = (int)id;
                    }
                    else
                    {
                        Blocks type = (Blocks)id;
                        this[(int)x, (int)y] = type;
                        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                        switch (type)
                        {
                            case Blocks.BALL:
                                ball = x;
                                break;
                            case Blocks.PADDLE:
                                paddle = x;
                                break;
                        }
                    }
                }

                //Display
                PrintToConsole();
                //Handle input for next move
                this.software.AddInput(ball.CompareTo(paddle));
            }

            //Show cursor again
            Console.CursorVisible = true;
            //Return the final score
            return this.Score;
        }

        /// <inheritdoc cref="object.ToString"/>
        public override void PrintToConsole(string? message = null) => base.PrintToConsole("              Score: " + this.Score);
        }

    /// <summary>
    /// Creates a new <see cref="Day13"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
    public Day13(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int width = 0;
        int height = 0;
        int blocks = 0;
        this.VM.Run();
        while (this.VM.HasOutputs)
        {
            (long x, long y, long id) = this.VM;
            if (id is (long)Blocks.BLOCK)
            {
                blocks++;
            }
            width = Math.Max(width, (int)x);
            height = Math.Max(height, (int)y);
        }
        AoCUtils.LogPart1(blocks);

        this.VM.Reset();
        Arcade arcade = new(width + 1, height + 1, this.VM);
        AoCUtils.LogPart2(arcade.Play());
    }
}
