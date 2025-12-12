using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 20
/// </summary>
public sealed class Day20 : ArraySolver<long>
{
    /// <summary>Part 2 decryption key</summary>
    private const long DECRYPTION_KEY = 811589153;
    /// <summary>Part 2 decryption loops</summary>
    private const int  LOOPS          = 10;

    /// <summary>
    /// Creates a new <see cref="Day20"/> Solver for 2022 - 20 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day20(string input) : base(input) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        long grove = DecryptData(this.Data);
        AoCUtils.LogPart1(grove);

        grove = DecryptData(this.Data.Select(v => v * DECRYPTION_KEY), LOOPS);
        AoCUtils.LogPart2(grove);
    }

    // ReSharper disable once CognitiveComplexity
    private static long DecryptData(IEnumerable<long> data, int mixingCount = 1)
    {
        // Put the data into a linked list and extract the nodes
        LinkedList<long> mixing = new(data);
        LinkedListNode<long>[] nodes = mixing.ToNodeArray();

        // Loop the mixing algorithm
        foreach (int _ in ..mixingCount)
        {
            // Loop through the nodes in original order
            foreach (LinkedListNode<long> node in nodes)
            {
                // Keep the modulus to avoid wraparounds
                long value = node.Value % (mixing.Count - 1L);
                switch (value)
                {
                    // Move forwards
                    case > 0L:
                        LinkedListNode<long> target = node.NextCircular();
                        mixing.Remove(node);
                        for (long i = value - 1L; i > 0L; i--)
                        {
                            target = target.NextCircular();
                        }

                        mixing.AddAfter(target, node);
                        break;

                    // Move backwards
                    case < 0L:
                        target = node.PreviousCircular();
                        mixing.Remove(node);
                        for (long i = value + 1L; i < 0L; i++)
                        {
                            target = target.PreviousCircular();
                        }

                        mixing.AddBefore(target, node);
                        break;
                }
            }
        }

        // Get decrypted data
        long[] decrypted = new long[mixing.Count];
        mixing.CopyTo(decrypted, 0);
        // Find grove location
        int zero = decrypted.IndexOf(0);
        return decrypted[(zero + 1000) % decrypted.Length]
             + decrypted[(zero + 2000) % decrypted.Length]
             + decrypted[(zero + 3000) % decrypted.Length];
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override long ConvertLine(string line) => long.Parse(line);
}
