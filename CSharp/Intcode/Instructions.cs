using System;
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
            ADD = 1,    //Add
            MUL = 2,    //Multiply
            INP = 3,    //Input
            OUT = 4,    //Output
            JNZ = 5,    //Jump Not Zero
            JEZ = 6,    //Jump Equal Zero
            TLT = 7,    //Test Less Than
            TEQ = 8,    //Test Equals
            
            NOP = 0,    //No Op
            HLT = 99    //Halt
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
        /// <param name="pointer">Pointer of the VM</param>
        /// <param name="data">Intcode VM data</param>
        /// <param name="modes">Operand modes</param>
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
                Opcodes.JNZ => Jnz,
                Opcodes.JEZ => Jez,
                Opcodes.TLT => Tlt,
                Opcodes.TEQ => Teq,
                
                //Nop, Halt, and unknown
                Opcodes.NOP => Nop,
                Opcodes.HLT => Hlt,
                _           => throw new InvalidEnumArgumentException(nameof(opcode), opcode, typeof(Opcodes))
            };

            return (instruction, modes);
        }

        /// <summary>
        /// ADD Instruction, adds the values of the first and second operands into the address of the third operand
        /// </summary>
        /// <param name="pointer">Current VM pointer</param>
        /// <param name="data">VM specific data</param>
        /// <param name="modes">Parameter modes</param>
        private static void Add(ref int pointer, in VMData data, in Modes modes)
        {
            ref int a = ref GetOperand(data.memory, pointer + 1, modes.first);
            ref int b = ref GetOperand(data.memory, pointer + 2, modes.second);
            ref int c = ref GetOperand(data.memory, pointer + 3, modes.third);
            
            c = a + b;
            pointer += 4;
        }

        /// <summary>
        /// MUL Instruction, multiplies the values of the first and second operands into the address of the third operand
        /// </summary>
        /// <param name="pointer">Current VM pointer</param>
        /// <param name="data">VM specific data</param>
        /// <param name="modes">Parameter modes</param>
        private static void Mul(ref int pointer, in VMData data, in Modes modes)
        {
            ref int a = ref GetOperand(data.memory, pointer + 1, modes.first);
            ref int b = ref GetOperand(data.memory, pointer + 2, modes.second);
            ref int c = ref GetOperand(data.memory, pointer + 3, modes.third);
            
            c = a * b;
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
            ref int a = ref GetOperand(data.memory, pointer + 1, modes.first);
            
            a = data.getInput();
            pointer += 2;
        }

        /// <summary>
        /// OUT Instruction, puts the value of the first operand in the output stream
        /// </summary>
        /// <param name="pointer">Current VM pointer</param>
        /// <param name="data">VM specific data</param>
        /// <param name="modes">Parameter modes</param>
        private static void Out(ref int pointer, in VMData data, in Modes modes)
        {
            ref int a = ref GetOperand(data.memory, pointer + 1, modes.first);
            
            data.setOutput(a);
            pointer += 2;
        }
        
        /// <summary>
        /// JNZ Instruction, if the first operand is not zero, sets the pointer to the value of the second operand
        /// </summary>
        /// <param name="pointer">Current VM pointer</param>
        /// <param name="data">VM specific data</param>
        /// <param name="modes">Parameter modes</param>
        private static void Jnz(ref int pointer, in VMData data, in Modes modes)
        {
            ref int a = ref GetOperand(data.memory, pointer + 1, modes.first);
            ref int b = ref GetOperand(data.memory, pointer + 2, modes.second);
            
            pointer = a is not 0 ? b : pointer + 3;
        }
        
        /// <summary>
        /// JEZ Instruction, if the first operand is zero, sets the pointer to the value of the second operand
        /// </summary>
        /// <param name="pointer">Current VM pointer</param>
        /// <param name="data">VM specific data</param>
        /// <param name="modes">Parameter modes</param>
        private static void Jez(ref int pointer, in VMData data, in Modes modes)
        {
            ref int a = ref GetOperand(data.memory, pointer + 1, modes.first);
            ref int b = ref GetOperand(data.memory, pointer + 2, modes.second);
            
            pointer = a is 0 ? b : pointer + 3;
        }
        
        /// <summary>
        /// TLT Instruction, if the first operand is less than the second operand, sets the third operand to 1, otherwise, sets the third operand to 0
        /// </summary>
        /// <param name="pointer">Current VM pointer</param>
        /// <param name="data">VM specific data</param>
        /// <param name="modes">Parameter modes</param>
        private static void Tlt(ref int pointer, in VMData data, in Modes modes)
        {
            ref int a = ref GetOperand(data.memory, pointer + 1, modes.first);
            ref int b = ref GetOperand(data.memory, pointer + 2, modes.second);
            ref int c = ref GetOperand(data.memory, pointer + 3, modes.third);
            
            c = a < b ? 1 : 0;
            pointer += 4;
        }
        
        /// <summary>
        /// TEQ Instruction, if the first operand is equal to the second operand, sets the third operand to 1, otherwise, sets the third operand to 0
        /// </summary>
        /// <param name="pointer">Current VM pointer</param>
        /// <param name="data">VM specific data</param>
        /// <param name="modes">Parameter modes</param>
        private static void Teq(ref int pointer, in VMData data, in Modes modes)
        {
            ref int a = ref GetOperand(data.memory, pointer + 1, modes.first);
            ref int b = ref GetOperand(data.memory, pointer + 2, modes.second);
            ref int c = ref GetOperand(data.memory, pointer + 3, modes.third);
            
            c = a == b ? 1 : 0;
            pointer += 4;
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
        /// ReSharper disable once SuggestBaseTypeForParameter - Cannot be IList because of the ref return
        private static ref int GetOperand(int[] memory, int pointer, in ParamModes mode)
        {
            //ReSharper disable once ConvertSwitchStatementToSwitchExpression - Cannot use a switch expression because of the ref return 
            switch (mode)
            {
                case ParamModes.POSITION:
                    return ref memory[memory[pointer]];
                case ParamModes.IMMEDIATE:
                    return ref memory[pointer];
                
                default:
                    throw new InvalidEnumArgumentException(nameof(mode), (int)mode, typeof(ParamModes));
            }
        }
        #endregion
    }
}
