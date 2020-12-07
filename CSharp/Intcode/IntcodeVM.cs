using System;
using System.ComponentModel;

namespace AdventOfCode.Intcode
{
    /// <summary>
    /// Intcode computer Virtual Machine
    /// </summary>
    public class IntcodeVM
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

        #region Constants
        private const int HALT = -1;
        private const StringSplitOptions OPTIONS = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        private static readonly char[] splitters = { ',' };
        #endregion
        
        #region Fields
        /// <summary>
        /// Intcode program memory pointer
        /// </summary>
        private int pointer;
        /// <summary>
        /// Intcode program memory
        /// </summary>
        private readonly int[] memory;
        /// <summary>
        /// Original memory state of the program
        /// </summary>
        private readonly ReadOnlyMemory<int> originalState;
        #endregion
        
        #region Properties
        /// <summary>
        /// If the Intcode VM is currently halted
        /// </summary>
        public bool IsHalted => this.pointer is HALT;

        /// <summary>
        /// Fetches the current instruction in memory
        /// </summary>
        private int Fetch => this.memory[this.pointer];
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Intcode VM by parsing the given code
        /// </summary>
        /// <param name="code">Comma separated Intcode to parse</param>
        public IntcodeVM(string code)
        {
            this.originalState = Array.ConvertAll(code.Split(splitters, OPTIONS), int.Parse);
            this.memory = new int[this.originalState.Length];
            this.originalState.CopyTo(this.memory);
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// Runs the Intcode VM until it reaches a halted state, then returns the value at the specified address.
        /// </summary>
        /// <param name="resultAddress">Address of the return value</param>
        /// <returns>Value at the return address after the VM halts</returns>
        /// <exception cref="InvalidOperationException">If the VM is already halted when started</exception>
        /// <exception cref="InvalidEnumArgumentException">If an Invalid Opcode is detected</exception>
        public int Run(int resultAddress = 0)
        {
            //Make sure we aren't already halted
            if (this.IsHalted) throw new InvalidOperationException("Intcode program is already in a halted state and must be reset to run again");
 
            //Program loop
            while (this.pointer is not HALT)
            {
                int op = this.Fetch;                        //Fetch
                Operation operation = Decode(op);           //Decode
                operation(this.memory, ref this.pointer);   //Execute
            }

            //Use the address currently stored in result to store the out value
            return this.memory[resultAddress];
        }

        /// <summary>
        /// Runs the Intcode VM until it reaches a halted state, then returns the value at the specified address.<br/>
        /// The "noun" and "verb" are values inserted into the first and second operand of the first instruction.
        /// </summary>
        /// <param name="noun">Value to insert into the first operand</param>
        /// <param name="verb">Value to insert into the second operand</param>
        /// <param name="resultAddress">Address of the return value</param>
        /// <returns>Value at the return address after the VM halts</returns>
        /// <exception cref="InvalidOperationException">If the VM is already halted when started</exception>
        /// <exception cref="InvalidEnumArgumentException">If an Invalid Opcode is detected</exception>
        public int Run(int noun, int verb, int resultAddress = 0)
        {
            //Make sure we aren't already halted
            if (this.IsHalted) throw new InvalidOperationException("Intcode program is already in a halted state and must be reset to run again");
            
            //Set the noun and verb
            this.memory[1] = noun;
            this.memory[2] = verb;
            
            //Run normally
            return Run(resultAddress);
        }

        /// <summary>
        /// Resets the Intcode VM to it's original state so it can be run again
        /// </summary>
        public void Reset()
        {
            this.pointer = 0;
            this.originalState.CopyTo(this.memory);
        }
        #endregion
        
        #region Static methods
        /// <summary>
        /// Decodes an opcode into it's associated operation
        /// </summary>
        /// <param name="opcode">Opcode to decode</param>
        /// <returns>The Operation this Opcode refers to</returns>
        /// <exception cref="InvalidEnumArgumentException">If an Invalid Opcode is detected</exception>
        private static Operation Decode(int opcode)
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
        #endregion
        
        #region Operations
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
        private static void Hlt(int[] memory, ref int pointer) => pointer = HALT;
        #endregion
    }
}
