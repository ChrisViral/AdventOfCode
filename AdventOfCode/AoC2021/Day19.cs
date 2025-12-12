using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using Transformation = System.Func<AdventOfCode.Maths.Vectors.Vector3<int>, AdventOfCode.Maths.Vectors.Vector3<int>>;

namespace AdventOfCode.AoC2021;

/// <summary>
/// Solver for 2021 Day 19
/// </summary>
public sealed class Day19 : Solver<List<Vector3<int>[]>>
{
    private const int MATCHING = 12;
    private static readonly Transformation[] Rotations =
    [
        // Rotation around +Y
        v => new Vector3<int>( v.X,  v.Y,  v.Z),
        v => new Vector3<int>(-v.Z,  v.Y,  v.X),
        v => new Vector3<int>(-v.X,  v.Y, -v.Z),
        v => new Vector3<int>( v.Z,  v.Y, -v.X),

        // Rotation around -Y
        v => new Vector3<int>( v.X, -v.Y, -v.Z),
        v => new Vector3<int>( v.Z, -v.Y,  v.X),
        v => new Vector3<int>(-v.X, -v.Y,  v.Z),
        v => new Vector3<int>(-v.Z, -v.Y, -v.X),

        // Rotation around +X
        v => new Vector3<int>( v.Z,  v.X,  v.Y),
        v => new Vector3<int>(-v.Y,  v.X,  v.Z),
        v => new Vector3<int>(-v.Z,  v.X, -v.Y),
        v => new Vector3<int>( v.Y,  v.X, -v.Z),

        // Rotation around -X
        v => new Vector3<int>( v.Z, -v.X, -v.Y),
        v => new Vector3<int>( v.Y, -v.X,  v.Z),
        v => new Vector3<int>(-v.Z, -v.X,  v.Y),
        v => new Vector3<int>(-v.Y, -v.X, -v.Z),

        // Rotation around +Z
        v => new Vector3<int>( v.Y,  v.Z,  v.X),
        v => new Vector3<int>(-v.X,  v.Z,  v.Y),
        v => new Vector3<int>(-v.Y,  v.Z, -v.X),
        v => new Vector3<int>( v.X,  v.Z, -v.Y),

        // Rotation around -Z
        v => new Vector3<int>( v.Y, -v.Z, -v.X),
        v => new Vector3<int>( v.X, -v.Z,  v.Y),
        v => new Vector3<int>(-v.Y, -v.Z,  v.X),
        v => new Vector3<int>(-v.X, -v.Z, -v.Y)
    ];
    private static readonly List<Vector3<int>> Buffer = [];

    /// <summary>
    /// Creates a new <see cref="Day19"/> Solver for 2021 - 19 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day19(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        List<Vector3<int>> beacons       = [];
        HashSet<Vector3<int>> scanners   = new(this.Data.Count) { Vector3<int>.Zero };
        HashSet<Vector3<int>> allBeacons = new(this.Data[0]);
        this.Data.RemoveAt(0);
        while (!this.Data.IsEmpty)
        {
            foreach (int i in ..this.Data.Count)
            {
                bool found = false;
                foreach (Transformation transformation in Rotations)
                {
                    beacons.Clear();
                    beacons.AddRange(this.Data[i].Select(transformation));
                    if (CheckMatching(allBeacons, beacons, out Vector3<int> scanner))
                    {
                        scanners.Add(scanner);
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    this.Data.RemoveAt(i);
                    break;
                }
            }
        }

        AoCUtils.LogPart1(allBeacons.Count);

        int distance = scanners.Max(first => scanners.Max(second => Vector3<int>.ManhattanDistance(first, second)));
        AoCUtils.LogPart2(distance);
    }

    // ReSharper disable once CognitiveComplexity
    private static bool CheckMatching(HashSet<Vector3<int>> references, List<Vector3<int>> beacons, out Vector3<int> scanner)
    {
        foreach (Vector3<int> reference in references)
        {
            foreach (Vector3<int> beacon in beacons)
            {
                int matching = 0;
                Vector3<int> origin = reference - beacon;
                Buffer.Clear();
                foreach (Vector3<int> fromReference in beacons.Select(b => b + origin))
                {
                    Buffer.Add(fromReference);
                    if (references.Contains(fromReference))
                    {
                        matching++;
                    }
                }

                if (matching >= MATCHING)
                {
                    references.UnionWith(Buffer);
                    scanner = origin;
                    return true;
                }
            }
        }

        scanner = Vector3<int>.Zero;
        return false;
    }

    /// <inheritdoc />
    protected override List<Vector3<int>[]> Convert(string[] rawInput)
    {
        List<Vector3<int>[]> scanners = [];
        for (int start = 1, end = 1; start < rawInput.Length; start = end + 1, end = start)
        {
            while (++end < rawInput.Length && rawInput[end][..3] is not "---") { }
            scanners.Add(rawInput[start..end].Select(b => Vector3<int>.Parse(b)).ToArray());
        }

        return scanners;
    }
}
