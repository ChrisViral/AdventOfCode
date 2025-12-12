using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using SpanLinq;

namespace AdventOfCode.AoC2024;

/// <summary>
/// Solver for 2024 Day 09
/// </summary>
public sealed class Day09 : Solver<string>
{
    /// <summary>
    /// Creates a new <see cref="Day09"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day09(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Store locally in span
        Span<int> fileChunks = stackalloc int[this.Data.Length + 1];
        foreach (int i in ..this.Data.Length)
        {
            fileChunks[i] = this.Data[i] - '0';
        }

        // Setup for checksum calculation
        long checksum = 0L;
        int tailIndex = fileChunks.Length - 2;
        int remainingTail = fileChunks[tailIndex];
        int blockIndex = 0;
        int blockId    = 0;
        for (int headIndex = 0; headIndex < tailIndex; headIndex++)
        {
            // Checksum the head block
            blockId = headIndex / 2;
            for (int blockEnd = blockIndex + fileChunks[headIndex]; blockIndex < blockEnd; blockIndex++)
            {
                checksum += blockId * blockIndex;
            }

            // Move the tail block to the current gap
            headIndex++;
            blockId = tailIndex / 2;
            for (int blockEnd = blockIndex + fileChunks[headIndex]; blockIndex < blockEnd; blockIndex++)
            {
                checksum += blockId * blockIndex;
                if (remainingTail > 1)
                {
                    // Reduce tail block size
                    remainingTail--;
                }
                else
                {
                    // Tail block depleted, move to next
                    tailIndex -= 2;
                    blockId   =  tailIndex / 2;
                    if (headIndex >= tailIndex)
                    {
                        // Rejoined with head
                        remainingTail = 0;
                        break;
                    }

                    remainingTail = fileChunks[tailIndex];
                }
            }
        }

        // Checksum remaining tail block
        while (remainingTail --> 0)
        {
            checksum += blockId * blockIndex++;
        }
        AoCUtils.LogPart1(checksum);

        // Create chunk ranges and filesystem
        int chunkIndex = 0;
        Span<Range> blocks = stackalloc Range[fileChunks.Length / 2];
        Span<int> fileSystem = stackalloc int[fileChunks.Sum()];
        for (int id = 0, i = 0; id < blocks.Length; id++)
        {
            // Make block range
            int size    = fileChunks[i++];
            Range block = new(chunkIndex, chunkIndex + size);
            blocks[id] = block;
            chunkIndex = block.End.Value + fileChunks[i++];

            // Fill filesystem with block
            fileSystem[block].Fill(id);
        }

        // Loop over all blocks backwards
        int packedIndex = blocks[0].End.Value;
        for (int i = blocks.Length - 1; packedIndex < blocks[i].Start.Value; i--)
        {
            // Check for gaps from the earliest known packed index
            ref Range block = ref blocks[i];
            int startIndex = packedIndex;
            while (startIndex < block.Start.Value)
            {
                // Get target span
                int endIndex = startIndex + (block.End.Value - block.Start.Value);
                Range targetBlock = startIndex..endIndex;
                Span<int> target = fileSystem[targetBlock];

                // Check if all values in target are 0
                if (OnlyHasZeroValues(target, out int nonZero))
                {
                    // Copy source span to target
                    Span<int> source = fileSystem[block];
                    source.CopyTo(target);

                    // Clear old region and set new block range
                    source.Clear();
                    block = targetBlock;

                    // If new start point was earliest packed index, move it forwards
                    if (startIndex == packedIndex)
                    {
                        packedIndex = endIndex;
                    }
                    break;
                }

                // If not, set the next start index from the end of the encroaching block
                startIndex = blocks[nonZero].End.Value;
            }
        }

        // Calculate checksum
        checksum = 0L;
        foreach (int i in ..fileSystem.Length)
        {
            checksum += i * fileSystem[i];
        }
        AoCUtils.LogPart2(checksum);
    }

    /// <summary>
    /// Checks if the given span has only zeroes or not
    /// </summary>
    /// <param name="span">Span to check</param>
    /// <param name="lastNonZero">The last non-zero value in the span, if any</param>
    /// <returns><see langword="true"/> if all values in the span were zero, otherwise <see langword="false"/></returns>
    private static bool OnlyHasZeroValues(in ReadOnlySpan<int> span, out int lastNonZero)
    {
        int i = span.Length - 1;
        lastNonZero = span[i];
        while (lastNonZero is 0)
        {
            if (i is 0) return true;
            lastNonZero = span[--i];
        }
        return false;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override string Convert(string[] rawInput) => rawInput[0];
}
