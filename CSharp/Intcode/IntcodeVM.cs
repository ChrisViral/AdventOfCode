using System;
using System.ComponentModel;
using System.IO;
using Instruction = AdventOfCode.Intcode.Instructions.Instruction;
using Modes = AdventOfCode.Intcode.Instructions.Modes;

namespace AdventOfCode.Intcode
{
    /// <summary>
    /// Intcode computer Virtual Machine, using a Fetch/Decode/Execute architecture, and input and output memory streams
    /// </summary>
    public class IntcodeVM : IDisposable
    {
        /// <summary>
        /// VM specific data
        /// </summary>
        public readonly struct VMData
        {
            #region Fields
            /// <summary>Memory of the VM</summary>
            public readonly int[] memory;
            /// <summary>Input function of the VM</summary>
            public readonly Func<int> getInput;
            /// <summary>Output function of the VM</summary>
            public readonly Action<int> setOutput;
            #endregion
            
            #region constructors
            /// <summary>
            /// Creates new VM data from the given IntcodeVM
            /// </summary>
            /// <param name="vm">VM to create the data for</param>
            public VMData(IntcodeVM vm)
            {
                this.memory = vm.memory;
                this.getInput = vm.GetNextInput;
                this.setOutput = vm.SetNextOutput;
            }
            #endregion
        }
        
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
        /// <summary>Intcode program memory pointer</summary>
        private int pointer = DEFAULT;
        /// <summary>Intcode program memory</summary>
        private readonly int[] memory;
        /// <summary>Original memory state of the program</summary>
        private readonly ReadOnlyMemory<int> originalState;
        /// <summary>The input stream of the IntcodeVM</summary>
        private readonly MemoryStream? inputStream;
        /// <summary>The output stream of the IntcodeVM</summary>
        private readonly MemoryStream? outputStream;
        /// <summary>The input stream reader of the IntcodeVM</summary>
        private readonly BinaryReader? inputReader;
        /// <summary>The output stream writer of the IntcodeVM</summary>
        private readonly BinaryWriter? outputWriter;
        /// <summary>Data relating to the VM</summary>
        private readonly VMData data;
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
        
        /// <summary>
        /// If the VM has disposed of it's input and output streams
        /// </summary>
        public bool IsDisposed { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Intcode VM by parsing the given code
        /// </summary>
        /// <param name="code">Comma separated Intcode to parse</param>
        /// <param name="inputStream">The input Stream for this Intcode VM</param>
        /// <param name="outputStream">The output Steam for this Intcode VM</param>
        public IntcodeVM(string code, MemoryStream? inputStream = null, MemoryStream? outputStream = null)
        {
            this.originalState = Array.ConvertAll(code.Split(splitters, OPTIONS), int.Parse);
            this.memory = new int[this.originalState.Length];
            this.originalState.CopyTo(this.memory);

            this.inputStream = inputStream;
            this.outputStream = outputStream;
            if (inputStream is not null)
            {
                this.inputStream = inputStream;
                this.inputReader = new BinaryReader(inputStream);
            }

            if (outputStream is not null)
            {
                this.outputStream = outputStream;
                this.outputWriter = new BinaryWriter(outputStream);
            }

            this.data = new VMData(this);
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// Runs the Intcode VM until it reaches a halted state, then returns the value at the specified address.
        /// </summary>
        /// <param name="resultAddress">Address of the return value, defaults to null</param>
        /// <returns>Value at the return address after the VM halts if there is one, otherwise null</returns>
        /// <exception cref="InvalidOperationException">If the VM is already halted when started</exception>
        /// <exception cref="InvalidEnumArgumentException">If an Invalid Opcode is detected</exception>
        public int? Run(int? resultAddress = null)
        {
            //Make sure we aren't already halted
            if (this.IsHalted) throw new InvalidOperationException("Intcode program is already in a halted state and must be reset to run again");
 
            //Program loop
            while (this.pointer is not HALT)
            {
                //Fetch
                int op = this.Fetch;
                //Decode
                (Instruction instruction, Modes modes) = Instructions.Decode(op);
                //Execute
                instruction(ref this.pointer, this.data, modes);
            }

            //Use the address currently stored in result to store the out value
            return resultAddress.HasValue ? this.memory[resultAddress.Value] : null;
        }

        /// <summary>
        /// Runs the Intcode VM until it reaches a halted state, then returns the value at the specified address.<br/>
        /// The "noun" and "verb" are values inserted into the first and second operand of the first instruction.
        /// </summary>
        /// <param name="noun">Value to insert into the first operand</param>
        /// <param name="verb">Value to insert into the second operand</param>
        /// <param name="resultAddress">Address of the return value, defaults to null</param>
        /// <returns>Value at the return address after the VM halts if there is one, otherwise null</returns>
        /// <exception cref="InvalidOperationException">If the VM is already halted when started</exception>
        /// <exception cref="InvalidEnumArgumentException">If an Invalid Opcode is detected</exception>
        public int? Run(int noun, int verb, int? resultAddress = null)
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
            if (!this.IsDisposed)
            {
                this.pointer = DEFAULT;
                this.originalState.CopyTo(this.memory);

                this.inputStream?.Seek(0L, SeekOrigin.Begin);
                this.outputStream?.Seek(0L, SeekOrigin.Begin);
                this.outputStream?.SetLength(0L);
            }
        }

        /// <summary>
        /// Gets the output array from this Intcode VM
        /// </summary>
        /// <returns>A new copy of the current output array of the VM</returns>
        public int[] GetOutput()
        {
            if (this.outputStream is null) return Array.Empty<int>();
            
            byte[] serialized = this.outputStream.ToArray();
            int[] output = new int[serialized.Length / sizeof(int)];
            Buffer.BlockCopy(serialized, 0, output, 0, serialized.Length);
            return output;
        }

        /// <summary>
        /// Gets the next available int from the input reader
        /// </summary>
        /// <returns>The next integer in the input stream</returns>
        /// <exception cref="InvalidOperationException">If no input stream is specified</exception>
        private int GetNextInput() => this.inputReader?.ReadInt32() ?? throw new InvalidOperationException("Cannot get input, no input stream set.");

        /// <summary>
        /// Adds an integer to the output stream
        /// </summary>
        /// <param name="output">Value to add to the output</param>
        /// <exception cref="InvalidOperationException">If no output stream is specified</exception>
        private void SetNextOutput(int output)
        {
            if (this.outputWriter is null) throw new InvalidOperationException("Cannot set output, no output stream set");
            
            this.outputWriter.Write(output);
        }

        /// <summary>
        /// Disposes the IntcodeVM and releases all resources
        /// </summary>
        /// <param name="closeStreams">If the underlying input and output streams should also be closed</param>
        public void Dispose(bool closeStreams = false)
        {
            if (!this.IsDisposed)
            {
                if (closeStreams)
                {
                    this.inputStream?.Close();
                    this.outputStream?.Close();
                }
                
                this.inputReader?.Dispose();
                this.outputWriter?.Dispose();

                this.IsDisposed = true;
            }
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        void IDisposable.Dispose() => Dispose();
        #endregion
    }
}
