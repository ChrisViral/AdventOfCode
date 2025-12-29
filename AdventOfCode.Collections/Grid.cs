using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using AdventOfCode.Collections.DebugViews;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enums;
using AdventOfCode.Utils.Extensions.Numbers;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Utils.Extensions.Strings;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Enumerables;
using JetBrains.Annotations;
using ZLinq;

namespace AdventOfCode.Collections;

/// <summary>
/// Grid movement wrap flags
/// </summary>
[Flags]
public enum Wrap
{
    NONE       = 0b00,
    VERTICAL   = 0b01,
    HORIZONTAL = 0b10,
    BOTH       = 0b11
}

/// <summary>
/// Generic grid structure
/// </summary>
/// <typeparam name="T">Type of element within the grid</typeparam>
[PublicAPI, DebuggerDisplay("Width = {Width}, Height = {Height}"), DebuggerTypeProxy(typeof(GridDebugView<>))]
public class Grid<T> : IGrid<T>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.grid[y, x];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.grid[y.GetOffset(this.Height), x.GetOffset(this.Width)];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.grid[vector.Y, vector.X];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.grid[vector.Y, vector.X] = value;
    }

    /// <summary>
    /// Accesses an element in the grid
    /// </summary>
    /// <param name="tuple">Position tuple in the grid</param>
    /// <returns>The element at the specified position</returns>
    public virtual T this[(int x, int y) tuple]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.grid[tuple.y, tuple.x];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.grid[tuple.y, tuple.x] = value;
    }

    /// <summary>
    /// Gets the given row of the grid<br/>
    /// </summary>
    /// <param name="row">Row index of the row to get</param>
    /// <returns>The specified row of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="row"/> is not within the limits of the Grid</exception>
    public virtual Span<T> this[int row]
    {
        get
        {
            if (row < 0 || row >= this.Height) throw new ArgumentOutOfRangeException(nameof(row), row, "Row index must be within limits of Grid");
            return this.grid.GetRowSpan(row);
        }
    }

    /// <summary>
    /// Gets the given row of the grid<br/>
    /// </summary>
    /// <param name="row">Row index of the row to get</param>
    /// <returns>The specified row of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="row"/> is not within the limits of the Grid</exception>
    public virtual Span<T> this[Index row] => this[row.GetOffset(this.Height)];

    /// <summary>
    /// Gets a slice of this Grid in a certain range
    /// </summary>
    /// <param name="xRange">X range of the slice</param>
    /// <param name="yRange">Y range of the slice</param>
    /// <returns>The grid slice</returns>
    /// <exception cref="ArgumentOutOfRangeException">If any part of the slice is outside of the grid's range</exception>
    /// <exception cref="InvalidOperationException">If the grid slice is of size 0 in one dimension</exception>
    public virtual ReadOnlySpan2D<T> this[Range xRange, Range yRange]
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
    public Grid(int width, int height, string[] input, [InstantHandle] Converter<string, T[]> converter, Converter<T, string>? toString = null)
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
    public Grid(ReadOnlySpan2D<T> span) : this(span.Width, span.Height)
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

    /// <inheritdoc />
    public void CopyFrom(IGrid<T> other)
    {
        if (other is Grid<T> otherGrid)
        {
            CopyFrom(otherGrid);
            return;
        }

        foreach ((Vector2<int> position, T element) in other.EnumeratePositions())
        {
            this[position] = element;
        }
    }

    /// <summary>
    /// Copies the data from another Grid into this one
    /// </summary>
    /// <param name="span">Span to copy from</param>
    public void CopyFrom(ReadOnlySpan2D<T> span)
    {
        if (span.Width != this.Width || span.Height != this.Height) throw new InvalidOperationException("Cannot copy two grids with different sizes");

        span.CopyTo(this.grid);
    }

    /// <summary>
    /// Creates a new Span2D over the grid
    /// </summary>
    /// <returns>Span over the entire grid</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan2D<T> AsSpan2D() => this.grid.AsSpan2D();

    /// <summary>
    /// Creates a new Span2D over a section of the grid
    /// </summary>
    /// <param name="width">Span width, from the top left corner</param>
    /// <param name="height">Span height, from the top left corner</param>
    /// <returns>Span over the specified part of the grid</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan2D<T> AsSpan2D(int width, int height) => this.grid.AsSpan2D(0, 0, height, width);

    /// <summary>
    /// Creates a new Span2D over a section of the grid
    /// </summary>
    /// <param name="column">Span column start, from the top left corner</param>
    /// <param name="width">Span width, from the top left corner</param>
    /// <param name="row">Span row start, from the top left corner</param>
    /// <param name="height">Span height, from the top left corner</param>
    /// <returns>Span over the specified part of the grid</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan2D<T> AsSpan2D(int column, int width, int row, int height) => this.grid.AsSpan2D(row, column, height, width);

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RefEnumerable<T> GetColumn(Index x) => GetColumn(x.GetOffset(this.Width));

    /// <summary>
    /// Gets the given column of the grid without allocations
    /// </summary>
    /// <param name="x">Column index of the column to get</param>
    /// <param name="column">Array in which to store the column data</param>
    /// <returns>The specified column of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="x"/> is not within the limits of the Grid</exception>
    /// <exception cref="ArgumentException">If <paramref name="column"/> is too small to fit the size of the column</exception>
    public void GetColumn(int x, Span<T> column)
    {
        if (x < 0 || x >= this.Width) throw new ArgumentOutOfRangeException(nameof(x), x, "Column index must be within limits of Grid");
        if (column.Length < this.Height) throw new ArgumentException("Pre allocated column array is too small", nameof(column));

        this.grid.GetColumn(x).CopyTo(column);
    }

    /// <inheritdoc cref="GetColumn(int, Span{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetColumn(Index x, Span<T> column) => GetColumn(x.GetOffset(this.Width), column);

    /// <summary>
    /// Set the given row of the grid by the specified array
    /// </summary>
    /// <param name="x">Column index</param>
    /// <param name="column">Column values</param>
    /// <exception cref="ArgumentOutOfRangeException">If the column index is out of bounds of the grid</exception>
    /// <exception cref="ArgumentException">If the column values array is larger than the width of the grid</exception>
    public virtual void SetColumn(int x, ReadOnlySpan<T> column)
    {
        if (x < 0 || x >= this.Width) throw new ArgumentOutOfRangeException(nameof(x), x, "Column index must be within limits of Grid");
        if (column.Length != this.Height) throw new ArgumentException("Column is not the same width as grid", nameof(column));

        column.CopyTo(this.grid.GetColumn(x));
    }

    /// <inheritdoc cref="SetColumn(int, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void SetColumn(Index x, ReadOnlySpan<T> column) => SetColumn(x.GetOffset(this.Width), column);

    /// <summary>
    /// Fill the grid with the given value
    /// </summary>
    /// <param name="value">Value to fill with</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fill(T value) => this.grid.AsSpan2D().Fill(value);

    /// <summary>
    /// Tries to get a value in the grid at the given position
    /// </summary>
    /// <param name="position">Position to get the value for</param>
    /// <param name="value">The value, if it was found</param>
    /// <returns><see langword="true"/> if the value was found, otherwise <see langword="false"/></returns>
    public virtual bool TryGetPosition(Vector2<int> position, [MaybeNullWhen(false)] out T value)
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
    /// <param name="wrap">If the vector should wrap around in some directions when going off the grid</param>
    /// <returns>The resulting Vector after the move, or null if the movement was invalid</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual Vector2<int>? MoveWithinGrid(Vector2<int> vector, Direction direction, Wrap wrap = Wrap.NONE)
    {
        return MoveWithinGrid(vector, direction.ToVector<int>(), wrap);
    }

    /// <summary>
    /// Moves the vector within the grid
    /// </summary>
    /// <param name="vector">Vector to move</param>
    /// <param name="travel">Vector to travel in</param>
    /// <param name="wrap">If the vector should wrap around in some directions when going off the grid</param>
    /// <returns>The resulting Vector after the move</returns>
    public virtual Vector2<int>? MoveWithinGrid(Vector2<int> vector, Vector2<int> travel, Wrap wrap = Wrap.NONE)
    {
        (int x, int y) result = vector + travel;

        //Wrap x axis
        if (wrap.HasFlags(Wrap.HORIZONTAL))
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
        if (wrap.HasFlags(Wrap.VERTICAL))
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
    /// <param name="wrap">If the vector should wrap around in some directions when going off the grid</param>
    /// <returns><see langword="true"/> if the move succeeded, else <see langword="false"/></returns>
    public virtual bool TryMoveWithinGrid(Vector2<int> vector, Direction direction, out Vector2<int> moved, Wrap wrap = Wrap.NONE)
    {
        Vector2<int>? move = MoveWithinGrid(vector, direction, wrap);
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
    /// <param name="wrap">If the vector should wrap around in some directions when going off the grid</param>
    /// <returns><see langword="true"/> if the move succeeded, else <see langword="false"/></returns>
    public virtual bool TryMoveWithinGrid(Vector2<int> vector, Vector2<int> travel, out Vector2<int> moved, Wrap wrap = Wrap.NONE)
    {
        Vector2<int>? move = MoveWithinGrid(vector, travel, wrap);
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
    public virtual bool WithinGrid(Vector2<int> position)
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

        throw new InvalidOperationException($"Value {value} could not be found");
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void Clear() => Array.Clear(this.grid);

    /// <inheritdoc />
    public IEnumerable<GridPosition<T>> EnumeratePositions()
    {
        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                Vector2<int> position = new(x, y);
                yield return new GridPosition<T>(position, this[position]);
            }
        }
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span2D<T>.Enumerator GetEnumerator() => this.grid.AsSpan2D().GetEnumerator();

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.grid.Cast<T>().GetEnumerator();

    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
}
