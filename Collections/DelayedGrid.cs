using System.Runtime.CompilerServices;
using AdventOfCode.Vectors;
using CommunityToolkit.HighPerformance;
using JetBrains.Annotations;

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
    public override void SetRow(int y, T[] row)
    {
        if (y < 0 || y >= this.Height) throw new ArgumentOutOfRangeException(nameof(y), y, "Row index must be within limits of Grid");
        if (row.Length != this.Width) throw new ArgumentException("Row is not the same width as grid", nameof(row));

        if (this.rowBufferSize is not 0)
        {
            //For primitives only
            Buffer.BlockCopy(row, 0, this.backupGrid, y * this.rowBufferSize, this.rowBufferSize);
            return;
        }

        row.CopyTo(this.backupGrid.GetRowSpan(y));
    }

    /// <inheritdoc />
    public override void SetRow(int y, ReadOnlySpan<T> row)
    {
        if (y < 0 || y >= this.Height) throw new ArgumentOutOfRangeException(nameof(y), y, "Row index must be within limits of Grid");
        if (row.Length != this.Width) throw new ArgumentException("Row is not the same width as grid", nameof(row));

        row.CopyTo(this.backupGrid.GetRowSpan(y));
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
