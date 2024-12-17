using System;
using System.Collections.Generic;
using System.ComponentModel;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using Microsoft.Z3;

namespace AdventOfCode.Solvers.AoC2024
{
    /// <summary>
    /// Solver for 2024 Day 17
    /// </summary>
    public class Day17 : Solver<(long a, long b, long c, int[] program)>
    {
        private enum Opcode
        {
            ADV = 0,    // A <- A / 2^Op
            BXL = 1,    // B <- B Xor Lit
            BST = 2,    // B <- Op Mod 8
            JNZ = 3,    // A not 0 => Jump Lit
            BXC = 4,    // B <- B Xor C
            OUT = 5,    // B Mod 8 -> Out
            BDV = 6,    // B <- A / 2^Op
            CDV = 7     // C <- A / 2^Op
        }

        private long a;
        private long b;
        private long c;
        private int ip;
        private int[] code = null!;
        private List<int> output = null!;

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day17"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
        public Day17(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Base.Solver.Run"/>
        public override void Run()
        {
            (this.a, this.b, this.c, this.code) = this.Data;
            this.output = new List<int>(this.code.Length);
            AoCUtils.LogPart1(RunProgram());

            // Looks like it's Z3 time again
            Context context = new();
            Optimize optimize = context.MkOptimize();

            Symbol initASymbol = context.MkSymbol("initA");
            BitVecExpr initA   = context.MkBVConst(initASymbol, 64);
            BitVecExpr ra = context.MkBVConst(initASymbol, 64);
            BitVecExpr rb = MakeIntBV(context, 0);
            BitVecExpr rc = MakeIntBV(context, 0);

            foreach (int bytecode in this.code)
            {
                BitVecExpr x = MakeIntBV(context, bytecode);
                for (this.ip = 0; ip < this.code.Length; /* i+ = 2 */)
                {
                    Opcode opcode = (Opcode)this.code[this.ip++];
                    int operand = this.code[this.ip++];
                    switch (opcode)
                    {
                        case Opcode.ADV:
                        {
                            // a = a / 1 << op
                            BitVecExpr one   = MakeIntBV(context, 1);
                            BitVecExpr shift = context.MkBVSHL(one,  GetComboBVOperand(operand, ra, rb, rc, context));
                            ra = context.MkBVSDiv(ra, shift);
                            break;
                        }

                        case Opcode.BXL:
                        {
                            // b = b ^ lit
                            BitVecExpr lit = MakeIntBV(context, operand);
                            rb = context.MkBVXOR(rb, lit);
                            break;
                        }

                        case Opcode.BST:
                        {
                            // b = op % 8
                            BitVecExpr eight = MakeIntBV(context, 8);
                            BitVecExpr value = GetComboBVOperand(operand, ra, rb, rc, context);
                            rb = context.MkBVSMod(value, eight);
                            break;
                        }

                        case Opcode.JNZ:
                            // Ignored for the solver
                            break;

                        case Opcode.BXC:
                            // b = b ^c
                            rb = context.MkBVXOR(rb, rc);
                            break;

                        case Opcode.OUT:
                        {
                            // out(b % 8)
                            BitVecExpr eight    = MakeIntBV(context, 8);
                            BitVecExpr outValue = context.MkBVSMod(rb, eight);
                            optimize.Assert(context.MkEq(outValue, x));
                            break;
                        }

                        case Opcode.BDV:
                        {
                            // b = a / 1 << op
                            BitVecExpr one   = MakeIntBV(context, 1);
                            BitVecExpr shift = GetComboBVOperand(operand, ra, rb, rc, context);
                            rb = context.MkBVSDiv(ra, context.MkBVSHL(one, shift));
                            break;
                        }

                        case Opcode.CDV:
                        {
                            // c = a / 1 << op
                            BitVecExpr one   = MakeIntBV(context, 1);
                            BitVecExpr shift = GetComboBVOperand(operand, ra, rb, rc, context);
                            rc = context.MkBVSDiv(ra, context.MkBVSHL(one, shift));
                            break;
                        }

                        default:
                            throw new InvalidEnumArgumentException(nameof(opcode), (int)opcode, typeof(Opcode));
                    }
                }

            }
            // Ensure that a is 0
            optimize.Assert(context.MkEq(ra, MakeIntBV(context, 0)));

            // Minimize initial a
            optimize.MkMinimize(initA);
            optimize.Check();
            Expr result = optimize.Model.Evaluate(initA, true);
            AoCUtils.LogPart2(result);
        }

        private string RunProgram()
        {
            int eod = this.code.Length;
            while (this.ip < eod)
            {
                Opcode opcode = (Opcode)this.code[this.ip++];
                switch (opcode)
                {
                    case Opcode.ADV:
                        Div(out this.a);
                        break;

                    case Opcode.BXL:
                        Xor(this.code[this.ip++]);
                        break;

                    case Opcode.BST:
                        this.b = GetComboOperand() % 8;
                        break;

                    case Opcode.JNZ when this.a is not 0:
                        this.ip = this.code[this.ip];
                        break;

                    case Opcode.JNZ:
                        this.ip++;
                        break;

                    case Opcode.BXC:
                        this.ip++;
                        Xor(this.c);
                        break;

                    case Opcode.OUT:
                        this.output.Add((int)(GetComboOperand() % 8));
                        break;

                    case Opcode.BDV:
                        Div(out this.b);
                        break;

                    case Opcode.CDV:
                        Div(out this.c);
                        break;

                    default:
                        throw new InvalidEnumArgumentException(nameof(opcode), (int)opcode, typeof(Opcode));
                }
            }

            return string.Join(',', this.output);
        }

        private void Div(out long register) => register = this.a / (1L << checked((int)GetComboOperand()));

        private void Xor(long value) => this.b ^= value;

        private long GetComboOperand()
        {
            long operand = this.code[this.ip++];
            return operand switch
            {
                0 or 1 or 2 or 3 => operand,
                4                => this.a,
                5                => this.b,
                6                => this.c,
                7                => throw new InvalidOperationException("Combo operator 7 is not supported"),
                _                => throw new InvalidOperationException("Invalid combo operator")
            };
        }

        private static BitVecExpr MakeIntBV(Context context, int value) => context.MkInt2BV(64, context.MkInt(value));

        private static BitVecExpr GetComboBVOperand(int operand, BitVecExpr ra, BitVecExpr rb, BitVecExpr rc, Context context)
        {
            return operand switch
            {
                0 or 1 or 2 or 3 => MakeIntBV(context, operand),
                4                => ra,
                5                => rb,
                6                => rc,
                7                => throw new InvalidOperationException("Combo operator 7 is not supported"),
                _                => throw new InvalidOperationException("Invalid combo operator")
            };
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override (long, long, long, int[]) Convert(string[] rawInput)
        {
            // Registers
            long registerA = long.Parse(rawInput[0].AsSpan(12));
            long registerB = long.Parse(rawInput[1].AsSpan(12));
            long registerC = long.Parse(rawInput[2].AsSpan(12));

            // Bytecode
            ReadOnlySpan<char> programSpan = rawInput[3].AsSpan(9);
            int[] program = new int[(programSpan.Length + 1) / 2];
            foreach (int i in ..program.Length)
            {
                program[i] = programSpan[i * 2] - '0';
            }
            return (registerA, registerB, registerC, program);
        }
        #endregion
    }
}

