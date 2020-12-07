using System;
using System.Collections.Generic;
using System.ComponentModel;
using VMData = AdventOfCode.Intcode.IntcodeVM.VMData;

namespace AdventOfCode.Intcode
{
    /// <summary>
    /// Intcode Instructions
    /// </summary>
    public static class Instructions
    {
        /// <summary>
        /// Intcode Opcodes
        /// </summary>
        public enum Opcodes
        {
            ADD = 1,
            MUL = 2,
            INP = 3,
            OUT = 4,
            
            NOP = 0,
            HLT = 99
        }
        
        /// <summary>
        /// Parameter modes
        /// </summary>
        public enum ParamModes
        {
            POSITION  = 0,
            IMMEDIATE = 1
        }

        /// <summary>
        /// Parameter Modes
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
            /// Creates a new set of Modes from the given input string
            /// </summary>
            /// <param name="modes">Value to parse the modes from</param>
            /// <exception cref="ArgumentException">If the input string has the inappropriate length</exception>
            /// <exception cref="InvalidEnumArgumentException">If one of the parsed modes is not a valid member of the enum</exception>
            public Modes(string modes)
            {
                if (modes.Length != 3) throw new ArgumentException($"Modes length is invalid, got {modes.Length}, expected 3", nameof(modes));

                this.first  = (ParamModes)(modes[2] - '0');
                if (!Enum.IsDefined(this.first)) throw new InvalidEnumArgumentException(nameof(this.first), (int)this.first, typeof(ParamModes));
                
                this.second = (ParamModes)(modes[1] - '0');
                if (!Enum.IsDefined(this.second)) throw new InvalidEnumArgumentException(nameof(this.second), (int)this.second, typeof(ParamModes));
                
                this.third  = (ParamModes)(modes[0] - '0');
                if (!Enum.IsDefined(this.third)) throw new InvalidEnumArgumentException(nameof(this.third), (int)this.third, typeof(ParamModes));
            }
            #endregion
        }

        /// <summary>
        /// Intcode operation delegate
        /// </summary>
        /// <param name="memory">Memory of the VM</param>
        /// <param name="pointer">Pointer of the VM</param>
        public delegate void Instruction(ref int pointer, in VMData data, in Modes modes);
        
        #region Static methods
        /// <summary>
        /// Decodes an opcode into it's associated instruction
        /// </summary>
        /// <param name="opcode">Opcode to decode</param>
        /// <returns>A Tuple containing the Instruction this Opcode refers to and it's parameter modes</returns>
        /// <exception cref="ArgumentException">If the Modes input string is of invalid length</exception>
        /// <exception cref="InvalidEnumArgumentException">If an invalid Opcodes or ParamModes is detected</exception>
        public static (Instruction instruction, Modes modes) Decode(int opcode)
        {
            string full = opcode.ToString().PadLeft(5, '0');
            Modes modes = new(full[..3]);
            Instruction instruction = (Opcodes)int.Parse(full[3..]) switch
            {
                //Instructions
                Opcodes.ADD => Add,
                Opcodes.MUL => Mul,
                Opcodes.INP => Inp,
                Opcodes.OUT => Out,
                
                //Nop, Halt, and unknown
                Opcodes.NOP => Nop,
                Opcodes.HLT => Hlt,
                _           => throw new InvalidEnumArgumentException(nameof(opcode), opcode, typeof(Opcodes))
            };

            return (instruction, modes);
        }

        /// <summary>
        /// ADD Instruction, adds the values in the addresses of the first and second arguments into the address of the third argument
        /// </summary>
        /// <param name="pointer">Current VM pointer</param>
        /// <param name="data">VM specific data</param>
        /// <param name="modes">Parameter modes</param>
        private static void Add(ref int pointer, in VMData data, in Modes modes)
        {
            (int a, int b, int c) = GetOperands(data.memory, pointer, modes);
            data.memory[c] = a + b;
            pointer += 4;
        }

        /// <summary>
        /// MUL Instruction, multiplies the values in the addresses of the first and second arguments into the address of the third argument
        /// </summary>
        /// <param name="pointer">Current VM pointer</param>
        /// <param name="data">VM specific data</param>
        /// <param name="modes">Parameter modes</param>
        private static void Mul(ref int pointer, in VMData data, in Modes modes)
        {
            (int a, int b, int c) = GetOperands(data.memory, pointer, modes);
            data.memory[c] = a * b;
            pointer += 4;
        }

        /// <summary>
        /// INP Instruction, gets a value from the input stream and puts it at the address of the first operand
        /// </summary>
        /// <param name="pointer">Current VM pointer</param>
        /// <param name="data">VM specific data</param>
        /// <param name="modes">Parameter modes</param>
        private static void Inp(ref int pointer, in VMData data, in Modes modes)
        {
            int a = GetOperand(data.memory, pointer + 1, ParamModes.IMMEDIATE); //Input parameter is always an address
            data.memory[a] = data.getInput();
            pointer += 2;
        }

        /// <summary>
        /// OUT Instruction, puts the value at the address of the first operand in the output stream
        /// </summary>
        /// <param name="pointer">Current VM pointer</param>
        /// <param name="data">VM specific data</param>
        /// <param name="modes">Parameter modes</param>
        private static void Out(ref int pointer, in VMData data, in Modes modes)
        {
            int a = GetOperand(data.memory, pointer + 1, modes.first);
            data.setOutput(a);
            pointer += 2;
        }
        
        /// <summary>
        /// NOP Instruction, increments pointer only
        /// </summary>
        /// <param name="pointer">Current VM pointer</param>
        /// <param name="data">VM specific data</param>
        /// <param name="modes">Parameter modes</param>
        private static void Nop(ref int pointer, in VMData data, in Modes modes) => pointer++;

        /// <summary>
        /// HLT Instruction, sets the pointer into the halted state
        /// </summary>
        /// <param name="pointer">Current VM pointer</param>
        /// <param name="data">VM specific data</param>
        /// <param name="modes">Parameter modes</param>
        /// ReSharper disable once RedundantAssignment
        private static void Hlt(ref int pointer, in VMData data, in Modes modes) => pointer = IntcodeVM.HALT;

        /// <summary>
        /// Returns the an operands in memory for the instruction at the given pointer
        /// </summary>
        /// <param name="memory">Memory of the VM</param>
        /// <param name="pointer">Pointer address of the operand to get</param>
        /// <param name="mode">Parameter mode</param>
        /// <returns>The operand for the given instruction</returns>
        /// <exception cref="InvalidEnumArgumentException">If an invalid ParamModes is detected</exception>
        private static int GetOperand(IReadOnlyList<int> memory, int pointer, in ParamModes mode)
        {
            return mode switch
            {
                ParamModes.POSITION  => memory[memory[pointer]],
                ParamModes.IMMEDIATE => memory[pointer],
                _                    => throw new InvalidEnumArgumentException(nameof(mode), (int)mode, typeof(ParamModes))
            };
        }

        /// <summary>
        /// Returns the three operands in memory for the instruction at the given pointer
        /// </summary>
        /// <param name="memory">Memory of the VM</param>
        /// <param name="pointer">Pointer at the current instruction</param>
        /// <param name="modes">Parameter modes</param>
        /// <returns>The three operands for the given instruction</returns>
        /// <exception cref="InvalidEnumArgumentException">If an invalid ParamModes is detected</exception>
        private static (int, int, int) GetOperands(IReadOnlyList<int> memory, int pointer, in Modes modes)
        {
            int a = GetOperand(memory, pointer + 1, modes.first);
            int b = GetOperand(memory, pointer + 2, modes.second);
            int c = GetOperand(memory, pointer + 3, ParamModes.IMMEDIATE); //Last parameter is the write address, always immediate
            return (a, b, c);
        }
        #endregion
    }
}
