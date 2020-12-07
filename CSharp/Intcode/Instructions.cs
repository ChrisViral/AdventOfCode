using System.Collections.Generic;
using System.ComponentModel;

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
            
            NOP = 0,
            HLT = 99
        }

        /// <summary>
        /// Intcode operation delegate
        /// </summary>
        /// <param name="memory">Memory of the VM</param>
        /// <param name="pointer">Pointer of the VM</param>
        public delegate void Instruction(int[] memory, ref int pointer);
        
        #region Static methods
        /// <summary>
        /// Decodes an opcode into it's associated instruction
        /// </summary>
        /// <param name="opcode">Opcode to decode</param>
        /// <returns>The Instruction this Opcode refers to</returns>
        /// <exception cref="InvalidEnumArgumentException">If an Invalid Opcode is detected</exception>
        public static Instruction Decode(int opcode)
        {
            return (Opcodes)opcode switch
            {
                Opcodes.NOP => Nop,
                Opcodes.ADD => Add,
                Opcodes.MUL => Mul,
                Opcodes.HLT => Hlt,
                _           => throw new InvalidEnumArgumentException(nameof(opcode), opcode, typeof(Opcodes))
            };
        }
        
        /// <summary>
        /// NOP Instruction, increments pointer only
        /// </summary>
        /// <param name="memory">Current VM memory</param>
        /// <param name="pointer">Current VM pointer</param>
        private static void Nop(int[] memory, ref int pointer) => pointer++;
        
        /// <summary>
        /// ADD Instruction, adds the values in the addresses of the first and second arguments into the address of the third argument
        /// </summary>
        /// <param name="memory">Current VM memory</param>
        /// <param name="pointer">Current VM pointer</param>
        private static void Add(int[] memory, ref int pointer)
        {
            (int a, int b, int c) = GetOperands(memory, pointer);
            memory[c] = memory[a] + memory[b];
            pointer += 4;
        }

        /// <summary>
        /// MUL Instruction, multiplies the values in the addresses of the first and second arguments into the address of the third argument
        /// </summary>
        /// <param name="memory">Current VM memory</param>
        /// <param name="pointer">Current VM pointer</param>
        private static void Mul(int[] memory, ref int pointer)
        {
            (int a, int b, int c) = GetOperands(memory, pointer);
            memory[c] = memory[a] * memory[b];
            pointer += 4;
        }

        /// <summary>
        /// HLT Instruction, sets the pointer into the halted state
        /// </summary>
        /// <param name="memory">Current VM memory</param>
        /// <param name="pointer">Current VM pointer</param>
        private static void Hlt(int[] memory, ref int pointer) => pointer = IntcodeVM.HALT;

        /// <summary>
        /// Returns the three operands in memory for the instruction at the given pointer
        /// </summary>
        /// <param name="memory">Memory of the VM</param>
        /// <param name="pointer">Pointer at the current instruction</param>
        /// <returns>The three operands for the given instruction</returns>
        private static (int, int, int) GetOperands(IReadOnlyList<int> memory, int pointer) => (memory[pointer + 1], memory[pointer + 2], memory[pointer + 3]);
        #endregion
    }
}
