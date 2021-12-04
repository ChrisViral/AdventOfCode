using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Grids;
using AdventOfCode.Grids.Vectors;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 17
/// </summary>
public class Day17 : IntcodeSolver
{
    private enum Hull
    {
        EMPTY       = '.',
        SCAFFOLD    = '#',
        ROBOT_UP    = '^',
        ROBOT_DOWN  = 'v',
        ROBOT_LEFT  = '<',
        ROBOT_RIGHT = '>'
    }

    #region Fields
    private (int x, int y) writePos = (0, -1);
    private ConsoleView<Hull> hull = null!;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day17"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
    public Day17(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        //Get the view
        this.VM.Run();
        StringBuilder scaffoldBuilder = new();
        this.VM.GetOutput().Select(o => (char)o).ForEach(c => scaffoldBuilder.Append(c));

        //Put into grid
        string[] view = scaffoldBuilder.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
        this.hull = new(view[0].Length, view.Length, ToChar, Anchor.TOP_LEFT);
        this.hull.Populate(view, s => s.ToCharArray().ConvertAll(c => (Hull)c));

        //Setup what is visible
        Vector2<int>? position = Vector2<int>.Zero;
        int alignment = 0;
        foreach (int y in ..this.hull.Height)
        {
            foreach (int x in ..this.hull.Width)
            {
                Vector2<int> pos = new(x, y);
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (this.hull[pos])
                {
                    //Intersections
                    case Hull.SCAFFOLD when pos.Adjacent().All(p => this.hull.WithinGrid(p) && this.hull[p] is Hull.SCAFFOLD):
                        alignment += pos.X * pos.Y;
                        break;

                    //Robot position
                    case Hull.ROBOT_UP:
                    case Hull.ROBOT_DOWN:
                    case Hull.ROBOT_LEFT:
                    case Hull.ROBOT_RIGHT:
                        position = pos;
                        break;
                }

            }
        }
        AoCUtils.LogPart1(alignment);

        //Get pathing
        Directions direction = Directions.UP;
        int steps = 1;
        StringBuilder pathBuilder = new();
        while (true)
        {
            Vector2<int>? newPosition = this.hull.MoveWithinGrid(position.Value, direction);
            if (newPosition.HasValue && this.hull[newPosition.Value] is Hull.SCAFFOLD)
            {
                //Movement valid
                steps++;
            }
            else
            {
                //Movement invalid, print steps
                if (steps > 1)
                {
                    pathBuilder.Append(steps).Append(',');
                    steps = 1;
                }

                //Try to turn left
                direction = direction.TurnLeft();
                newPosition = this.hull.MoveWithinGrid(position.Value, direction);
                if (newPosition.HasValue && this.hull[newPosition.Value] is Hull.SCAFFOLD)
                {
                    pathBuilder.Append("L,");
                }
                else
                {
                    //Otherwise turn right
                    direction = direction.Invert();
                    newPosition = this.hull.MoveWithinGrid(position.Value, direction);
                    if (!newPosition.HasValue || this.hull[newPosition.Value] is not Hull.SCAFFOLD)
                    {
                        //End of path
                        pathBuilder.Remove(pathBuilder.Length - 1, 1);
                        break;
                    }
                    pathBuilder.Append("R,");
                }
            }
            //Update position
            position = newPosition;
        }

        //Parse best grouped instructions
        Regex regex = new(@"(?:([RL][RL,\d]{6,19}),).*(\1)$");
        string[] instructions = new string[3];
        foreach (int i in ..3)
        {
            string instruction = regex.Match(pathBuilder.ToString()).Groups[1].Value;
            pathBuilder.Replace(instruction, new((char)(i + 'A'), 1)).Remove(pathBuilder.Length - 2, 2);
            instructions[i] = instruction + "\n";
        }

        //Setup for video feed
        this.VM.Reset();
        this.VM[0] = 2L;

        //Print all prompts
        Prompt(pathBuilder.Append(",C,B,A\n").ToString(), this.hull.Size + this.hull.Height + 1);
        instructions.ForEach(s => Prompt(s));
        Prompt("y\n");

        //Setup feed
        Console.CursorVisible = false;
        this.VM.OnOutput += OnOutput;
        this.VM.Run();
        Console.CursorVisible = true;
    }

    /// <summary>
    /// Prints a prompt to the VM and console
    /// </summary>
    /// <param name="response">Response to the prompt</param>
    /// <param name="skip">Characters to skip in VM output</param>
    private void Prompt(string response, int skip = 0)
    {
        this.VM.Run();
        string prompt = new(this.VM.GetOutput()[skip..^1].ConvertAll(i => (char)i));
        Console.Write($"{prompt} {response}");
        this.VM.AddInput(response);
    }

    /// <summary>
    /// VM output listener
    /// </summary>
    private void OnOutput()
    {
        long value = this.VM.GetNextOutput();
        switch (value)
        {
            case '\n':
                //Newline, adjust write position
                this.writePos.y++;
                this.writePos.x = 0;
                if (this.writePos.y == this.hull.Height + 1)
                {
                    this.writePos.y = 0;
                    this.hull.PrintToConsole();
                }
                break;

            case <= sbyte.MaxValue:
                //ASCII value
                this.hull[this.writePos] = (Hull)value;
                this.writePos.x++;
                break;

            default:
                //Final answer
                AoCUtils.LogPart2(value);
                break;
        }
    }

    /// <summary>
    /// Hull to char conversion
    /// </summary>
    /// <param name="hull">Hull value to convert to char</param>
    /// <returns>The pretty char representation of the hull</returns>
    private static char ToChar(Hull hull)
    {
        return hull switch
        {
            Hull.EMPTY    => '░',
            Hull.SCAFFOLD => '▓',
            _             => (char)hull
        };
    }
    #endregion
}