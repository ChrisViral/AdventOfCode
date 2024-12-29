using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace AdventOfCode.Intcode;

/// <summary>
/// Intcode Instructions
/// </summary>
[PublicAPI]
public static class Instructions
{
    /// <summary>
    /// Intcode Opcodes
    /// </summary>
    private enum Opcodes
    {
        ADD = 1, //Add
        MUL = 2, //Multiply
        INP = 3, //Input
        OUT = 4, //Output
        JNZ = 5, //Jump Not Zero
        JEZ = 6, //Jump Equal Zero
        TLT = 7, //Test Less Than
        TEQ = 8, //Test Equals
        REL = 9, //Relative Base Set

        NOP = 0, //No Op
        HLT = 99 //Halt
    }

    /// <summary>
    /// Parameter modes
    /// </summary>
    public enum ParamModes
    {
        POSITION  = 0,
        IMMEDIATE = 1,
        RELATIVE  = 2
    }

    /// <summary>
    /// Intcode operation delegate
    /// </summary>
    /// <param name="pointer">Pointer of the VM</param>
    /// <param name="relative">Relative base of the VM</param>
    /// <param name="data">Intcode VM data</param>
    /// <param name="modes">Operand modes</param>
    public delegate IntcodeVM.VMState Instruction(ref int pointer, ref int relative, in IntcodeVM.VMData data, Modes modes);

    /// <summary>
    /// Operand Modes
    /// </summary>
    public readonly struct Modes
    {
        #region Fields
        /// <summary> First parameter mode </summary>
        public readonly ParamModes first;
        /// <summary> Second parameter mode </summary>
        public readonly ParamModes second;
        /// <summary> Third parameter mode </summary>
        public readonly ParamModes third;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new set of Modes from the given number
        /// </summary>
        /// <param name="modes">Value to extract the modes from</param>
        /// <exception cref="InvalidEnumArgumentException">If one of the parsed modes is not a valid member of the enum</exception>
        public Modes(int modes)
        {
            // ReSharper disable LocalVariableHidesMember
            (modes,     int first)  = Math.DivRem(modes, 10);
            (int third, int second) = Math.DivRem(modes, 10);
            this.first  = (ParamModes)first;
            this.second = (ParamModes)second;
            this.third  = (ParamModes)third;
            // ReSharper restore LocalVariableHidesMember
        }
        #endregion
    }

    #region Constants
    /// <summary>
    /// True Constant
    /// </summary>
    private const long TRUE = 1L;
    /// <summary>
    /// False Constant
    /// </summary>
    private const long FALSE = 0L;
    #endregion

    #region Static methods
    /// <summary>
    /// Decodes an opcode into it's associated instruction
    /// </summary>
    /// <param name="opcode">Opcode to decode</param>
    /// <returns>A Tuple containing the Instruction this Opcode refers to and it's parameter modes</returns>
    /// <exception cref="ArgumentException">If the Modes input string is of invalid length</exception>
    /// <exception cref="InvalidEnumArgumentException">If an invalid Opcodes or ParamModes is detected</exception>
    public static (Instruction instruction, Modes modes) Decode(long opcode)
    {
        (int mode, opcode) = Math.DivRem((int)opcode, 100);
        Modes modes = new(mode);
        Opcodes op = (Opcodes)opcode;
        Instruction instruction = op switch
        {
            //Instructions
            Opcodes.ADD => Add,
            Opcodes.MUL => Mul,
            Opcodes.INP => Inp,
            Opcodes.OUT => Out,
            Opcodes.JNZ => Jnz,
            Opcodes.JEZ => Jez,
            Opcodes.TLT => Tlt,
            Opcodes.TEQ => Teq,
            Opcodes.REL => Rel,

            //Nop, Halt, and unknown
            Opcodes.NOP => Nop,
            Opcodes.HLT => Hlt,
            _           => throw new InvalidEnumArgumentException(nameof(opcode), (int)op, typeof(Opcodes))
        };

        return (instruction, modes);
    }

    /// <summary>
    /// ADD Instruction, adds the values of the first and second operands into the address of the third operand
    /// </summary>
    /// <param name="pointer">Current VM pointer</param>
    /// <param name="relative">Current VM relative base</param>
    /// <param name="data">VM specific data</param>
    /// <param name="modes">Parameter modes</param>
    private static IntcodeVM.VMState Add(ref int pointer, ref int relative, in IntcodeVM.VMData data, Modes modes)
    {
        ref long a = ref GetOperand(pointer++, relative, data.memory, modes.first);
        ref long b = ref GetOperand(pointer++, relative, data.memory, modes.second);
        ref long c = ref GetOperand(pointer++, relative, data.memory, modes.third);

        c = a + b;
        return IntcodeVM.VMState.RUNNING;
    }

    /// <summary>
    /// MUL Instruction, multiplies the values of the first and second operands into the address of the third operand
    /// </summary>
    /// <param name="pointer">Current VM pointer</param>
    /// <param name="relative">Current VM relative base</param>
    /// <param name="data">VM specific data</param>
    /// <param name="modes">Parameter modes</param>
    private static IntcodeVM.VMState Mul(ref int pointer, ref int relative, in IntcodeVM.VMData data, Modes modes)
    {
        ref long a = ref GetOperand(pointer++, relative, data.memory, modes.first);
        ref long b = ref GetOperand(pointer++, relative, data.memory, modes.second);
        ref long c = ref GetOperand(pointer++, relative, data.memory, modes.third);

        c = a * b;
        return IntcodeVM.VMState.RUNNING;
    }

    /// <summary>
    /// INP Instruction, gets a value from the input stream and puts it at the address of the first operand
    /// </summary>
    /// <param name="pointer">Current VM pointer</param>
    /// <param name="relative">Current VM relative base</param>
    /// <param name="data">VM specific data</param>
    /// <param name="modes">Parameter modes</param>
    private static IntcodeVM.VMState Inp(ref int pointer, ref int relative, in IntcodeVM.VMData data, Modes modes)
    {
        //Make sure we can get the input first
        if (!data.input(out long input)) return IntcodeVM.VMState.STALLED;

        ref long a = ref GetOperand(pointer++, relative, data.memory, modes.first);

        a = input;
        return IntcodeVM.VMState.RUNNING;
    }

    /// <summary>
    /// OUT Instruction, puts the value of the first operand in the output stream
    /// </summary>
    /// <param name="pointer">Current VM pointer</param>
    /// <param name="relative">Current VM relative base</param>
    /// <param name="data">VM specific data</param>
    /// <param name="modes">Parameter modes</param>
    private static IntcodeVM.VMState Out(ref int pointer, ref int relative, in IntcodeVM.VMData data, Modes modes)
    {
        ref long a = ref GetOperand(pointer++, relative, data.memory, modes.first);

        data.output(a);
        return IntcodeVM.VMState.RUNNING;
    }

    /// <summary>
    /// JNZ Instruction, if the first operand is not zero, sets the pointer to the value of the second operand
    /// </summary>
    /// <param name="pointer">Current VM pointer</param>
    /// <param name="relative">Current VM relative base</param>
    /// <param name="data">VM specific data</param>
    /// <param name="modes">Parameter modes</param>
    private static IntcodeVM.VMState Jnz(ref int pointer, ref int relative, in IntcodeVM.VMData data, Modes modes)
    {
        ref long a = ref GetOperand(pointer++, relative, data.memory, modes.first);
        ref long b = ref GetOperand(pointer++, relative, data.memory, modes.second);

        pointer = a is not 0L ? (int)b : pointer;
        return IntcodeVM.VMState.RUNNING;
    }

    /// <summary>
    /// JEZ Instruction, if the first operand is zero, sets the pointer to the value of the second operand
    /// </summary>
    /// <param name="pointer">Current VM pointer</param>
    /// <param name="relative">Current VM relative base</param>
    /// <param name="data">VM specific data</param>
    /// <param name="modes">Parameter modes</param>
    private static IntcodeVM.VMState Jez(ref int pointer, ref int relative, in IntcodeVM.VMData data, Modes modes)
    {
        ref long a = ref GetOperand(pointer++, relative, data.memory, modes.first);
        ref long b = ref GetOperand(pointer++, relative, data.memory, modes.second);

        pointer = a is 0L ? (int)b : pointer;
        return IntcodeVM.VMState.RUNNING;
    }

    /// <summary>
    /// TLT Instruction, if the first operand is less than the second operand, sets the third operand to 1, otherwise, sets the third operand to 0
    /// </summary>
    /// <param name="pointer">Current VM pointer</param>
    /// <param name="relative">Current VM relative base</param>
    /// <param name="data">VM specific data</param>
    /// <param name="modes">Parameter modes</param>
    private static IntcodeVM.VMState Tlt(ref int pointer, ref int relative, in IntcodeVM.VMData data, Modes modes)
    {
        ref long a = ref GetOperand(pointer++, relative, data.memory, modes.first);
        ref long b = ref GetOperand(pointer++, relative, data.memory, modes.second);
        ref long c = ref GetOperand(pointer++, relative, data.memory, modes.third);

        c = a < b ? TRUE : FALSE;
        return IntcodeVM.VMState.RUNNING;
    }

    /// <summary>
    /// TEQ Instruction, if the first operand is equal to the second operand, sets the third operand to 1, otherwise, sets the third operand to 0
    /// </summary>
    /// <param name="pointer">Current VM pointer</param>
    /// <param name="relative">Current VM relative base</param>
    /// <param name="data">VM specific data</param>
    /// <param name="modes">Parameter modes</param>
    private static IntcodeVM.VMState Teq(ref int pointer, ref int relative, in IntcodeVM.VMData data, Modes modes)
    {
        ref long a = ref GetOperand(pointer++, relative, data.memory, modes.first);
        ref long b = ref GetOperand(pointer++, relative, data.memory, modes.second);
        ref long c = ref GetOperand(pointer++, relative, data.memory, modes.third);

        c = a == b ? TRUE : FALSE;
        return IntcodeVM.VMState.RUNNING;
    }

    /// <summary>
    /// REL Instruction, sets the relative base to the first operand
    /// </summary>
    /// <param name="pointer">Current VM pointer</param>
    /// <param name="relative">Current VM relative base</param>
    /// <param name="data">VM specific data</param>
    /// <param name="modes">Parameter modes</param>
    private static IntcodeVM.VMState Rel(ref int pointer, ref int relative, in IntcodeVM.VMData data, Modes modes)
    {
        ref long a = ref GetOperand(pointer++, relative, data.memory, modes.first);

        relative += (int)a;
        return IntcodeVM.VMState.RUNNING;
    }

    /// <summary>
    /// NOP Instruction, increments pointer only
    /// </summary>
    /// <param name="pointer">Current VM pointer</param>
    /// <param name="relative">Current VM relative base</param>
    /// <param name="data">VM specific data</param>
    /// <param name="modes">Parameter modes</param>
    private static IntcodeVM.VMState Nop(ref int pointer, ref int relative, in IntcodeVM.VMData data, Modes modes)
    {
        return IntcodeVM.VMState.RUNNING;
    }

    /// <summary>
    /// HLT Instruction, sets the pointer into the halted state
    /// </summary>
    /// <param name="pointer">Current VM pointer</param>
    /// <param name="relative">Current VM relative base</param>
    /// <param name="data">VM specific data</param>
    /// <param name="modes">Parameter modes</param>
    /// ReSharper disable once RedundantAssignment
    private static IntcodeVM.VMState Hlt(ref int pointer, ref int relative, in IntcodeVM.VMData data, Modes modes)
    {
        pointer = IntcodeVM.HALT;
        return IntcodeVM.VMState.HALTED;
    }

    /// <summary>
    /// Returns the operands in memory for the instruction at the given pointer
    /// </summary>
    /// <param name="pointer">Pointer address of the operand to get</param>
    /// <param name="relative">Current VM relative base</param>
    /// <param name="memory">Memory of the VM</param>
    /// <param name="mode">Parameter mode</param>
    /// <returns>The operand for the given instruction</returns>
    /// <exception cref="InvalidEnumArgumentException">If an invalid ParamModes is detected</exception>
    /// ReSharper disable once SuggestBaseTypeForParameter - Cannot be IList because of the ref return
    private static ref long GetOperand(int pointer, int relative, Memory<long> memory, ParamModes mode)
    {
        //ReSharper disable once ConvertSwitchStatementToSwitchExpression - Cannot use a switch expression because of the ref return
        switch (mode)
        {
            case ParamModes.POSITION:
                return ref memory.Span[(int)memory.Span[pointer]];
            case ParamModes.IMMEDIATE:
                return ref memory.Span[pointer];
            case ParamModes.RELATIVE:
                return ref memory.Span[(int)memory.Span[pointer] + relative];

            default:
                throw new InvalidEnumArgumentException(nameof(mode), (int)mode, typeof(ParamModes));
        }
    }
    #endregion
}