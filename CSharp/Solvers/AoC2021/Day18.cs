using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 18
/// </summary>
public class Day18 : ArraySolver<Day18.Number>
{
    public class Number : IEnumerable<Number>, IAdditionOperators<Number, Number, Number>
    {
        public Number? Parent  { get; private set; }

        public Number? Left    { get; private set;  }

        public Number? Right   { get; private set;  }

        public int LeftValue   { get; private set;  }

        public int RightValue  { get; private set;  }

        private int Depth
        {
            get
            {
                int depth = 0;
                Number? parent = Parent;
                while (parent is not null)
                {
                    depth++;
                    parent = parent.Parent;
                }

                return depth;
            }
        }

        private Number Root
        {
            get
            {
                Number root = this;
                while (root.Parent is not null)
                {
                    root = root.Parent;
                }

                return root;
            }
        }

        public int Magnitude => ((Left?.Magnitude ?? LeftValue) * 3) + ((Right?.Magnitude ?? RightValue) * 2);

        private bool MustExplode => Left is null && Right is null && Depth >= 4;

        private bool MustSplit   => (Left is null && LeftValue > 9) || (Right is null && RightValue > 9);

        private Number() { }

        private Number(Number left, Number right)
        {
            Left  = new Number(left)  { Parent = this };
            Right = new Number(right) { Parent = this };
        }

        private Number(int left, int right)
        {
            LeftValue = left;
            RightValue = right;
        }

        private Number(Number number)
        {
            if (number.Left is not null)
            {
                Left = new Number(number.Left) { Parent = this };
            }
            if (number.Right is not null)
            {
                Right = new Number(number.Right) { Parent = this };
            }

            LeftValue = number.LeftValue;
            RightValue = number.RightValue;
            Parent = number.Parent;
        }

        private void Explode()
        {
            Number root = this.Root;
            Number? previous = root.TakeWhile(n => n != this).LastOrDefault(n => n.Left is null || n.Right is null);
            Number? next = root.SkipWhile(n => n != this).Skip(1).FirstOrDefault(n => n.Left is null || n.Right is null);

            if (previous is not null)
            {
                if (previous.Right is null)
                {
                    previous.RightValue += this.LeftValue;
                }
                else
                {
                    previous.LeftValue += this.LeftValue;
                }
            }

            if (next is not null)
            {
                if (next.Left is null)
                {
                    next.LeftValue += this.RightValue;
                }
                else
                {
                    next.RightValue += this.RightValue;
                }
            }

            if (Parent!.Left == this)
            {
                Parent.Left = null;
                Parent.LeftValue = 0;
            }
            else
            {
                Parent.Right = null;
                Parent.RightValue = 0;
            }

            this.Parent = null;
        }

        private void Split()
        {
            if (Left is null && LeftValue > 9)
            {
                int left  = (int)MathF.Floor(LeftValue / 2f);
                int right = (int)MathF.Ceiling(LeftValue / 2f);
                Left      = new Number(left, right) { Parent = this };
                LeftValue = 0;
            }
            else
            {
                int left  = (int)MathF.Floor(RightValue / 2f);
                int right = (int)MathF.Ceiling(RightValue / 2f);
                Right      = new Number(left, right) { Parent = this };
                RightValue = 0;
            }
        }

        public override string ToString()
        {
            // ReSharper disable once ArrangeMethodOrOperatorBody
            return $"[{Left?.ToString() ?? LeftValue.ToString()},{Right?.ToString() ?? RightValue.ToString()}]";
        }

        public IEnumerator<Number> GetEnumerator()
        {
            if (Left is not null)
            {
                foreach (Number nested in Left)
                {
                    yield return nested;
                }
            }

            yield return this;

            if (Right is not null)
            {
                foreach (Number nested in Right)
                {
                    yield return nested;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static Number operator +(Number left, Number right)
        {
            Number result = new(left, right);
            bool modified;
            do
            {
                modified = false;
                Number? toExplode = result.FirstOrDefault(n => n.MustExplode);
                if (toExplode != null)
                {
                    toExplode.Explode();
                    modified = true;
                    continue;
                }

                Number? toSplit = result.FirstOrDefault(n => n.MustSplit);
                if (toSplit != null)
                {
                    toSplit.Split();
                    modified = true;
                }
            }
            while (modified);

            return result;
        }

        public static Number ParseNumber(string line)
        {
            int i = 1;
            return ParseNumber(line, ref i);
        }

        private static Number ParseNumber(string line, ref int i)
        {
            Number number = new();
            if (line[i] is '[')
            {
                // Skip opening parenthesis
                i++;
                number.Left = ParseNumber(line, ref i);
                number.Left.Parent = number;
                // Skip comma
                i++;
            }
            else
            {
                // Put in value
                number.LeftValue = line[i] - '0';
                // Skip comma
                i += 2;
            }

            if (line[i] is '[')
            {
                // Skip opening parenthesis
                i++;
                number.Right = ParseNumber(line, ref i);
                number.Right.Parent = number;
                // Skip closing parenthesis
                i++;
            }
            else
            {
                // Put in value
                number.RightValue = line[i] - '0';
                // Skip closing parenthesis
                i += 2;
            }

            return number;
        }
    }

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day18"/> Solver for 2021 - 18 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Number"/>[] fails</exception>
    public Day18(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Number sum = this.Data.Sum();
        AoCUtils.LogPart1(sum.Magnitude);

        int maxMagnitude = this.Data.
                                SelectMany(n => this.Data
                                                    .Where(m => m != n)
                                                    .Select(m => n + m))
                               .Max(n => n.Magnitude);
        AoCUtils.LogPart2(maxMagnitude);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Number ConvertLine(string line) => Number.ParseNumber(line);
    #endregion
}
