using System;
using System.Collections.Generic;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using SpanLinq;

namespace AdventOfCode.Solvers.AoC2024
{
    /// <summary>
    /// Solver for 2024 Day 09
    /// </summary>
    public class Day09 : Solver<string>
    {
        private record struct Block(int Id, int Size, int Gap);

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day09"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
        public Day09(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            // Store locally in span
            Span<int> fileSystem = stackalloc int[this.Data.Length];
            foreach (int i in ..fileSystem.Length)
            {
                fileSystem[i] = this.Data[i] - '0';
            }

            // Setup for checksum calculation
            long checksum = 0L;
            int headIndex = 0;
            int tailIndex = fileSystem.Length - 1;
            int remainingTail = fileSystem[tailIndex];
            int blockIndex = 0;
            int blockId = 0;
            while (headIndex < tailIndex)
            {
                // Checksum the head block
                blockId = headIndex / 2;
                for (int blockEnd = blockIndex + fileSystem[headIndex]; blockIndex < blockEnd; blockIndex++)
                {
                    checksum += blockId * blockIndex;
                }

                // Move the tail block to the current gap
                headIndex++;
                blockId = tailIndex / 2;
                for (int blockEnd = blockIndex + fileSystem[headIndex]; blockIndex < blockEnd; blockIndex++)
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
                        tailIndex   -= 2;
                        blockId = tailIndex / 2;
                        if (headIndex >= tailIndex)
                        {
                            // Rejoined with head
                            remainingTail = 0;
                            break;
                        }
                        remainingTail = fileSystem[tailIndex];
                    }
                }
                headIndex++;
            }

            // Checksum remaining tail block
            while (remainingTail --> 0)
            {
                checksum += blockId * blockIndex++;
            }

            AoCUtils.LogPart1(checksum);

            // Make file system into linked list
            LinkedList<Block> linkedFileSystem = [];
            LinkedListNode<Block>[] blockNodes = new LinkedListNode<Block>[(fileSystem.Length + 1) / 2];
            for (int i = 0; i < fileSystem.Length - 1; i++)
            {
                int id   = i / 2;
                int size = fileSystem[i++];
                int gap  = fileSystem[i];
                blockNodes[id] = linkedFileSystem.AddLast(new Block(id, size, gap));
            }
            blockNodes[^1] = linkedFileSystem.AddLast(new Block(blockNodes.Length - 1, fileSystem[^1], 0));

            tailIndex = blockNodes.Length - 1;
            LinkedListNode<Block> headBlock = linkedFileSystem.First!;
            HashSet<int> filledBlocks = new(100) { headBlock.ValueRef.Id };
            for (LinkedListNode<Block> tailBlock = blockNodes[tailIndex]; !filledBlocks.Contains(tailBlock.ValueRef.Id); tailBlock = blockNodes[--tailIndex])
            {
                // While the head has no gap, move along the list
                ref Block tail = ref tailBlock.ValueRef;

                // Find where we can insert the current block
                for (LinkedListNode<Block> insertionTarget = headBlock; insertionTarget.ValueRef.Id != tail.Id; insertionTarget = insertionTarget.Next!)
                {
                    ref Block target = ref insertionTarget.ValueRef;
                    if (target.Gap >= tail.Size)
                    {
                        // Increase gap on the block before where the tail is being removed
                        ref Block beforeTail = ref tailBlock.Previous!.ValueRef;
                        beforeTail.Gap += tail.Size + tail.Gap;

                        // Set the gap on the tail to what is left after insertion
                        tail.Gap   =  target.Gap - tail.Size;
                        target.Gap =  0;

                        // Move node to it's new location
                        linkedFileSystem.Remove(tailBlock);
                        linkedFileSystem.AddAfter(insertionTarget, tailBlock);
                        break;
                    }
                }

                while (headBlock.ValueRef.Gap is 0)
                {
                    headBlock = headBlock.Next!;
                    filledBlocks.Add(headBlock.ValueRef.Id);
                }
            }

            checksum   = 0L;
            blockIndex = 0;
            foreach (Block block in linkedFileSystem)
            {
                int endIndex = blockIndex + block.Size;
                int idSum    = (endIndex - 1).Triangular() - (blockIndex - 1).Triangular();
                checksum    += idSum * block.Id;
                blockIndex   = endIndex + block.Gap;
            }

            AoCUtils.LogPart2(fileSystem.Sum());
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override string Convert(string[] rawInput) => rawInput[0];
        #endregion
    }
}

