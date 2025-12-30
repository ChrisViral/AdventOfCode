using System.Runtime.CompilerServices;
using AdventOfCode.Maths.Vectors;
using CommunityToolkit.HighPerformance;
using JetBrains.Annotations;
using ZLinq;

[assembly: ZLinqDropInExternalExtension("AdventOfCode.Collections", "AdventOfCode.Collections.DelayedGrid`1", "AdventOfCode.Utils.ValueEnumerators.FromSpan2D`1", GenerateAsPublic = true)]

namespace AdventOfCode.Collections;

/// <summary>
/// Delayed grid which does not apply changes until requested
/// </summary>
/// <typeparam name="T">Grid element type</typeparam>
[PublicAPI]
public sealed class DelayedGrid<T> : Grid<T>
{
    /// <summary>Backup array</summary>
    private readonly T[,] backupGrid;

    /// <inheritdoc />
    public override T this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.backupGrid[y, x] = value;
    }

    /// <inheritdoc />
    public override T this[Index x, Index y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.backupGrid[y.GetOffset(this.Height), x.GetOffset(this.Width)] = value;
    }

    /// <inheritdoc />
    public override T this[Vector2<int> vector]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.backupGrid[vector.Y, vector.X] = value;
    }

    /// <inheritdoc />
    public override T this[(int x, int y) tuple]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.backupGrid[tuple.y, tuple.x] = value;
    }

    /// <summary>
    /// Gets the given row of the grid<br/>
    /// </summary>
    /// <param name="row">Row index of the row to get</param>
    /// <returns>The specified row of the grid</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="row"/> is not within the limits of the Grid</exception>
    public override Span<T> this[int row]
    {
        get
        {
            if (row < 0 || row >= this.Height) throw new ArgumentOutOfRangeException(nameof(row), row, "Row index must be within limits of Grid");
            return this.backupGrid.GetRowSpan(row);
        }
    }

    /// <inheritdoc />
    public DelayedGrid(int width, int height, Converter<T, string>? toString = null) : base(width, height, toString)
    {
        this.backupGrid = new T[height, width];
    }

    /// <inheritdoc />
    public DelayedGrid(int width, int height, string[] input, Converter<string, T[]> converter, Converter<T, string>? toString = null) : base(width, height, input, converter, toString)
    {
        this.backupGrid = this.grid.AsSpan2D().ToArray();
    }

    /// <inheritdoc />
    public DelayedGrid(DelayedGrid<T> other) : base(other)
    {
        this.backupGrid = other.backupGrid.AsSpan2D().ToArray();
    }

    /// <inheritdoc />
    public DelayedGrid(Grid<T> other) : base(other)
    {
        this.backupGrid = this.grid.AsSpan2D().ToArray();
    }

    /// <inheritdoc />
    public DelayedGrid(ReadOnlySpan2D<T> span) : base(span)
    {
        this.backupGrid = span.ToArray();
    }

    /// <inheritdoc />
    public override void SetColumn(int x, ReadOnlySpan<T> column)
    {
        if (x < 0 || x >= this.Width) throw new ArgumentOutOfRangeException(nameof(x), x, "Column index must be within limits of Grid");
        if (column.Length != this.Height) throw new ArgumentException("Column is not the same width as grid", nameof(column));

        column.CopyTo(this.backupGrid.GetColumn(x));
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Clear()
    {
        base.Clear();
        Array.Clear(this.backupGrid);
    }

    /// <summary>
    /// Applies the delayed changes to the grid
    /// </summary>
    /// <param name="clear">If the backup grid should be cleared</param>
    public void Apply(bool clear = false)
    {
        this.backupGrid.AsSpan2D().CopyTo(this.grid);
        if (clear)
        {
            Array.Clear(this.backupGrid);
        }
    }
}
