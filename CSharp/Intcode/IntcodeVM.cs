using System;
using System.ComponentModel;
using Operation = AdventOfCode.Intcode.Operations.Operation;

namespace AdventOfCode.Intcode
{
    /// <summary>
    /// Intcode computer Virtual Machine
    /// </summary>
    public class IntcodeVM
    {
        #region Constants
        /// <summary>
        /// Halted pointer state
        /// </summary>
        public const int HALT = -1;
        /// <summary>
        /// Default pointer state
        /// </summary>
        public const int DEFAULT = 0;
        
        private const StringSplitOptions OPTIONS = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        private static readonly char[] splitters = { ',' };
        #endregion
        
        #region Fields
        /// <summary>
        /// Intcode program memory pointer
        /// </summary>
        private int pointer = DEFAULT;
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
                int op = this.Fetch;                         //Fetch
                Operation operation = Operations.Decode(op); //Decode
                operation(this.memory, ref this.pointer);    //Execute
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
            this.pointer = DEFAULT;
            this.originalState.CopyTo(this.memory);
        }
        #endregion
    }
}
