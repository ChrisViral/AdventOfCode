using System.Collections;
using System.Numerics;
using AdventOfCode.Utils.Extensions.Enumerables;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2021;

/// <summary>
/// Solver for 2021 Day 18
/// </summary>
public sealed class Day18 : ArraySolver<Day18.Number>
{
    public sealed class Number : IEnumerable<Number>, IAdditionOperators<Number, Number, Number>
    {
        private Number? Parent  { get; set; }

        private Number? Left    { get; set;  }

        private Number? Right   { get; set;  }

        private int LeftValue   { get; set;  }

        private int RightValue  { get; set;  }

        private int Depth
        {
            get
            {
                int depth = 0;
                Number? parent = this.Parent;
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

        public int Magnitude => ((this.Left?.Magnitude ?? this.LeftValue) * 3) + ((this.Right?.Magnitude ?? this.RightValue) * 2);

        private bool MustExplode => this.Left is null && this.Right is null && this.Depth >= 4;

        private bool MustSplit   => (this.Left is null && this.LeftValue > 9) || (this.Right is null && this.RightValue > 9);

        private Number() { }

        private Number(Number left, Number right)
        {
            this.Left  = new Number(left)  { Parent = this };
            this.Right = new Number(right) { Parent = this };
        }

        private Number(int left, int right)
        {
            this.LeftValue = left;
            this.RightValue = right;
        }

        private Number(Number number)
        {
            if (number.Left is not null)
            {
                this.Left = new Number(number.Left) { Parent = this };
            }
            if (number.Right is not null)
            {
                this.Right = new Number(number.Right) { Parent = this };
            }

            this.LeftValue = number.LeftValue;
            this.RightValue = number.RightValue;
            this.Parent = number.Parent;
        }

        // ReSharper disable once CognitiveComplexity
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

            if (this.Parent!.Left == this)
            {
                this.Parent.Left = null;
                this.Parent.LeftValue = 0;
            }
            else
            {
                this.Parent.Right = null;
                this.Parent.RightValue = 0;
            }

            this.Parent = null;
        }

        private void Split()
        {
            if (this.Left is null && this.LeftValue > 9)
            {
                int left  = (int)MathF.Floor(this.LeftValue / 2f);
                int right = (int)MathF.Ceiling(this.LeftValue / 2f);
                this.Left      = new Number(left, right) { Parent = this };
                this.LeftValue = 0;
            }
            else
            {
                int left  = (int)MathF.Floor(this.RightValue / 2f);
                int right = (int)MathF.Ceiling(this.RightValue / 2f);
                this.Right      = new Number(left, right) { Parent = this };
                this.RightValue = 0;
            }
        }

        public override string ToString()
        {
            // ReSharper disable once ArrangeMethodOrOperatorBody
            return $"[{this.Left?.ToString() ?? this.LeftValue.ToString()},{this.Right?.ToString() ?? this.RightValue.ToString()}]";
        }

        public IEnumerator<Number> GetEnumerator()
        {
            if (this.Left is not null)
            {
                foreach (Number nested in this.Left)
                {
                    yield return nested;
                }
            }

            yield return this;

            if (this.Right is not null)
            {
                foreach (Number nested in this.Right)
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

    /// <summary>
    /// Creates a new <see cref="Day18"/> Solver for 2021 - 18 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Number"/>[] fails</exception>
    public Day18(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Number sum = this.Data.Sum();
        AoCUtils.LogPart1(sum.Magnitude);

        int maxMagnitude = this.Data.SelectMany(n => this.Data
                                                         .Where(m => m != n)
                                                         .Select(m => n + m))
                               .Max(n => n.Magnitude);

        AoCUtils.LogPart2(maxMagnitude);
    }

    /// <inheritdoc />
    protected override Number ConvertLine(string line) => Number.ParseNumber(line);
}
