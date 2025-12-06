using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Extensions.StringBuilders;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Enumerables;
using JetBrains.Annotations;

namespace AdventOfCode.Collections;

/// <summary>
/// Generic grid structure
/// </summary>
/// <typeparam name="T">Type of element within the grid</typeparam>
[PublicAPI]
public class Grid<T> : IEnumerable<T>
{
    private static readonly EqualityComparer<T> Comparer = EqualityComparer<T>.Default;

    protected readonly T[,] grid;
    protected readonly int rowBufferSize;
    protected readonly Converter<T, string> toString;
    protected readonly StringBuilder toStringBuilder = new();

    /// <summary>
    /// Grid width
    /// </summary>
    public int Width { get; }

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
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <returns>The element at the specified position</returns>
    public virtual T this[Index x, Index y]
    {
        get => this.grid[y.GetOffset(this.Height), x.GetOffset(this.Width)];
        set => this.grid[y.GetOffset(this.Height), x.GetOffset(this.Width)] = value;
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
    /// <exception cref="ArgumentOutOfRangeException">If any part of the slice is outside of the grid's range</exception>
    /// <exception cref="InvalidOperationException">If the grid slice is of size 0 in one dimension</exception>
    public virtual Span2D<T> this[Range xRange, Range yRange]
    {
        get
        {
            // Get offsets and lengths by dimension
            (int column, int width) = xRange.GetOffsetAndLength(this.Width);
            (int row, int height) = yRange.GetOffsetAndLength(this.Height);

            if (width <= 0 || height <= 0) throw new InvalidOperationException("Grid slice has at least one zero sized dimension");

            return this.grid.AsSpan2D(row, column, height, width);
        }
    }

    /// <summary>
    /// Creates a new grid with the specified size
    /// </summary>
    /// <param name="width">Width of the grid</param>
    /// <param name="height">Height of the grid</param>
    /// <param name="toString">ToString conversion function</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="width"/> or <paramref name="height"/> is the than or equal to zero</exception>
    public Grid(int width, int height, Converter<T, string>? toString = null)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be greater than 0");
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be greater than 0");

        this.Width      = width;
        this.Height     = height;
        this.Size       = width * height;
        this.grid       = new T[height, width];
        this.Dimensions = new Vector2<int>(width, height);

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
        this.Width         = other.Width;
        this.Height        = other.Height;
        this.Size          = other.Size;
        this.Dimensions    = other.Dimensions;
        this.rowBufferSize = other.rowBufferSize;
        this.toString      = other.toString;
        this.grid          = new T[this.Height, this.Width];
        CopyFrom(other);
    }

    /// <summary>
    /// Grid copy constructor
    /// </summary>
    /// <param name="span">Span to create a copy of</param>
    public Grid(Span2D<T> span) : this(span.Width, span.Height)
    {
        CopyFrom(span);
    }

    /// <summary>
    /// Populates the grid with the given input data
    /// </summary>
    /// <param name="input">Input lines</param>
    /// <param name="converter">Conversion function from the input string to a full row</param>
    /// <exception cref="ArgumentException">If the input lines is not of the same size as the amount of rows in the grid</exception>
    /// <exception cref="InvalidOperationException">If a certain line does not produce a row of the same length as the grid</exception>
    /// ReSharper disable once MemberCanBePrivate.Global
    public void Populate(ReadOnlySpan<string> input, [InstantHandle] Converter<string, T[]> converter)
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
    /// Copies the data from another Grid into this one
    /// </summary>
    /// <param name="span">Span to copy from</param>
    public void CopyFrom(Span2D<T> span)
    {
        if (span.Width != this.Width || span.Height != this.Height) throw new InvalidOperationException("Cannot copy two grids with different sizes");

        span.CopyTo(this.grid);
    }

    /// <summary>
    /// Creates a new Span2D over the grid
    /// </summary>
    /// <returns>Span over the entire grid</returns>
    public Span2D<T> AsSpan2D() => this.grid.AsSpan2D();

    /// <summary>
    /// Creates a new Span2D over a section of the grid
    /// </summary>
    /// <returns>Span over the specified part of the grid</returns>
    public Span2D<T> AsSpan2D(int column, int width, int row, int height) => this.grid.AsSpan2D(row, column, height, width);

    /// <summary>
    /// Gets the given row of the grid<br/>
    /// <b>NOTE</b>: This allocates a new array on each call
    /// </summary>
    /// <param name="y">Row index of the row to get</param>
    /// <returns>The specified row of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="y"/> is not within the limits of the Grid</exception>
    public Span<T> GetRow(int y)
    {
        if (y < 0 || y >= this.Height) throw new ArgumentOutOfRangeException(nameof(y), y, "Row index must be within limits of Grid");

        return this.grid.GetRowSpan(y);
    }

    /// <inheritdoc cref="GetRow(int)"/>
    public Span<T> GetRow(Index y) => GetRow(y.GetOffset(this.Height));

    /// <summary>
    /// Gets the given row of the grid without allocations
    /// </summary>
    /// <param name="y">Row index of the row to get</param>
    /// <param name="row">Array in which to store the row data</param>
    /// <returns>The specified row of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="y"/> is not within the limits of the Grid</exception>
    /// <exception cref="ArgumentException">If <paramref name="row"/> is too small to fit the size of the row</exception>
    public void GetRow(int y, in T[] row)
    {
        if (y < 0 || y >= this.Height) throw new ArgumentOutOfRangeException(nameof(y), y, "Row index must be within limits of Grid");
        if (row.Length < this.Width) throw new ArgumentException("Pre allocated column array is too small", nameof(row));

        if (this.rowBufferSize is not 0)
        {
            //For primitives only
            Buffer.BlockCopy(this.grid, y * this.rowBufferSize, row, 0, this.rowBufferSize);
            return;
        }

        this.grid.GetRowSpan(y).CopyTo(row);
    }

    /// <inheritdoc cref="GetRow(int, in T[])"/>
    public void GetRow(Index y, in T[] row) => GetRow(y.GetOffset(this.Height), row);

    /// <summary>
    /// Gets the given column of the grid<br/>
    /// <b>NOTE</b>: This allocates a new array on each call
    /// </summary>
    /// <param name="x">Column index of the column to get</param>
    /// <returns>The specified column of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="x"/> is not within the limits of the Grid</exception>
    public RefEnumerable<T> GetColumn(int x)
    {
        if (x < 0 || x >= this.Width) throw new ArgumentOutOfRangeException(nameof(x), x, "Column index must be within limits of Grid");

        return this.grid.GetColumn(x);
    }

    /// <inheritdoc cref="GetColumn(int)"/>
    public RefEnumerable<T> GetColumn(Index x) => GetColumn(x.GetOffset(this.Width));

    /// <summary>
    /// Gets the given column of the grid without allocations
    /// </summary>
    /// <param name="x">Column index of the column to get</param>
    /// <param name="column">Array in which to store the column data</param>
    /// <returns>The specified column of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="x"/> is not within the limits of the Grid</exception>
    /// <exception cref="ArgumentException">If <paramref name="column"/> is too small to fit the size of the column</exception>
    public void GetColumn(int x, in T[] column)
    {
        if (x < 0 || x >= this.Width) throw new ArgumentOutOfRangeException(nameof(x), x, "Column index must be within limits of Grid");
        if (column.Length < this.Height) throw new ArgumentException("Pre allocated column array is too small", nameof(column));

        this.grid.GetColumn(x).CopyTo(column);
    }

    /// <inheritdoc cref="GetColumn(int, in T[])"/>
    public void GetColumn(Index x, in T[] column) => GetColumn(x.GetOffset(this.Width), column);

    /// <summary>
    /// Gets the given column of the grid without allocations
    /// </summary>
    /// <param name="x">Column index of the column to get</param>
    /// <param name="column">Array in which to store the column data</param>
    /// <returns>The specified column of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="x"/> is not within the limits of the Grid</exception>
    /// <exception cref="ArgumentException">If <paramref name="column"/> is too small to fit the size of the column</exception>
    public void GetColumn(int x, ref Span<T> column)
    {
        if (x < 0 || x >= this.Width) throw new ArgumentOutOfRangeException(nameof(x), x, "Column index must be within limits of Grid");
        if (column.Length < this.Height) throw new ArgumentException("Pre allocated column array is too small", nameof(column));

        this.grid.GetColumn(x).CopyTo(column);
    }

    /// <inheritdoc cref="GetColumn(int, ref Span{T})"/>
    public void GetColumn(Index x, ref Span<T> column) => GetColumn(x.GetOffset(this.Width), ref column);

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
        if (row.Length != this.Width) throw new ArgumentException("Row is not the same width as grid", nameof(row));

        if (this.rowBufferSize is not 0)
        {
            //For primitives only
            Buffer.BlockCopy(row, 0, this.grid, y * this.rowBufferSize, this.rowBufferSize);
            return;
        }

        row.CopyTo(this.grid.GetRowSpan(y));
    }

    /// <inheritdoc cref="SetRow(int, T[])"/>
    public void SetRow(Index y, T[] row) => SetRow(y.GetOffset(this.Height), row);

    /// <summary>
    /// Set the given row of the grid by the specified array
    /// </summary>
    /// <param name="y">Row index</param>
    /// <param name="row">Row values</param>
    /// <exception cref="ArgumentOutOfRangeException">If the row index is out of bounds of the grid</exception>
    /// <exception cref="ArgumentException">If the row values array is larger than the width of the grid</exception>
    public void SetRow(int y, ReadOnlySpan<T> row)
    {
        if (y < 0 || y >= this.Height) throw new ArgumentOutOfRangeException(nameof(y), y, "Row index must be within limits of Grid");
        if (row.Length != this.Width) throw new ArgumentException("Row is not the same width as grid", nameof(row));

        row.CopyTo(this.grid.GetRowSpan(y));
    }

    /// <inheritdoc cref="SetRow(int, ReadOnlySpan{T})"/>
    public void SetRow(Index y, ReadOnlySpan<T> row) => SetRow(y.GetOffset(this.Height), row);

    /// <summary>
    /// Set the given row of the grid by the specified array
    /// </summary>
    /// <param name="x">Column index</param>
    /// <param name="column">Column values</param>
    /// <exception cref="ArgumentOutOfRangeException">If the column index is out of bounds of the grid</exception>
    /// <exception cref="ArgumentException">If the column values array is larger than the width of the grid</exception>
    public void SetColumn(int x, ReadOnlySpan<T> column)
    {
        if (x < 0 || x >= this.Width) throw new ArgumentOutOfRangeException(nameof(x), x, "Column index must be within limits of Grid");
        if (column.Length != this.Height) throw new ArgumentException("Column is not the same width as grid", nameof(column));

        column.CopyTo(this.grid.GetColumn(x));
    }

    /// <inheritdoc cref="SetColumn(int, ReadOnlySpan{T})"/>
    public void SetColumn(Index x, ReadOnlySpan<T> column) => SetColumn(x.GetOffset(this.Width), column);

    /// <summary>
    /// Fill the grid with the given value
    /// </summary>
    /// <param name="value">Value to fill with</param>
    public void Fill(T value) => this.grid.AsSpan2D().Fill(value);

    /// <summary>
    /// Tries to get a value in the grid at the given position
    /// </summary>
    /// <param name="position">Position to get the value for</param>
    /// <param name="value">The value, if it was found</param>
    /// <returns><see langword="true"/> if the value was found, otherwise <see langword="false"/></returns>
    public virtual bool TryGetPosition(in Vector2<int> position, [MaybeNullWhen(false)] out T value)
    {
        if (WithinGrid(position))
        {
            value = this[position];
            return true;
        }

        value = default;
        return false;
    }

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
        else if (!result.x.IsInRange(0, this.Width)) return null;

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
        else if (!result.y.IsInRange(0, this.Height)) return null;

        //Return result
        return new Vector2<int>(result);
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

        moved = new Vector2<int>(-1, -1);
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

        moved = new Vector2<int>(-1, -1);
        return false;
    }

    /// <summary>
    /// Checks if a given position Vector2 is within the grid
    /// </summary>
    /// <param name="position">Position vector</param>
    /// <returns>True if the Vector2 is within the grid, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual bool WithinGrid(in Vector2<int> position)
    {
        return position.X >= 0 && position.X < this.Width && position.Y >= 0 && position.Y < this.Height;
    }

    /// <summary>
    /// Gets the position of the given value in the grid, if it exists
    /// </summary>
    /// <param name="value">Value to find</param>
    /// <returns>The first position in the grid that the value is found at, or <c>(-1, -1)</c> if it wasn't</returns>
    public virtual Vector2<int> PositionOf(T value)
    {
        foreach (Vector2<int> pos in Vector2<int>.EnumerateOver(this.Width, this.Height))
        {
            if (Comparer.Equals(value, this[pos]))
            {
                return pos;
            }
        }

        return new Vector2<int>(-1, -1);
    }

    /// <summary>
    /// Checks if the given value is in the grid or not
    /// </summary>
    /// <param name="value">Value to find</param>
    /// <returns><see langword="true"/> if the value was in the grid, otherwise <see langword="false"/></returns>
    public bool Contains(T value)
    {
        foreach (Vector2<int> pos in Vector2<int>.EnumerateOver(this.Width, this.Height))
        {
            if (Comparer.Equals(value, this[pos]))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Clears this grid
    /// </summary>
    public void Clear() => Array.Clear(this.grid);

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public Enumerator GetEnumerator() => new(this);

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.grid.Cast<T>().GetEnumerator();

    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    IEnumerator IEnumerable.GetEnumerator() => this.grid.Cast<T>().GetEnumerator();

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

    /// <summary>
    /// Rapid ref enumerator using 2D spans
    /// </summary>
    /// <param name="grid">Grid to create the enumerator from</param>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public ref struct Enumerator(Grid<T> grid)
    {
        private Span2D<T>.Enumerator enumerator = grid.grid.AsSpan2D().GetEnumerator();

        /// <inheritdoc cref="CommunityToolkit.HighPerformance.Span2D{T}.Enumerator.Current"/>
        public readonly ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref this.enumerator.Current;
        }

        /// <inheritdoc cref="CommunityToolkit.HighPerformance.Span2D{T}.Enumerator.MoveNext()"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => this.enumerator.MoveNext();
    }
}
