using System;
using System.Text;
using System.Threading;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Solvers.Specialized;

namespace AdventOfCode.Solvers.AoC2019
{
    /// <summary>
    /// Solver for 2019 Day 13
    /// </summary>
    public class Day13 : IntcodeSolver
    {
        /// <summary>
        /// Intcode Arcade game
        /// </summary>
        public class Arcade
        {
            #region Constants
            /// <summary>
            /// Block ID
            /// </summary>
            private const int BLOCK  = 2;
            /// <summary>
            /// Ball ID
            /// </summary>
            private const int BALL   = 3;
            /// <summary>
            /// Paddle ID
            /// </summary>
            private const int PADDLE = 4;
            /// <summary>
            /// ID to character mapping
            /// </summary>
            private static readonly char[] tileMapping =
            {
                ' ', //Empty
                '▓', //Wall
                '▒', //Block
                '═', //Paddle
                'O'  //Ball
            };
            #endregion
            
            #region Fields
            private readonly IntcodeVM software;
            private readonly char[,] screen;
            private readonly StringBuilder sb;
            private readonly int width;
            private readonly int height;
            private int printedLines;
            #endregion
            
            #region Properties
            public int Blocks { get; }
            public int Score { get; private set; }
            #endregion
            
            #region Constructors
            /// <summary>
            /// Creates and setups a new Arcade from the given software
            /// </summary>
            /// <param name="software">Software to run the arcade on</param>
            public Arcade(IntcodeVM software)
            {
                this.software = software;
                this.Blocks = 0;
                this.width = 0;
                this.height = 0;
                this.software.Run();
                while (this.software.HasOutputs)
                {
                    (long x, long y, long id) = this.software;
                    if (id is BLOCK)
                    {
                        this.Blocks++;
                    }

                    this.width = Math.Max(this.width, (int)x);
                    this.height = Math.Max(this.height, (int)y);
                }
                
                this.software.Reset();
                this.width++;
                this.height++;
                this.screen = new char[this.height, this.width];
                this.sb = new StringBuilder(this.screen.Length + this.height + 15);
            }
            #endregion
            
            #region Methods
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
                            this.screen[y, x] = tileMapping[id];
                            switch (id)
                            {
                                case BALL:
                                    ball = x;
                                    break;
                                case PADDLE:
                                    paddle = x;
                                    break;
                            }
                        }
                    }
                    
                    //Display
                    PrintToConsole();
                    //Handle input for next move
                    this.software.AddInput(ball > paddle ? -1L : (ball < paddle ? 1L : 0L));
                    //Render at 30fps to see what's going on
                    Thread.Sleep(33);
                }

                //Show cursor again
                Console.CursorVisible = true;
                //Return the final score
                return this.Score;
            }
            
            /// <summary>
            /// Prints the arcade screen to the console
            /// </summary>
            public void PrintToConsole()
            {
                if (this.printedLines is not 0)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - this.printedLines);
                }
                Console.Write(this);
                this.printedLines = this.height + 1;
            }
            
            /// <summary>
            /// Converts the arcade screen to a string
            /// </summary>
            /// <returns>String representation of the arcade screen</returns>
            public override string ToString()
            {
                this.sb.Clear();
                this.sb.AppendLine($"Score: {this.Score}");
                foreach (int y in ..this.height)
                {
                    foreach (int x in ..this.width)
                    {
                        this.sb.Append(this.screen[y, x]);
                    }
                    this.sb.AppendLine();
                }

                return this.sb.ToString();
            }
            #endregion
        }
        
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day13"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
        public Day13(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            Arcade arcade = new(this.VM);
            AoCUtils.LogPart1(arcade.Blocks);
            AoCUtils.LogPart2(arcade.Play());
        }
        #endregion
    }
}
