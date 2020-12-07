using System.ComponentModel;

namespace AdventOfCode.Intcode
{
    /// <summary>
    /// Intcode Operations
    /// </summary>
    public static class Operations
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
        public delegate void Operation(int[] memory, ref int pointer);
        
        #region Static methods
        /// <summary>
        /// Decodes an opcode into it's associated operation
        /// </summary>
        /// <param name="opcode">Opcode to decode</param>
        /// <returns>The Operation this Opcode refers to</returns>
        /// <exception cref="InvalidEnumArgumentException">If an Invalid Opcode is detected</exception>
        public static Operation Decode(int opcode)
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
        /// NOP Operation, increments pointer only
        /// </summary>
        /// <param name="memory">Current VM memory</param>
        /// <param name="pointer">Current VM pointer</param>
        private static void Nop(int[] memory, ref int pointer) => pointer++;
        
        /// <summary>
        /// ADD Operation, adds the values in the addresses of the first and second arguments into the address of the third argument
        /// </summary>
        /// <param name="memory">Current VM memory</param>
        /// <param name="pointer">Current VM pointer</param>
        private static void Add(int[] memory, ref int pointer)
        {
            memory[pointer + 3] = memory[pointer + 1] + memory[pointer + 2];
            pointer += 4;
        }

        /// <summary>
        /// MUL Operation, multiplies the values in the addresses of the first and second arguments into the address of the third argument
        /// </summary>
        /// <param name="memory">Current VM memory</param>
        /// <param name="pointer">Current VM pointer</param>
        private static void Mul(int[] memory, ref int pointer)
        {
            memory[pointer + 3] = memory[pointer + 1] * memory[pointer + 2];
            pointer += 4;
        }

        /// <summary>
        /// HLT Operation, sets the pointer into the halted state
        /// </summary>
        /// <param name="memory">Current VM memory</param>
        /// <param name="pointer">Current VM pointer</param>
        private static void Hlt(int[] memory, ref int pointer) => pointer = IntcodeVM.HALT;
        #endregion
    }
}
