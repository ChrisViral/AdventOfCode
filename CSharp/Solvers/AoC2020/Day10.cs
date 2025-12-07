using System;
using System.Linq;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 10
/// </summary>
public sealed class Day10 : Solver<Day10.Adapter[]>
{
    /// <summary>
    /// Adapter object
    /// </summary>
    public sealed class Adapter : IComparable<Adapter>
    {
        /// <summary>
        /// Tolerance between different adapters
        /// </summary>
        private const int TOLERANCE = 3;

        /// <summary>
        /// Jolts rating of this adapter
        /// </summary>
        public int Jolts { get; set; }

        /// <summary>
        /// Paths to final adapter from this one
        /// </summary>
        public long Paths { get; set; } = 1L;

        /// <summary>
        /// List of compatible adapters with this one
        /// </summary>
        public Adapter[]? Compatible { get; private set; }

        /// <summary>
        /// Creates a new adapter with the specified jolts
        /// </summary>
        /// <param name="jolts"></param>
        public Adapter(int jolts) => this.Jolts = jolts;

        /// <summary>
        /// Sets all the compatible adapters to this one from
        /// </summary>
        /// <param name="adapters"></param>
        /// <param name="index"></param>
        public void SetCompatible(Adapter[] adapters, int index)
        {
            this.Compatible = adapters[index..].TakeWhile(a => a.Jolts - this.Jolts <= TOLERANCE).ToArray();
            this.Paths = this.Compatible.Sum(a => a.Paths);
        }

        /// <inheritdoc cref="IComparable{T}.CompareTo"/>
        public int CompareTo(Adapter? other) => this.Jolts.CompareTo(other?.Jolts);

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => this.Jolts.ToString();
    }

    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Adapter"/>[] fails</exception>
    public Day10(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        this.Data.Sort();

        //To make this simpler lets just store the results in an array, easier to code than vars, less waste than a dict
        int[] counts = new int[4];
        //Final and final jumps
        counts[3]++;
        counts[this.Data[1].Jolts]++;

        for (int i = 1; i < this.Data.Length - 2; /*i++*/)
        {
            counts[-this.Data[i++].Jolts + this.Data[i].Jolts]++;
        }

        AoCUtils.LogPart1(counts[1] * counts[3]);
        AoCUtils.LogPart2(this.Data[0].Paths);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Adapter[] Convert(string[] rawInput)
    {
        Adapter[] adapters = new Adapter[rawInput.Length + 2];
        adapters[0] = new Adapter(0);
        adapters[^1] = new Adapter(int.MaxValue);
        foreach (int i in ..rawInput.Length)
        {
            adapters[i + 1] = new Adapter(int.Parse(rawInput[i]));
        }

        adapters.Sort();
        adapters[^1].Jolts = adapters[^2].Jolts + 3;

        for (int i = adapters.Length - 2; i >= 0; i--)
        {
            adapters[i].SetCompatible(adapters, i + 1);
        }

        return adapters;
    }
}
