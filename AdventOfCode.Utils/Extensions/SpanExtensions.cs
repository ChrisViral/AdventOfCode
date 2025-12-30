using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Utils.ValueEnumerators;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Enumerables;
using JetBrains.Annotations;
using ZLinq;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Utils.Extensions.Spans;

/// <summary>
/// Span extensions
/// </summary>
[PublicAPI]
public static class SpanExtensions
{
    /// <param name="span">Span instance</param>
    /// <typeparam name="T">Value contained in the span</typeparam>
    extension<T>(Span2D<T> span)
    {
        /// <summary>
        /// Converts this Span2D to a ValueEnumerable
        /// </summary>
        /// <returns>ValueEnumerable instance</returns>
        public ValueEnumerable<FromSpan2D<T>, T> AsValueEnumerable()
        {
            return new ValueEnumerable<FromSpan2D<T>, T>(new FromSpan2D<T>(span));
        }

        /// <summary>
        /// Rotates this span 90° clockwise into a destination span
        /// </summary>
        /// <param name="destination">Destination span to rotate this span into</param>
        /// <exception cref="InvalidOperationException">If the destination span's dimensions does not match the rotated dimensions of this span</exception>
        public void RotateRight(Span2D<T> destination)
        {
            if (span.Width < destination.Height || span.Height < destination.Width) throw new InvalidOperationException("Must rotate into correctly dimensioned span");

            foreach (int i in ..span.Height)
            {
                RefEnumerable<T> column = destination.GetColumn(destination.Width - i - 1);
                span.GetRowSpan(i).CopyTo(column);
            }
        }

        /// <summary>
        /// Rotates this span 90° counter-clockwise into a destination span
        /// </summary>
        /// <param name="destination">Destination span to rotate this span into</param>
        /// <exception cref="InvalidOperationException">If the destination span's dimensions does not match the rotated dimensions of this span</exception>
        public void RotateLeft(Span2D<T> destination)
        {
            if (span.Width < destination.Height || span.Height < destination.Width) throw new InvalidOperationException("Must rotate into correctly dimensioned span");

            foreach (int i in ..span.Height)
            {
                Span<T> row = destination.GetRowSpan(destination.Height - i - 1);
                span.GetColumn(i).CopyTo(row);
            }
        }

        /// <summary>
        /// Rotates this span 180° into a destination span
        /// </summary>
        /// <param name="destination">Destination span to rotate this span into</param>
        /// <exception cref="InvalidOperationException">If the destination span's dimensions does not match the rotated dimensions of this span</exception>
        public void RotateHalf(Span2D<T> destination)
        {
            if (span.Width < destination.Width || span.Height < destination.Height) throw new InvalidOperationException("Must rotate into correctly dimensioned span");

            foreach (int i in ..span.Height)
            {
                Span<T> row = destination.GetRowSpan(destination.Height - i - 1);
                span.GetRowSpan(i).CopyTo(row);
                row.Reverse();
            }
        }
        /// <summary>
        /// Flips this span on it's vertical axis
        /// </summary>
        /// <param name="destination">Destination span to flip this span into</param>
        /// <exception cref="InvalidOperationException">If the destination span's dimensions does not match the flipped dimensions of this span</exception>
        public void FlipVertical(Span2D<T> destination)
        {
            if (span.Width < destination.Width || span.Height < destination.Height) throw new InvalidOperationException("Must rotate into correctly dimensioned span");

            foreach (int i in ..span.Height)
            {
                Span<T> row = destination.GetRowSpan(destination.Height - i - 1);
                span.GetRowSpan(i).CopyTo(row);
            }
        }

        /// <summary>
        /// Flips this span on it's horizontal axis
        /// </summary>
        /// <param name="destination">Destination span to flip this span into</param>
        /// <exception cref="InvalidOperationException">If the destination span's dimensions does not match the flipped dimensions of this span</exception>
        public void FlipHorizontal(Span2D<T> destination)
        {
            if (span.Width < destination.Width || span.Height < destination.Height) throw new InvalidOperationException("Must rotate into correctly dimensioned span");

            foreach (int i in ..span.Width)
            {
                RefEnumerable<T> column = destination.GetColumn(destination.Width - i - 1);
                span.GetColumn(i).CopyTo(column);
            }
        }

        /// <summary>
        /// Transposes the x and y coordinates of this span
        /// </summary>
        /// <param name="destination">Destination span to transpose this span into</param>
        /// <exception cref="InvalidOperationException">If the destination span's dimensions does not match the transposed dimensions of this span</exception>
        public void Transpose(Span2D<T> destination)
        {
            if (span.Width < destination.Height || span.Height < destination.Width) throw new InvalidOperationException("Must transpose into correctly dimensioned span");

            foreach (int i in ..span.Height)
            {
                RefEnumerable<T> column = destination.GetColumn(i);
                span.GetRowSpan(i).CopyTo(column);
            }
        }
    }

    /// <param name="span">Span instance</param>
    /// <typeparam name="T">Value contained in the span</typeparam>
    extension<T>(ReadOnlySpan2D<T> span)
    {
        /// <summary>
        /// Converts this Span2D to a ValueEnumerable
        /// </summary>
        /// <returns>ValueEnumerable instance</returns>
        public ValueEnumerable<FromSpan2D<T>, T> AsValueEnumerable()
        {
            return new ValueEnumerable<FromSpan2D<T>, T>(new FromSpan2D<T>(span));
        }

        /// <summary>
        /// Rotates this span 90° clockwise into a destination span
        /// </summary>
        /// <param name="destination">Destination span to rotate this span into</param>
        /// <exception cref="InvalidOperationException">If the destination span's dimensions does not match the rotated dimensions of this span</exception>
        public void RotateRight(Span2D<T> destination)
        {
            if (span.Width < destination.Height || span.Height < destination.Width) throw new InvalidOperationException("Must rotate into correctly dimensioned span");

            foreach (int i in ..span.Height)
            {
                RefEnumerable<T> column = destination.GetColumn(destination.Width - i - 1);
                span.GetRowSpan(i).CopyTo(column);
            }
        }

        /// <summary>
        /// Rotates this span 90° counter-clockwise into a destination span
        /// </summary>
        /// <param name="destination">Destination span to rotate this span into</param>
        /// <exception cref="InvalidOperationException">If the destination span's dimensions does not match the rotated dimensions of this span</exception>
        public void RotateLeft(Span2D<T> destination)
        {
            if (span.Width < destination.Height || span.Height < destination.Width) throw new InvalidOperationException("Must rotate into correctly dimensioned span");

            foreach (int i in ..span.Height)
            {
                Span<T> row = destination.GetRowSpan(destination.Height - i - 1);
                span.GetColumn(i).CopyTo(row);
            }
        }

        /// <summary>
        /// Rotates this span 180° into a destination span
        /// </summary>
        /// <param name="destination">Destination span to rotate this span into</param>
        /// <exception cref="InvalidOperationException">If the destination span's dimensions does not match the rotated dimensions of this span</exception>
        public void RotateHalf(Span2D<T> destination)
        {
            if (span.Width < destination.Width || span.Height < destination.Height) throw new InvalidOperationException("Must rotate into correctly dimensioned span");

            foreach (int i in ..span.Height)
            {
                Span<T> row = destination.GetRowSpan(destination.Height - i - 1);
                span.GetRowSpan(i).CopyTo(row);
                row.Reverse();
            }
        }
        /// <summary>
        /// Flips this span on it's vertical axis
        /// </summary>
        /// <param name="destination">Destination span to flip this span into</param>
        /// <exception cref="InvalidOperationException">If the destination span's dimensions does not match the flipped dimensions of this span</exception>
        public void FlipVertical(Span2D<T> destination)
        {
            if (span.Width < destination.Width || span.Height < destination.Height) throw new InvalidOperationException("Must rotate into correctly dimensioned span");

            foreach (int i in ..span.Height)
            {
                Span<T> row = destination.GetRowSpan(destination.Height - i - 1);
                span.GetRowSpan(i).CopyTo(row);
            }
        }

        /// <summary>
        /// Flips this span on it's horizontal axis
        /// </summary>
        /// <param name="destination">Destination span to flip this span into</param>
        /// <exception cref="InvalidOperationException">If the destination span's dimensions does not match the flipped dimensions of this span</exception>
        public void FlipHorizontal(Span2D<T> destination)
        {
            if (span.Width < destination.Width || span.Height < destination.Height) throw new InvalidOperationException("Must rotate into correctly dimensioned span");

            foreach (int i in ..span.Width)
            {
                RefEnumerable<T> column = destination.GetColumn(destination.Width - i - 1);
                span.GetColumn(i).CopyTo(column);
            }
        }

        /// <summary>
        /// Transposes the x and y coordinates of this span
        /// </summary>
        /// <param name="destination">Destination span to transpose this span into</param>
        /// <exception cref="InvalidOperationException">If the destination span's dimensions does not match the transposed dimensions of this span</exception>
        public void Transpose(Span2D<T> destination)
        {
            if (span.Width < destination.Height || span.Height < destination.Width) throw new InvalidOperationException("Must transpose into correctly dimensioned span");

            foreach (int i in ..span.Height)
            {
                RefEnumerable<T> column = destination.GetColumn(i);
                span.GetRowSpan(i).CopyTo(column);
            }
        }
    }
}
