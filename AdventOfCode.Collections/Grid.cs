using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using AdventOfCode.Collections.DebugViews;
using AdventOfCode.Collections.Pooling;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enums;
using AdventOfCode.Utils.Extensions.Numbers;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Utils.Extensions.Spans;
using AdventOfCode.Utils.ValueEnumerators;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Enumerables;
using JetBrains.Annotations;
using ZLinq;

[assembly: ZLinqDropInExternalExtension("AdventOfCode.Collections", "AdventOfCode.Collections.Grid`1", "AdventOfCode.Utils.ValueEnumerators.FromSpan2D`1", GenerateAsPublic = true)]

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
    public virtual ReadOnlySpan<T> this[int row]
    {
        get
        {
            if (row < 0 || row >= this.Height) throw new ArgumentOutOfRangeException(nameof(row), row, "Row index must be within limits of Grid");
            return this.grid.GetRowSpan(row);
        }
        set
        {
            if (row < 0 || row >= this.Height) throw new ArgumentOutOfRangeException(nameof(row), row, "Row index must be within limits of Grid");
            if (value.Length != this.Width) throw new ArgumentException("Assigned span must be of the same length as grid width", nameof(value));
            value.CopyTo(this.grid.GetRowSpan(row));
        }
    }

    /// <summary>
    /// Gets the given row of the grid<br/>
    /// </summary>
    /// <param name="row">Row index of the row to get</param>
    /// <returns>The specified row of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="row"/> is not within the limits of the Grid</exception>
    public virtual ReadOnlySpan<T> this[Index row]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this[row.GetOffset(this.Height)];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this[row.GetOffset(this.Height)] = value;
    }

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
    public Span2D<T> AsSpan2D() => this.grid.AsSpan2D();

    /// <summary>
    /// Creates a new Span2D over a section of the grid
    /// </summary>
    /// <param name="width">Span width, from the top left corner</param>
    /// <param name="height">Span height, from the top left corner</param>
    /// <returns>Span over the specified part of the grid</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span2D<T> AsSpan2D(int width, int height) => this.grid.AsSpan2D(0, 0, height, width);

    /// <summary>
    /// Creates a new Span2D over a section of the grid
    /// </summary>
    /// <param name="column">Span column start, from the top left corner</param>
    /// <param name="width">Span width, from the top left corner</param>
    /// <param name="row">Span row start, from the top left corner</param>
    /// <param name="height">Span height, from the top left corner</param>
    /// <returns>Span over the specified part of the grid</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span2D<T> AsSpan2D(int column, int width, int row, int height) => this.grid.AsSpan2D(row, column, height, width);

    /// <summary>
    /// Gets the given column of the grid
    /// </summary>
    /// <param name="y">Column index of the column to get</param>
    /// <returns>The specified column of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="y"/> is not within the limits of the Grid</exception>
    public Span<T> GetRow(int y)
    {
        if (y < 0 || y >= this.Height) throw new ArgumentOutOfRangeException(nameof(y), y, "Row index must be within limits of Grid");
        return this.grid.GetRowSpan(y);
    }

    /// <inheritdoc cref="GetColumn(int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RefEnumerable<T> GetRow(Index y) => GetColumn(y.GetOffset(this.Height));

    /// <summary>
    /// Gets the given column of the grid
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
    /// Splits the grid into equally sized chunks
    /// </summary>
    /// <param name="chunkSize">Chunk size</param>
    /// <returns>An enumerable of the grid in chunks</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="chunkSize"/> is less than zero or greater than the width/height</exception>
    /// <exception cref="ArgumentException">If <paramref name="chunkSize"/> is not a factor of the width/height</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueEnumerable<FromChunks, Grid<T>> Chunk(int chunkSize)
    {
        return new ValueEnumerable<FromChunks, Grid<T>>(new FromChunks(this, chunkSize, chunkSize));
    }

    /// <summary>
    /// Splits the grid into equally sized chunks
    /// </summary>
    /// <param name="chunkWidth">Chunk width</param>
    /// <param name="chunkHeight">Chunk height</param>
    /// <returns>An enumerable of the grid in chunks</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="chunkWidth"/> or <paramref name="chunkHeight"/> are less than zero or greater than the width/height</exception>
    /// <exception cref="ArgumentException">If <paramref name="chunkWidth"/> or <paramref name="chunkHeight"/> are not factors of the width/height</exception>
    /// ReSharper disable once CognitiveComplexity
    public ValueEnumerable<FromChunks, Grid<T>> Chunk(int chunkWidth, int chunkHeight)
    {
        return new ValueEnumerable<FromChunks, Grid<T>>(new FromChunks(this, chunkWidth, chunkHeight));
    }

    /// <summary>
    /// Combines the given list of chunks into one big grid
    /// </summary>
    /// <param name="chunks">Chunks to combine</param>
    /// <param name="size">Combined width/hieight</param>
    /// <returns>A new grid of the specified dimensions, made of the combined chunks</returns>
    /// <exception cref="ArgumentException">Is <paramref name="chunks"/> is empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="size"/> is less than one</exception>
    /// <exception cref="InvalidOperationException">If the size of the chunks does not match with the width/height in some way</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Grid<T> CombineChunks(IReadOnlyList<Grid<T>> chunks, int size) => CombineChunks(chunks, size, size);

    /// <summary>
    /// Combines the given list of chunks into one big grid
    /// </summary>
    /// <param name="chunks">Chunks to combine</param>
    /// <param name="width">Combined width</param>
    /// <param name="height">Combined height</param>
    /// <returns>A new grid of the specified dimensions, made of the combined chunks</returns>
    /// <exception cref="ArgumentException">Is <paramref name="chunks"/> is empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="width"/> or <paramref name="height"/> are less than one</exception>
    /// <exception cref="InvalidOperationException">If the size of the chunks does not match with the width/height in some way</exception>
    /// ReSharper disable once CognitiveComplexity
    public static Grid<T> CombineChunks(IReadOnlyList<Grid<T>> chunks, int width, int height)
    {
        // Make sure we have some data
        if (chunks.Count is 0) throw new ArgumentException("Chunks list is empty");

        // Make sure our target dimensions are valid
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be greater than zero");
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), height, "Width must be greater than zero");

        // Check that the chunks fit within the target size
        Grid<T> first = chunks[0];
        int chunkWidth = first.Width;
        int chunkHeight = first.Height;
        if (chunkWidth > width || chunkHeight > height) throw new InvalidOperationException("Chunks are too big to combine into the specified width or height");
        if (!chunkWidth.IsFactor(width)) throw new InvalidOperationException("Chunks width not a factor of target width");
        if (!chunkHeight.IsFactor(height)) throw new InvalidOperationException("Chunks height not a factor of target height");
        if (!chunks.Skip(1).All(c => c.Width == chunkWidth && c.Height == chunkHeight)) throw new InvalidOperationException("Chunks size is not uniform");

        // Get the horizontal and vertical counts of chunks and make sure we have the right amount of items
        int xCount = width / chunkWidth;
        int yCount = height / chunkHeight;
        if (chunks.Count != xCount * yCount) throw new InvalidOperationException("Chunks list does not have the appropriate amount of items to properly combine");

        // Trivial case
        if (xCount is 1 && yCount is 1) return first;

        int i = 0;
        Grid<T> combined = new(width, height, first.toString);
        for (int y = 0; y < height; y += chunkHeight)
        {
            for (int x = 0; x < width; x += chunkWidth)
            {
                // Combine into bigger grid
                Grid<T> chunk = chunks[i++];
                chunk.AsSpan2D().CopyTo(combined.AsSpan2D(x, chunkWidth, y, chunkHeight));
            }
        }
        return combined;
    }
    /// <summary>
    /// Creates a new grid rotated to the right
    /// </summary>
    /// <returns>A copy of the grid rotated to the right</returns>
    public Grid<T> RotateRight()
    {
        Grid<T> rotated = new(this.Height, this.Width, this.toString);
        AsSpan2D().RotateRight(rotated.AsSpan2D());
        return rotated;
    }

    /// <summary>
    /// Creates a new grid rotated to the left
    /// </summary>
    /// <returns>A copy of the grid rotated to the left</returns>
    public Grid<T> RotateLeft()
    {
        Grid<T> rotated = new(this.Height, this.Width, this.toString);
        AsSpan2D().RotateLeft(rotated.AsSpan2D());
        return rotated;
    }

    /// <summary>
    /// Creates a new grid rotated a half turn
    /// </summary>
    /// <returns>A copy of the grid rotated to the left</returns>
    public Grid<T> RotateHalf()
    {
        Grid<T> rotated = new(this.Width, this.Height, this.toString);
        AsSpan2D().RotateHalf(rotated.AsSpan2D());
        return rotated;
    }

    /// <summary>
    /// Creates a new grid flipped vertically
    /// </summary>
    /// <returns>A copy of the grid flipped verticallyt</returns>
    public Grid<T> FlipVertical()
    {
        Grid<T> rotated = new(this.Width, this.Height, this.toString);
        AsSpan2D().FlipVertical(rotated.AsSpan2D());
        return rotated;
    }

    /// <summary>
    /// Creates a new grid flipped horizontally
    /// </summary>
    /// <returns>A copy of the grid flipped horizontally</returns>
    public Grid<T> FlipHorizontal()
    {
        Grid<T> rotated = new(this.Width, this.Height, this.toString);
        AsSpan2D().FlipHorizontal(rotated.AsSpan2D());
        return rotated;
    }

    /// <summary>
    /// Creates a new grid rotated a half turn
    /// </summary>
    /// <returns>A copy of the grid rotated to the left</returns>
    public Grid<T> Transpose()
    {
        Grid<T> rotated = new(this.Height, this.Width, this.toString);
        AsSpan2D().Transpose(rotated.AsSpan2D());
        return rotated;
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

    /// <summary>
    /// Converts this to a value enumerable
    /// </summary>
    /// <returns>Value enumerable over this grid</returns>
    public ValueEnumerable<FromSpan2D<T>, T> AsValueEnumerable()
    {
        return new ValueEnumerable<FromSpan2D<T>, T>(new FromSpan2D<T>(AsSpan2D()));
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
        using Pooled<StringBuilder> sb = StringBuilderObjectPool.Shared.Get();
        foreach (int j in ..this.Height)
        {
            foreach (int i in ..this.Width)
            {
                sb.Ref.Append(this.toString(this[i, j]));
            }
            sb.Ref.AppendLine();
        }

        return sb.Ref.ToString(0, sb.Ref.Length - Environment.NewLine.Length);
    }

    /// <summary>
    /// Grid chunk enumerator
    /// </summary>
    public ref struct FromChunks : IValueEnumerator<Grid<T>>
    {
        private readonly Grid<T> grid;
        private readonly int chunkWidth;
        private readonly int chunkHeight;
        private int x;
        private int y;

        /// <summary>
        /// Creates a new ChunkEnumerator
        /// </summary>
        /// <param name="grid">Grid to chunk up</param>
        /// <param name="chunkWidth">Chunk width</param>
        /// <param name="chunkHeight">Chunk height</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="chunkWidth"/> or <paramref name="chunkHeight"/> are less than zero or greater than the width/height</exception>
        /// <exception cref="ArgumentException">If <paramref name="chunkWidth"/> or <paramref name="chunkHeight"/> are not factors of the width/height</exception>
        public FromChunks(Grid<T> grid, int chunkWidth, int chunkHeight)
        {
            // Make sure we have a multiple of the size in each direction
            if (chunkWidth <= 0 || chunkWidth > grid.Width) throw new ArgumentOutOfRangeException(nameof(chunkWidth), chunkWidth, "Chunk width must be greater than zero and less than the grid's width");
            if (chunkHeight <= 0 || chunkHeight > grid.Height) throw new ArgumentOutOfRangeException(nameof(chunkHeight), chunkHeight, "Chunk height must be greater than zero and less than the grid's height");
            if (!chunkWidth.IsFactor(grid.Width)) throw new ArgumentException("Chunk width must be a factor of grid width", nameof(chunkWidth));
            if (!chunkHeight.IsFactor(grid.Height)) throw new ArgumentException("Chunk height must be a factor of grid height", nameof(chunkHeight));

            this.grid        = grid;
            this.chunkWidth  = chunkWidth;
            this.chunkHeight = chunkHeight;
        }

        /// <inheritdoc />
        public bool TryGetNext(out Grid<T> current)
        {
            // End condition
            if (this.y == this.grid.Height)
            {
                current = null!;
                return false;
            }

            // Create chunk
            current = new Grid<T>(this.chunkWidth, this.chunkHeight, this.grid.toString);
            ReadOnlySpan2D<T> source = this.grid.AsSpan2D(this.x, this.chunkWidth, this.y, this.chunkHeight);
            source.CopyTo(current.AsSpan2D());

            // Increment to next chunk
            this.x += this.chunkWidth;
            if (this.x == this.grid.Width)
            {
                this.x = 0;
                this.y += this.chunkHeight;
            }
            return true;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = (this.grid.Width / this.chunkWidth) * (this.grid.Height / this.chunkHeight);
            return true;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetSpan(out ReadOnlySpan<Grid<T>> span)
        {
            span = default;
            return false;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(scoped Span<Grid<T>> destination, Index offset) => false;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() { }
    }
}
