using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Enumerables;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 14
/// </summary>
public sealed class Day14 : Solver<Day14.Instruction[]>
{
    /// <summary>
    /// Ferry bitmask
    /// </summary>
    public readonly struct Bitmask
    {
        /// <summary>
        /// Size in bits of the bitmask
        /// </summary>
        private const int SIZE = 36;

        private readonly long positiveMask;
        private readonly long negativeMask;

        /// <summary>
        /// Creates a new bitmask as specified
        /// </summary>
        /// <param name="mask">Value to parse as a Bitmask</param>
        public Bitmask(string mask)
        {
            this.positiveMask = 0L;
            this.negativeMask = 0L;

            //Go through the string and parse the bits
            long i = 1L << (SIZE - 1);
            foreach (char c in mask)
            {
                switch (c)
                {
                    case '1':
                        this.positiveMask |= i;
                        break;
                    case '0':
                        this.negativeMask |= i;
                        break;
                }

                i >>= 1;
            }
        }

        /// <summary>
        /// Get all the masked addresses by this mask
        /// </summary>
        /// <param name="address">Address to get all the masked versions for</param>
        /// <param name="bit">Current working bit, should always be called with default value</param>
        /// <returns>A sequence of all masked addresses, this will be of size 2^n for n unstable bits</returns>
        public IEnumerable<long> GetMaskedAddresses(long address, int bit = SIZE)
        {
            //If at a negative bit number, done, return
            if (bit < 0) yield return address;

            //Setup current mask
            long mask = 1L << bit - 1;
            //While active bit position valid
            while (bit-- >= 0)
            {
                //If masked by the positive mask
                if ((this.positiveMask & mask) is not 0)
                {
                    //Force the bit into a 1
                    address |= mask;
                }
                //If not masked by the negative mask
                else if ((this.negativeMask & mask) is 0)
                {
                    //Get masks for current bit
                    foreach (long a in GetMaskedAddresses(address, bit))
                    {
                        yield return a;
                    }
                    //Then get masks for flipped bits
                    foreach (long a in GetMaskedAddresses(address ^ mask, bit))
                    {
                        yield return a;
                    }

                    //Immediately hop out
                    yield break;
                }

                //Move the mask to the right
                mask >>= 1;
            }

            //Return the finalized address
            yield return address;
        }

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString()
        {
            StringBuilder sb = new(SIZE);
            for (long i = 1L << (SIZE - 1); sb.Length is not SIZE; i >>= 1)
            {
                if ((this.positiveMask & i) is not 0)
                {
                    sb.Append('1');
                }
                else if ((this.negativeMask & i) is not 0)
                {
                    sb.Append('0');
                }
                else
                {
                    sb.Append('X');
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Bitwise AND on the given integer and bitmask. The operations sets the bits in the positive mask and removes the bits in the negative mask
        /// </summary>
        /// <param name="a">Integer to and</param>
        /// <param name="b">Bitmask to apply</param>
        /// <returns>The integer with the bitmask applied</returns>
        public static long operator &(long a, in Bitmask b) => (a | b.positiveMask) & ~b.negativeMask;
    }

    /// <summary>
    /// Ferry program instruction
    /// </summary>
    public sealed class Instruction
    {
        /// <summary>
        /// Instruction opcode
        /// </summary>
        public enum Opcode
        {
            MASK,
            MEM
        }

        /// <summary>
        /// Regex parse pattern
        /// </summary>
        public const string PATTERN = @"(?:mask = ([01X]{36})|mem\[(\d+)\] = (\d+))";

        private readonly Opcode operation;
        private readonly long address;
        private readonly long value;
        private readonly Bitmask mask;

        /// <summary>
        /// Creates a new Mask operation
        /// </summary>
        /// <param name="mask">Bitmask to set</param>
        public Instruction(string mask)
        {
            this.operation = Opcode.MASK;
            this.mask = new Bitmask(mask);
        }

        /// <summary>
        /// Creates a new memory operation
        /// </summary>
        /// <param name="address">Address to set</param>
        /// <param name="value">Value to set</param>
        public Instruction(long address, long value)
        {
            this.operation = Opcode.MEM;
            this.address = address;
            this.value = value;
        }

        /// <summary>
        /// Executes the current instruction on the program memory
        /// </summary>
        /// <param name="memory">Current program memory</param>
        /// <param name="bitmask">Current program bitmask</param>
        public void Execute(Dictionary<long, long> memory, ref Bitmask bitmask)
        {
            switch (this.operation)
            {
                case Opcode.MASK:
                    bitmask = this.mask;
                    break;

                case Opcode.MEM:
                    memory[this.address] = this.value & bitmask;
                    break;
            }
        }

        /// <summary>
        /// Executes the current instruction on the program memory for a V2 decoder
        /// </summary>
        /// <param name="memory">Current program memory</param>
        /// <param name="bitmask">Current program bitmask</param>
        public void ExecuteV2(Dictionary<long, long> memory, ref Bitmask bitmask)
        {
            switch (this.operation)
            {
                case Opcode.MASK:
                    bitmask = this.mask;
                    break;

                case Opcode.MEM:
                    bitmask.GetMaskedAddresses(this.address).ForEach(a => memory[a] = this.value);
                    break;
            }
        }
    }

    /// <summary>
    /// Creates a new <see cref="Day14"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Instruction"/>[] fails</exception>
    public Day14(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        //Part one decoding
        Dictionary<long, long> memory = new();
        Bitmask bitmask = default;
        this.Data.ForEach(i => i.Execute(memory, ref bitmask));
        AoCUtils.LogPart1(memory.Values.Sum());

        //Part two decoding
        memory.Clear();
        Bitmask bitmaskV2 = default;
        this.Data.ForEach(i => i.ExecuteV2(memory, ref bitmaskV2));
        AoCUtils.LogPart2(memory.Values.Sum());
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Instruction[] Convert(string[] rawInput) => RegexFactory<Instruction>.ConstructObjects(Instruction.PATTERN, rawInput, RegexOptions.Compiled);
}
