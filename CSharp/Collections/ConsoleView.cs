using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Vectors;
using JetBrains.Annotations;

namespace AdventOfCode.Collections;

/// <summary>
/// View anchors
/// </summary>
[PublicAPI]
public enum Anchor
{
    TOP_LEFT,
    TOP_RIGHT,
    BOTTOM_LEFT,
    BOTTOM_RIGHT,
    MIDDLE
}

/// <summary>
/// Console interactive view
/// </summary>
/// <typeparam name="T">Type of object in the view</typeparam>
[PublicAPI]
public class ConsoleView<T> : Grid<T> where T : notnull
{
    #region Fields
    private readonly Vector2<int> anchor;
    private readonly char[] viewBuffer;
    private readonly Converter<T, char> toChar;
    private readonly int sleepTime;
    private readonly Stopwatch timer = new();
    protected int printedLines;
    #endregion

    #region Indexers
    /// <summary>
    /// Gets or sets a position in the view
    /// </summary>
    /// <param name="x">X Position</param>
    /// <param name="y">Y Position</param>
    /// <returns>The value in the view at the given location</returns>
    public override T this[int x, int y]
    {
        get => this[new(x, y)];
        set => this[new(x, y)] = value;
    }

    /// <summary>
    /// Gets or sets a position in the view
    /// </summary>
    /// <param name="pos">Position vector</param>
    /// <returns>The value in the view at the given location</returns>
    public T this[in Vector2<int> pos]
    {
        get => base[pos + this.anchor];
        set
        {
            (int x, int y) = pos + this.anchor;
            this.grid[y, x] = value;
            this.viewBuffer[(y * (this.Width + 1)) + x] = this.toChar(value);
        }
    }

    /// <summary>
    /// Gets or sets a position in the view
    /// </summary>
    /// <param name="tuple">Position tuple</param>
    /// <returns>The value in the view at the given location</returns>
    public override T this[(int x, int y) tuple]
    {
        get => this[new(tuple)];
        set => this[new(tuple)] = value;
    }
    #endregion

    #region Constructors
    /// <summary>
    /// ConsoleView base constructor, calls the Grid constructor to setup the underlying data
    /// </summary>
    /// <param name="width">Width of the view</param>
    /// <param name="height">Height of the view</param>
    /// <param name="toChar">Element to char conversion function</param>
    /// <param name="fps">Display FPS</param>
    private ConsoleView(int width, int height, Converter<T, char> toChar, int fps) : base(width, height)
    {
        //Setup
        this.viewBuffer = new char[height * (width + 1)];
        this.toChar = toChar;
        this.sleepTime = 1000 / fps;
    }

    /// <summary>
    /// Creates a new ConsoleView of the given height and width
    /// </summary>
    /// <param name="width">Width of the view</param>
    /// <param name="height">Height of the view</param>
    /// <param name="converter">Element to character conversion</param>
    /// <param name="anchor">Anchor from which the position written in the view is offset by, defaults to MIDDLE</param>
    /// <param name="defaultValue">The default value to fill the view with</param>
    /// <param name="fps">Target FPS of the display, defaults to 30</param>
    public ConsoleView(int width, int height, Converter<T, char> converter, Anchor anchor = Anchor.MIDDLE, T defaultValue = default!, int fps = 30) : this(width, height, converter, fps)
    {
        this.anchor = anchor switch
        {
            Anchor.TOP_LEFT     => Vector2<int>.Zero,
            Anchor.TOP_RIGHT    => new(width - 1, 0),
            Anchor.BOTTOM_LEFT  => new(0, height - 1),
            Anchor.BOTTOM_RIGHT => new(width - 1, height - 1),
            Anchor.MIDDLE       => new(width / 2, height / 2),
            _                   => throw new InvalidEnumArgumentException(nameof(anchor), (int)anchor, typeof(Anchor))
        };
        FillDefault(converter, defaultValue);
    }

    /// <summary>
    /// Creates a new ConsoleView of the given height and width
    /// </summary>
    /// <param name="width">Width of the view</param>
    /// <param name="height">Height of the view</param>
    /// <param name="converter">Element to character conversion</param>
    /// <param name="anchor">Anchor from which the position written in the view is offset by</param>
    /// <param name="defaultValue">The default value to fill the view with</param>
    /// <param name="fps">Target FPS of the display, defaults to 30</param>
    /// ReSharper disable once MemberCanBeProtected.Global
    public ConsoleView(int width, int height, Converter<T, char> converter, Vector2<int> anchor, T defaultValue = default!, int fps = 30) : this(width, height, converter, fps)
    {
        this.anchor = anchor;
        FillDefault(converter, defaultValue);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Fills the view with a specified default value
    /// </summary>
    /// <param name="converter">Value to char converter</param>
    /// <param name="defaultValue">Value to fill in</param>
    public void FillDefault([InstantHandle] Converter<T, char> converter, T defaultValue)
    {
        //View buffer
        this.viewBuffer.Fill(converter(defaultValue));

        //Maze buffer
        T[] line = new T[this.Width];
        line.Fill(defaultValue);

        //Set everything in the arrays
        foreach (int j in ..this.Height)
        {
            this.viewBuffer[((this.Width + 1) * j) + this.Width] = '\n';
            if (this.rowBufferSize is not 0)
            {
                Buffer.BlockCopy(line, 0, this.grid, j * this.rowBufferSize, this.rowBufferSize);
                continue;
            }

            foreach (int i in ..this.Width)
            {
                this.grid[j, i] = line[i];
            }
        }
    }

    /// <inheritdoc cref="Grid{T}.Populate"/>
    public new void Populate(string[] input, [InstantHandle] Converter<string, T[]> converter)
    {
        if (input.Length != this.Height) throw new ArgumentException("Input array does not have the same amount of rows as the grid");

        for ((int x, int y) pos = (0, 0); pos.y < this.Height; pos.y++)
        {
            T[] result = converter(input[pos.y]);
            if (result.Length != this.Width) throw new InvalidOperationException($"Input line {pos.y} does not produce a row of the same length as the grid after conversion");

            for (pos.x = 0; pos.x < this.Width; pos.x++)
            {
                this[pos - this.anchor] = result[pos.x];
            }
        }
    }

    /// <summary>
    /// Prints the view screen to the console and waits to hit the target FPS
    /// </summary>
    public virtual void PrintToConsole()
    {
        //Clear anything already printed
        if (this.printedLines is not 0)
        {
            Console.SetCursorPosition(0, Math.Max(0, Console.CursorTop - this.printedLines));
        }

        //Write to console
        Console.Write(ToString());
        this.printedLines = this.Height;
        //Display at target fps
        this.timer.Stop();
        Thread.Sleep(Math.Max(0, this.sleepTime - (int)this.timer.ElapsedMilliseconds));
        this.timer.Restart();
    }

    /// <summary>
    /// Moves the vector within the grid
    /// </summary>
    /// <param name="vector">Vector to move</param>
    /// <param name="direction">Direction to move in</param>
    /// <param name="wrapX">If the vector should wrap around horizontally in the grid, else the movement is invalid</param>
    /// <param name="wrapY">If the vector should wrap around vertically in the grid, else the movement is invalid</param>
    /// <returns>The resulting Vector after the move, or null if the movement was invalid</returns>
    public override Vector2<int>? MoveWithinGrid(in Vector2<int> vector, Direction direction, bool wrapX = false, bool wrapY = false)
    {
        return MoveWithinGrid(vector, direction.ToVector<int>(), wrapX, wrapY);
    }

    /// <summary>
    /// Moves the vector within the grid
    /// </summary>
    /// <param name="vector">Vector to move</param>
    /// <param name="travel">Vector to travel in</param>
    /// <param name="wrapX">If the vector should wrap around horizontally in the grid, else the limits act like walls</param>
    /// <param name="wrapY">If the vector should wrap around vertically in the grid, else the limits act like walls</param>
    /// <returns>The resulting Vector after the move</returns>
    public override Vector2<int>? MoveWithinGrid(in Vector2<int> vector, in Vector2<int> travel, bool wrapX = false, bool wrapY = false)
    {
        Vector2<int>? result = base.MoveWithinGrid(vector - this.anchor, travel, wrapX, wrapY);
        return result.HasValue ? result.Value + this.anchor : null;
    }

    /// <inheritdoc cref="Grid{T}.WithinGrid"/>
    public override bool WithinGrid(Vector2<int> position) => base.WithinGrid(position - this.anchor);

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => new(this.viewBuffer);
    #endregion
}