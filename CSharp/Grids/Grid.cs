using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Utils;
using Vector2 = AdventOfCode.Grids.Vectors.Vector2<int>;

namespace AdventOfCode.Grids;

/// <summary>
/// Generic grid structure
/// </summary>
/// <typeparam name="T">Type of element within the grid</typeparam>
public class Grid<T> : IEnumerable<T>
{
    #region Fields
    protected readonly T[,] grid;
    protected readonly int rowBufferSize;
    private readonly Converter<T, string> toString;
    #endregion

    #region Properties
    /// <summary>
    /// Grid width
    /// </summary>
    public int Width{ get; }

    /// <summary>
    /// Grid height
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Size of the grid
    /// </summary>
    public int Size { get; }
    #endregion

    #region Indexers
    /// <summary>
    /// Accesses an element in the grid
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <returns>The element at the specified position</returns>
    public virtual T this[int x, int y]
    {
        get => this.grid[y, x];
        set => this.grid[y, x] = value;
    }

    /// <summary>
    /// Accesses an element in the grid
    /// </summary>
    /// <param name="vector">Position vector in the grid</param>
    /// <returns>The element at the specified position</returns>
    public virtual T this[Vector2 vector]
    {
        get => this.grid[vector.Y, vector.X];
        set => this.grid[vector.Y, vector.X] = value;
    }

    /// <summary>
    /// Accesses an element in the grid
    /// </summary>
    /// <param name="tuple">Position tuple in the grid</param>
    /// <returns>The element at the specified position</returns>
    public virtual T this[(int x, int y) tuple]
    {
        get => this.grid[tuple.y, tuple.x];
        set => this.grid[tuple.y, tuple.x] = value;
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new grid with the specified size
    /// </summary>
    /// <param name="width">Width of the grid</param>
    /// <param name="height">Height of the grid</param>
    /// <param name="toString">ToString conversion function</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="width"/> or <paramref name="height"/> is the than or equal to zero</exception>
    public Grid(int width, int height, Converter<T, string>? toString = null)
    {
        if (width <= 0)  throw new ArgumentOutOfRangeException(nameof(width),  width,  "Width must be greater than 0");
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be greater than 0");

        this.Width = width;
        this.Height = height;
        this.Size = width * height;
        this.grid = new T[height, width];

        if (typeof(T).IsPrimitive)
        {
            this.rowBufferSize = this.Width * AoCUtils.GetSizeOfPrimitive<T>();
        }
        this.toString = toString ?? (t => t?.ToString() ?? string.Empty);
    }

    /// <summary>
    /// Creates a new grid with the specified size and populates it
    /// </summary>
    /// <param name="width">Width of the grid</param>
    /// <param name="height">Height of the grid</param>
    /// <param name="input">Input lines</param>
    /// <param name="converter">Conversion function from the input string to a full row</param>
    /// <param name="toString">ToString conversion function</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="width"/> or <paramref name="height"/> is the than or equal to zero</exception>
    /// <exception cref="ArgumentException">If the input lines is not of the same size as the amount of rows in the grid</exception>
    /// <exception cref="InvalidOperationException">If a certain line does not produce a row of the same length as the grid</exception>
    public Grid(int width, int height, string[] input, Converter<string, T[]> converter, Converter<T, string>? toString = null) : this(width, height, toString) => Populate(input, converter);

    /// <summary>
    /// Grid copy constructor
    /// </summary>
    /// <param name="other">Other grid to create a copy of</param>
    public Grid(Grid<T> other)
    {
        this.Width = other.Width;
        this.Height = other.Height;
        this.Size = other.Size;
        this.rowBufferSize = other.rowBufferSize;
        this.toString = other.toString;
        this.grid = new T[this.Height, this.Width];
        CopyFrom(other);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Populates the grid with the given input data
    /// </summary>
    /// <param name="input">Input lines</param>
    /// <param name="converter">Conversion function from the input string to a full row</param>
    /// <exception cref="ArgumentException">If the input lines is not of the same size as the amount of rows in the grid</exception>
    /// <exception cref="InvalidOperationException">If a certain line does not produce a row of the same length as the grid</exception>
    public void Populate(string[] input, Converter<string, T[]> converter)
    {
        if (input.Length != this.Height) throw new ArgumentException("Input array does not have the same amount of rows as the grid");

        for (int j = 0; j < this.Height; j++)
        {
            T[] result = converter(input[j]);
            if (result.Length != this.Width) throw new InvalidOperationException($"Input line {j} does not produce a row of the same length as the grid after conversion");

            if (this.rowBufferSize != 0)
            {
                //For primitives only
                Buffer.BlockCopy(result, 0, this.grid, j * this.rowBufferSize, this.rowBufferSize);
            }
            else
            {
                for (int i = 0; i < this.Width; i++)
                {
                    this[i, j] = result[i];
                }
            }
        }
    }

    /// <summary>
    /// Copies the data from another Grid into this one
    /// </summary>
    /// <param name="other">Other grid to copy from</param>
    public void CopyFrom(Grid<T> other)
    {
        if (other.Size != this.Size) throw new InvalidOperationException("Cannot copy two grids with different sizes");

        //Copy data over
        if (this.rowBufferSize > 0)
        {
            Buffer.BlockCopy(other.grid, 0, this.grid, 0, this.Height * this.rowBufferSize);
        }
        else
        {
            Array.Copy(other.grid, this.grid, this.Size);
        }
    }

    /// <summary>
    /// Gets the given row of the grid<br/>
    /// <b>NOTE</b>: This allocates a new array on each call
    /// </summary>
    /// <param name="y">Row index of the row to get</param>
    /// <returns>The specified row of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="y"/> is not within the limits of the Grid</exception>
    public T[] GetRow(int y)
    {
        if (y < 0 || y >= this.Height) throw new ArgumentOutOfRangeException(nameof(y), y, "Row index must be within limits of Grid");

        T[] row = new T[this.Width];
        if (this.rowBufferSize != 0)
        {
            //For primitives only
            Buffer.BlockCopy(this.grid, y * this.rowBufferSize, row, 0, this.rowBufferSize);
        }
        else
        {
            for (int i = 0; i < this.Width; i++)
            {
                row[i] = this[i, y];
            }
        }
        return row;
    }

    /// <summary>
    /// Gets the given column of the grid<br/>
    /// <b>NOTE</b>: This allocates a new array on each call
    /// </summary>
    /// <param name="x">Column index of the column to get</param>
    /// <returns>The specified column of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="x"/> is not within the limits of the Grid</exception>
    public T[] GetColumn(int x)
    {
        if (x < 0 || x >= this.Width) throw new ArgumentOutOfRangeException(nameof(x), x, "Column index must be within limits of Grid");

        T[] column = new T[this.Height];
        for (int j = 0; j < this.Height; j++)
        {
            column[j] = this[x, j];
        }
        return column;
    }

    /// <summary>
    /// Moves the vector within the grid
    /// </summary>
    /// <param name="vector">Vector to move</param>
    /// <param name="directions">Direction to move in</param>
    /// <param name="wrapX">If the vector should wrap around horizontally in the grid, else the movement is invalid</param>
    /// <param name="wrapY">If the vector should wrap around vertically in the grid, else the movement is invalid</param>
    /// <returns>The resulting Vector after the move, or null if the movement was invalid</returns>
    public virtual Vector2? MoveWithinGrid(in Vector2 vector, Directions directions, bool wrapX = false, bool wrapY = false) => MoveWithinGrid(vector, directions.ToVector<int>(), wrapX, wrapY);

    /// <summary>
    /// Moves the vector within the grid
    /// </summary>
    /// <param name="vector">Vector to move</param>
    /// <param name="travel">Vector to travel in</param>
    /// <param name="wrapX">If the vector should wrap around horizontally in the grid, else the limits act like walls</param>
    /// <param name="wrapY">If the vector should wrap around vertically in the grid, else the limits act like walls</param>
    /// <returns>The resulting Vector after the move</returns>
    public virtual Vector2? MoveWithinGrid(in Vector2 vector, in Vector2 travel, bool wrapX = false, bool wrapY = false)
    {
        (int x, int y) result = vector + travel;
        //Check if an invalid wrap must happen
        if ((!wrapX && (result.x >= this.Width  || result.x < 0))
         || (!wrapY && (result.y >= this.Height || result.y < 0))) return null;

        //Wrap x axis
        if (wrapX)
        {
            if (result.x >= this.Width)
            {
                result.x -= this.Width;
            }
            else if (result.x < 0)
            {
                result.x += this.Width;
            }
        }

        //Wrap y axis
        if (wrapY)
        {
            if (result.y >= this.Height)
            {
                result.y -= this.Height;
            }
            else if (result.y < 0)
            {
                result.y += this.Height;
            }
        }

        //Return result
        return new Vector2(result);
    }

    /// <summary>
    /// Checks if a given position Vector2 is within the grid
    /// </summary>
    /// <param name="position">Position vector</param>
    /// <returns>True if the Vector2 is within the grid, false otherwise</returns>
    public virtual bool WithinGrid(Vector2 position) => position.X >= 0 && position.X < this.Width && position.Y >= 0 && position.Y < this.Height;

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public IEnumerator<T> GetEnumerator() => this.grid.Cast<T>().GetEnumerator();

    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    IEnumerator IEnumerable.GetEnumerator() => this.grid.GetEnumerator();

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString()
    {
        StringBuilder sb = new();
        foreach (int j in ..this.Height)
        {
            foreach (int i in ..this.Width)
            {
                sb.Append(this.toString(this[i, j]));
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
    #endregion
}