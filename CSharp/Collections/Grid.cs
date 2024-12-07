using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Extensions.StringBuilders;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using JetBrains.Annotations;

namespace AdventOfCode.Collections;

/// <summary>
/// Generic grid structure
/// </summary>
/// <typeparam name="T">Type of element within the grid</typeparam>
[PublicAPI]
public class Grid<T> : IEnumerable<T>
{
    #region Static fields
    private static readonly EqualityComparer<T> Comparer = EqualityComparer<T>.Default;
    #endregion

    #region Fields
    protected readonly T[,] grid;
    protected readonly int rowBufferSize;
    protected readonly Converter<T, string> toString;
    protected readonly StringBuilder toStringBuilder = new();
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

    /// <summary>
    /// Dimensions of the grid
    /// </summary>
    public Vector2<int> Dimensions { get; }
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
    /// ReSharper disable once VirtualMemberNeverOverridden.Global
    public virtual T this[Vector2<int> vector]
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

    /// <summary>
    /// Gets a slice of this Grid in a certain range
    /// </summary>
    /// <param name="xRange">X range of the slice</param>
    /// <param name="yRange">Y range of the slice</param>
    /// <returns>The grid slice</returns>
    /// <exception cref="InvalidOperationException">If any part of the slice is outside of the grid's range</exception>
    public virtual Grid<T> this[Range xRange, Range yRange]
    {
        get
        {
            // Get starting point
            int startX = xRange.Start.IsFromEnd ? Width  - xRange.Start.Value : xRange.Start.Value;
            int startY = yRange.Start.IsFromEnd ? Height - yRange.Start.Value : yRange.Start.Value;
            Vector2<int> start = new(startX, startY);
            if (!WithinGrid(start)) throw new InvalidOperationException("Starting point of the grid slice outside of the original grid");

            // Get ending point
            int endX = xRange.End.IsFromEnd ? Width  - xRange.End.Value : xRange.End.Value;
            int endY = yRange.End.IsFromEnd ? Height - yRange.End.Value : yRange.End.Value;
            Vector2<int> end = new(endX, endY);
            if (!WithinGrid(end - Vector2<int>.One)) throw new InvalidOperationException("Ending point of the grid slice outside of the original grid");

            // Get slice size
            Vector2<int> size = end - start;
            if (size.X <= 0 || size.Y <= 0) throw new InvalidOperationException("Grid slice has at least one zero sized dimension");

            // Create and fill slice
            Grid<T> slice = new(size.X, size.Y, this.toString);
            foreach (Vector2<int> position in Vector2<int>.Enumerate(size.X, size.Y))
            {
                slice[position] = this[start + position];
            }

            return slice;
        }
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
        if (width  <= 0) throw new ArgumentOutOfRangeException(nameof(width),  width,  "Width must be greater than 0");
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be greater than 0");

        this.Width  = width;
        this.Height = height;
        this.Size   = width * height;
        this.grid   = new T[height, width];
        this.Dimensions = new(width, height);

        if (typeof(T).IsPrimitive)
        {
            this.rowBufferSize = this.Width * PrimitiveUtils<T>.BufferSize;
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
    public Grid(int width, int height, string[] input, Converter<string, T[]> converter, Converter<T, string>? toString = null)
        : this(width, height, toString)
    {
        Populate(input, converter);
    }

    /// <summary>
    /// Grid copy constructor
    /// </summary>
    /// <param name="other">Other grid to create a copy of</param>
    public Grid(Grid<T> other)
    {
        this.Width  = other.Width;
        this.Height = other.Height;
        this.Size   = other.Size;
        this.Dimensions    = other.Dimensions;
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
    /// ReSharper disable once MemberCanBePrivate.Global
    public void Populate(string[] input, [InstantHandle] Converter<string, T[]> converter)
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
                continue;
            }

            for (int i = 0; i < this.Width; i++)
            {
                this[i, j] = result[i];
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
        if (this.rowBufferSize is not 0)
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
    /// Gets the given row of the grid without allocations
    /// </summary>
    /// <param name="y">Row index of the row to get</param>
    /// <param name="row">Array in which to store the row data</param>
    /// <returns>The specified row of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="y"/> is not within the limits of the Grid</exception>
    /// <exception cref="ArgumentException">If <paramref name="row"/> is too small to fit the size of the row</exception>
    public void GetRowNoAlloc(int y, T[] row)
    {
        if (y < 0 || y >= this.Height) throw new ArgumentOutOfRangeException(nameof(y), y, "Row index must be within limits of Grid");
        if (row.Length < this.Width) throw new ArgumentException("Pre allocated column array is too small", nameof(row));

        if (this.rowBufferSize is not 0)
        {
            //For primitives only
            Buffer.BlockCopy(this.grid, y * this.rowBufferSize, row, 0, this.rowBufferSize);
            return;
        }

        for (int i = 0; i < this.Width; i++)
        {
            row[i] = this[i, y];
        }
    }

    /// <summary>
    /// Gets the given row of the grid without allocations
    /// </summary>
    /// <param name="y">Row index of the row to get</param>
    /// <param name="row">Array in which to store the row data</param>
    /// <returns>The specified row of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="y"/> is not within the limits of the Grid</exception>
    /// <exception cref="ArgumentException">If <paramref name="row"/> is too small to fit the size of the row</exception>
    public void GetRowNoAlloc(int y, ref Span<T> row)
    {
        if (y < 0 || y >= this.Height) throw new ArgumentOutOfRangeException(nameof(y), y, "Row index must be within limits of Grid");
        if (row.Length < this.Width) throw new ArgumentException("Pre allocated column array is too small", nameof(row));

        for (int i = 0; i < this.Width; i++)
        {
            row[i] = this[i, y];
        }
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
    /// Gets the given column of the grid without allocations
    /// </summary>
    /// <param name="x">Column index of the column to get</param>
    /// <param name="column">Array in which to store the column data</param>
    /// <returns>The specified column of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="x"/> is not within the limits of the Grid</exception>
    /// <exception cref="ArgumentException">If <paramref name="column"/> is too small to fit the size of the column</exception>
    public void GetColumnNoAlloc(int x, T[] column)
    {
        if (x < 0 || x >= this.Width) throw new ArgumentOutOfRangeException(nameof(x), x, "Column index must be within limits of Grid");
        if (column.Length < this.Height) throw new ArgumentException("Pre allocated column array is too small", nameof(column));

        for (int j = 0; j < this.Height; j++)
        {
            column[j] = this[x, j];
        }
    }

    /// <summary>
    /// Gets the given column of the grid without allocations
    /// </summary>
    /// <param name="x">Column index of the column to get</param>
    /// <param name="column">Array in which to store the column data</param>
    /// <returns>The specified column of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="x"/> is not within the limits of the Grid</exception>
    /// <exception cref="ArgumentException">If <paramref name="column"/> is too small to fit the size of the column</exception>
    public void GetColumnNoAlloc(int x, ref Span<T> column)
    {
        if (x < 0 || x >= this.Width) throw new ArgumentOutOfRangeException(nameof(x), x, "Column index must be within limits of Grid");
        if (column.Length < this.Height) throw new ArgumentException("Pre allocated column array is too small", nameof(column));

        for (int j = 0; j < this.Height; j++)
        {
            column[j] = this[x, j];
        }
    }

    /// <summary>
    /// Set the given row of the grid by the specified array
    /// </summary>
    /// <param name="y">Row index</param>
    /// <param name="row">Row values</param>
    /// <exception cref="ArgumentOutOfRangeException">If the row index is out of bounds of the grid</exception>
    /// <exception cref="ArgumentException">If the row values array is larger than the width of the grid</exception>
    public void SetRow(int y, T[] row)
    {
        if (y < 0 || y >= this.Height) throw new ArgumentOutOfRangeException(nameof(y), y, "Row index must be within limits of Grid");
        if (row.Length != this.Width)  throw new ArgumentException("Row is not the same width as grid", nameof(row));

        if (this.rowBufferSize is not 0)
        {
            //For primitives only
            Buffer.BlockCopy(row, 0, this.grid, y * this.rowBufferSize, this.rowBufferSize);
            return;
        }

        for (int i = 0; i < this.Width; i++)
        {
            this[i, y] = row[i];
        }
    }

    /// <summary>
    /// Set the given row of the grid by the specified array
    /// </summary>
    /// <param name="x">Column index</param>
    /// <param name="column">Column values</param>
    /// <exception cref="ArgumentOutOfRangeException">If the column index is out of bounds of the grid</exception>
    /// <exception cref="ArgumentException">If the column values array is larger than the width of the grid</exception>
    public void SetColumn(int x, T[] column)
    {
        if (x < 0 || x >= this.Width)     throw new ArgumentOutOfRangeException(nameof(x), x, "Column index must be within limits of Grid");
        if (column.Length != this.Height) throw new ArgumentException("Column is not the same width as grid", nameof(column));

        for (int j = 0; j < this.Height; j++)
        {
            this[x, j] = column[j];
        }
    }

    /// <summary>
    /// Fill the grid with the given value
    /// </summary>
    /// <param name="value">Value to fill with</param>
    public void Fill(T value) => this.Dimensions.EnumerateOver().ForEach(v => this[v] = value);

    /// <summary>
    /// Moves the vector within the grid
    /// </summary>
    /// <param name="vector">Vector to move</param>
    /// <param name="direction">Direction to move in</param>
    /// <param name="wrapX">If the vector should wrap around horizontally in the grid, else the movement is invalid</param>
    /// <param name="wrapY">If the vector should wrap around vertically in the grid, else the movement is invalid</param>
    /// <returns>The resulting Vector after the move, or null if the movement was invalid</returns>
    public virtual Vector2<int>? MoveWithinGrid(in Vector2<int> vector, Direction direction, bool wrapX = false, bool wrapY = false)
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
    public virtual Vector2<int>? MoveWithinGrid(in Vector2<int> vector, in Vector2<int> travel, bool wrapX = false, bool wrapY = false)
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
        if (!wrapY) return new(result);

        if (result.y >= this.Height)
        {
            result.y -= this.Height;
        }
        else if (result.y < 0)
        {
            result.y += this.Height;
        }

        //Return result
        return new(result);
    }

    /// <summary>
    /// Tries to move the vector within the grid
    /// </summary>
    /// <param name="vector">Vector to move</param>
    /// <param name="direction">Direction to move in</param>
    /// <param name="moved">The resulting moved vector, if succeeded</param>
    /// <param name="wrapX">If the vector should wrap around horizontally in the grid, else the movement is invalid</param>
    /// <param name="wrapY">If the vector should wrap around vertically in the grid, else the movement is invalid</param>
    /// <returns><see langword="true"/> if the move succeeded, else <see langword="false"/></returns>
    public virtual bool TryMoveWithinGrid(in Vector2<int> vector, Direction direction, out Vector2<int> moved, bool wrapX = false, bool wrapY = false)
    {
        Vector2<int>? move = MoveWithinGrid(vector, direction, wrapX, wrapY);
        if (move.HasValue)
        {
            moved = move.Value;
            return true;
        }

        moved = new(-1, -1);
        return false;
    }

    /// <summary>
    /// Tries to move the vector within the grid
    /// </summary>
    /// <param name="vector">Vector to move</param>
    /// <param name="travel">Vector to travel in</param>
    /// <param name="moved">The resulting moved vector, if succeeded</param>
    /// <param name="wrapX">If the vector should wrap around horizontally in the grid, else the limits act like walls</param>
    /// <param name="wrapY">If the vector should wrap around vertically in the grid, else the limits act like walls</param>
    /// <returns><see langword="true"/> if the move succeeded, else <see langword="false"/></returns>
    public virtual bool TryMoveWithinGrid(in Vector2<int> vector, in Vector2<int> travel, out Vector2<int> moved, bool wrapX = false, bool wrapY = false)
    {
        Vector2<int>? move = MoveWithinGrid(vector, travel, wrapX, wrapY);
        if (move.HasValue)
        {
            moved = move.Value;
            return true;
        }

        moved = new(-1, -1);
        return false;
    }

    /// <summary>
    /// Checks if a given position Vector2 is within the grid
    /// </summary>
    /// <param name="position">Position vector</param>
    /// <returns>True if the Vector2 is within the grid, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual bool WithinGrid(Vector2<int> position)
    {
        return position.X >= 0 && position.X < this.Width && position.Y >= 0 && position.Y < this.Height;
    }

    /// <summary>
    /// Gets the position of the given value in the grid, if it exists
    /// </summary>
    /// <param name="value">Value to find</param>
    /// <returns>The first position in the grid that the value is found at, or <c>(-1, -1)</c> if it wasn't</returns>
    public Vector2<int> PositionOf(T value)
    {
        foreach (Vector2<int> pos in Vector2<int>.Enumerate(this.Width, this.Height))
        {
            if (Comparer.Equals(value, this[pos]))
            {
                return pos;
            }
        }

        return new(-1, -1);
    }

    /// <summary>
    /// Checks if the given value is in the grid or not
    /// </summary>
    /// <param name="value">Value to find</param>
    /// <returns><see langword="true"/> if the value was in the grid, otherwise <see langword="false"/></returns>
    public bool Contains(T value) => Vector2<int>.Enumerate(this.Width, this.Height)
                                                 .Any(pos => Comparer.Equals(value, this[pos]));

    /// <summary>
    /// Clears this grid
    /// </summary>
    public void Clear() => Array.Clear(this.grid);

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public IEnumerator<T> GetEnumerator() => this.grid.Cast<T>().GetEnumerator();

    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    IEnumerator IEnumerable.GetEnumerator() => this.grid.GetEnumerator();

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString()
    {
        foreach (int j in ..this.Height)
        {
            foreach (int i in ..this.Width)
            {
                this.toStringBuilder.Append(this.toString(this[i, j]));
            }

            this.toStringBuilder.AppendLine();
        }

        return this.toStringBuilder.ToStringAndClear();
    }
    #endregion
}