﻿using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AdventOfCode.Solvers.AoC2020
{
    /// <summary>
    /// Solver for 2020 Day 18
    /// </summary>
    public class Day18 : Solver<string[][]>
    {
        /// <summary>
        /// Numerical operation types
        /// </summary>
        public enum Operation
        {
            ADD,
            MULTIPLY
        }

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day18"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/>[][] fails</exception>
        public Day18(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            AoCUtils.LogPart1(this.Data.Sum(CalculateExpression));
            AoCUtils.LogPart2(this.Data.Sum(CalculateAdvancedExpression));
        }

        /// <summary>
        /// Calculates an expression
        /// </summary>
        /// <param name="expression">Tokenized expression to evaluate</param>
        /// <returns>The value of the expression</returns>
        private static long CalculateExpression(IEnumerable<string> expression)
        {
            //Setup
            Stack<(long, Operation)> operationStack = new();
            long total = 0L;
            Operation operation = Operation.ADD;
            //Loop through every expression
            foreach (string s in expression)
            {
                switch (s)
                {
                    case "+":
                        operation = Operation.ADD;
                        break;
                    case "*":
                        operation = Operation.MULTIPLY;
                        break;

                    case { } when s[0] is '(':
                        //Push current stack
                        operationStack.Push((total, operation));
                        //Remove parenthesis
                        string n = s.TrimStart('(');
                        int parenthesis = s.Length - n.Length;
                        //Add blank operations for every extra parenthesis
                        while (parenthesis-- > 1)
                        {
                            operationStack.Push((0L, Operation.ADD));
                        }
                        //Parse number
                        total = long.Parse(n);
                        break;

                    case { } when s[^1] is ')':
                        //Get current result
                        n = s.TrimEnd(')');
                        long result = Evaluate(total, long.Parse(n), operation);
                        //Remove parenthesis
                        parenthesis = s.Length - n.Length;
                        do
                        {
                            //Calculate results from closed parenthesis
                            (total, operation) = operationStack.Pop();
                            result = Evaluate(total, result, operation);
                        }
                        while (parenthesis-- > 1);
                        //Set result
                        total = result;
                        break;

                    case { }:
                        //Evaluate new result
                        total = Evaluate(total, long.Parse(s), operation);
                        break;
                }
            }

            return total;
        }

        /// <summary>
        /// Evaluates a two operand expression
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="operation">Operation type</param>
        /// <returns>The value of the operation</returns>
        private static long Evaluate(long a, long b, Operation operation)
        {
            return operation switch
            {
                Operation.ADD      => a + b,
                Operation.MULTIPLY => a * b,
                _                  => throw new InvalidEnumArgumentException(nameof(operation), (int)operation, typeof(Operation))
            };
        }

        /// <summary>
        /// Calculates an "advanced" expression
        /// </summary>
        /// <param name="expression">Tokenized expression to evaluate</param>
        /// <returns>The value of the expression</returns>
        private static long CalculateAdvancedExpression(IEnumerable<string> expression)
        {
            //Multiplied values
            Stack<List<long>> multipliers = new();
            multipliers.Push(new List<long>());
            //Setup
            Stack<(long, Operation)> operationStack = new();
            long total = 0L;
            Operation operation = Operation.ADD;
            foreach (string s in expression)
            {
                switch (s)
                {
                    case "+":
                        operation = Operation.ADD;
                        break;
                    case "*":
                        operation = Operation.MULTIPLY;
                        break;

                    case { } when s[0] is '(':
                        //Push current stack
                        operationStack.Push((total, operation));
                        multipliers.Push(new List<long>());
                        //Remove parenthesis
                        string n = s.TrimStart('(');
                        int parenthesis = s.Length - n.Length;
                        //Add blank operations for every extra parenthesis
                        while (parenthesis-- > 1)
                        {
                            operationStack.Push((0L, Operation.ADD));
                            multipliers.Push(new List<long>());
                        }
                        //Parse number
                        total = long.Parse(n);
                        break;

                    case { } when s[^1] is ')':
                        //Get current result
                        n = s.TrimEnd(')');
                        long result = EvaluateAdvanced(total, long.Parse(n), operation, multipliers.Peek());
                        //Remove parenthesis
                        parenthesis = s.Length - n.Length;
                        do
                        {
                            //Calculate results from closed parenthesis
                            List<long> mult = multipliers.Pop();
                            result = mult.Aggregate(1L, (a, b) => a * b) * result;
                            (total, operation) = operationStack.Pop();
                            result = EvaluateAdvanced(total, result, operation, multipliers.Peek());
                        }
                        while (parenthesis-- > 1);
                        //Set result
                        total = result;
                        break;

                    case { }:
                        //Evaluate new result
                        total = EvaluateAdvanced(total, long.Parse(s), operation, multipliers.Peek());
                        break;
                }
            }

            long value = multipliers.Peek().Aggregate(1L, (a, b) => a * b) * total;
            return value;
        }

        /// <summary>
        /// Evaluates an "advanced" two operand expression
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="operation">Operation type</param>
        /// <param name="multipliers">Multiplication backlog</param>
        /// <returns>The value of the expression</returns>
        private static long EvaluateAdvanced(long a, long b, Operation operation, List<long> multipliers)
        {
            switch (operation)
            {
                case Operation.ADD:
                    return a + b;
                case Operation.MULTIPLY:
                    multipliers.Add(a);
                    return b;
                
                default:
                    throw new InvalidEnumArgumentException(nameof(operation), (int)operation, typeof(Operation));
            }
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override string[][] Convert(string[] rawInput) => Array.ConvertAll(rawInput, s => s.Split(' ', StringSplitOptions.TrimEntries));
        #endregion
    }
}
