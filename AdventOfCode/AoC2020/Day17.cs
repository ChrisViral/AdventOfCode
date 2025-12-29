using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 17
/// </summary>
public sealed class Day17 : Solver<(Day17.Cube<Vector3<int>> part1, Day17.Cube<Vector4<int>> part2)>
{
    /// <summary>
    /// Conway cube implementation
    /// </summary>
    /// <typeparam name="T">Type of element within the cube</typeparam>
    public sealed class Cube<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Delegate to create new objects from
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <returns>The created object</returns>
        public delegate T Factory(int x, int y);

        /// <summary>
        /// Delegate to explore the surroundings of a value
        /// </summary>
        /// <param name="value">Value to explore</param>
        /// <returns>It's surrounding values</returns>
        public delegate IEnumerable<T> Explorer(T value);

        private readonly HashSet<T> activeCubes;
        private readonly Dictionary<T, int> surrounding = new();
        private readonly Explorer explorer;

        /// <summary>
        /// Creates a new cube with the given input, object factory, and explorer
        /// </summary>
        /// <param name="input">Input to create the cube from</param>
        /// <param name="factory">Object factory function</param>
        /// <param name="explorer">Object explorer function</param>
        public Cube(IReadOnlyList<string> input, Factory factory, Explorer explorer)
        {
            this.activeCubes = [];
            this.explorer    = explorer;
            int n = input.Count;
            int l = n / 2;
            foreach (int y in ..n)
            {
                string s = input[y];
                foreach (int x in ..n)
                {
                    if (s[x] is '#')
                    {
                        this.activeCubes.Add(factory(x - l, y - l));
                    }
                }
            }
        }

        /// <summary>
        /// Simulates the cube for a certain amount of turns
        /// </summary>
        /// <param name="n">Turns amount</param>
        /// <returns>The number of active cubes at the end of the simulation</returns>
        /// ReSharper disable once CognitiveComplexity
        public int Simulate(int n)
        {
            foreach (int _ in ..n)
            {
                foreach (T active in this.activeCubes)
                {
                    foreach (T adjacent in this.explorer(active))
                    {
                        this.surrounding.TryGetValue(adjacent, out int hits);
                        this.surrounding[adjacent] = hits + 1;
                    }
                }

                //Check all positions with hits
                foreach ((T pos, int hits) in this.surrounding)
                {
                    if (!this.activeCubes.Contains(pos) && hits is 3)
                    {
                        this.activeCubes.Add(pos);
                    }
                }

                //Check all active cubes
                foreach (T active in this.activeCubes.ToArray())
                {
                    this.surrounding.TryGetValue(active, out int hits);
                    if (hits is not 2 and not 3)
                    {
                        this.activeCubes.Remove(active);
                    }
                }

                this.surrounding.Clear();
            }

            return this.activeCubes.Count;
        }
    }

    /// <summary>
    /// Creates a new <see cref="Day17"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="bool"/>[,,] fails</exception>
    public Day17(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        AoCUtils.LogPart1(this.Data.part1.Simulate(6));
        AoCUtils.LogPart2(this.Data.part2.Simulate(6));
    }

    /// <inheritdoc />
    protected override (Cube<Vector3<int>>, Cube<Vector4<int>>) Convert(string[] rawInput)
    {
        Cube<Vector3<int>> cube3 = new(rawInput, (x, y) => new Vector3<int>(x, y, 0), v => v.AsAdjacentEnumerable(withDiagonals: true));
        Cube<Vector4<int>> cube4 = new(rawInput, (x, y) => new Vector4<int>(x, y, 0, 0), v => v.Adjacent(withDiagonals: true));
        return (cube3, cube4);
    }
}
