using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Maths.Vectors.BitVectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using ZLinq;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 17
/// </summary>
public sealed class Day17 : Solver<string>
{
    private readonly record struct PathData(Vector2<int> Position, string Path);

    private const int GRID_SIZE = 4;
    private static readonly Vector2<int> VaultPosition = new(GRID_SIZE - 1, GRID_SIZE - 1);
    private static readonly ImmutableArray<char> DirectionChar = ['U', 'D', 'L', 'R'];
    private static readonly byte[] HashBuffer = new byte[MD5.HashSizeInBytes];
    private static readonly char[] HexBuffer  = new char[Direction.CardinalDirections.Length];
    private static readonly bool[] BitArray   = new bool[Direction.CardinalDirections.Length];

    private readonly MD5 md5 = MD5.Create();

    /// <summary>
    /// Creates a new <see cref="Day17"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day17(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        IEnumerable<string> pathsEnumerable = GetValidPaths();
        // ReSharper disable once PossibleMultipleEnumeration
        AoCUtils.LogPart1(pathsEnumerable.First());
        // ReSharper disable once PossibleMultipleEnumeration
        AoCUtils.LogPart2(pathsEnumerable.Last().Length);
    }

    // ReSharper disable once CognitiveComplexity
    private IEnumerable<string> GetValidPaths()
    {
        Queue<PathData> path = new(100);
        path.Enqueue(new PathData(Vector2<int>.Zero, this.Data));
        while (path.TryDequeue(out PathData data))
        {
            BitVector8 locks = GetLocks(data.Path);
            for (int i = 0; i < Direction.CardinalDirections.Length; i++)
            {
                // Check the lock
                if (!locks[i]) continue;

                // Check if the move is valid
                Vector2<int> newPosition = data.Position + Direction.CardinalDirections[i];
                if (newPosition is { X: < 0 or >= GRID_SIZE } or { Y: < 0 or >= GRID_SIZE }) continue;

                string newPath = data.Path + DirectionChar[i];
                if (newPosition == VaultPosition)
                {
                    yield return newPath[this.Data.Length..];
                    continue;
                }

                path.Enqueue(new PathData(newPosition, newPath));
            }
        }
    }

    private BitVector8 GetLocks(ReadOnlySpan<char> path)
    {
        // Get path bytes
        Span<byte> data = stackalloc byte[Encoding.UTF8.GetByteCount(path)];
        Encoding.UTF8.TryGetBytes(path, data, out _);
        // Compute MD5
        this.md5.TryComputeHash(data, HashBuffer, out _);
        // Convert to lowercase hex string
        System.Convert.TryToHexStringLower(HashBuffer.AsSpan(0, 2), HexBuffer, out _);
        // Convert to locked/unlocked array
        HexBuffer.AsValueEnumerable()
                 .Select(c => c is >= 'b' and <= 'f')
                 .CopyTo(BitArray);
        // Convert to bit vector
        return BitVector8.FromBitArray(BitArray);
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => rawInput[0];

    /// <inheritdoc />
    public override void Dispose()
    {
        this.md5.Dispose();
        base.Dispose();
    }
}
