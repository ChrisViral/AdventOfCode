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
        /// VM states
        /// </summary>
        public enum VMStates
        {
            READY,
            RUNNING,
            STALLED,
            HALTED
        }
        
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
        /// <summary>
        /// Default output stream size (1kb)
        /// </summary>
        public const int DEFAULT_SIZE = 1024;
        
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
        /// <summary>The input stream reader of the IntcodeVM</summary>
        private BinaryReader inputReader = null!;
        /// <summary>The input stream writer</summary>
        private BinaryWriter inputWriter = null!;
        /// <summary>The output stream writer of the IntcodeVM</summary>
        private BinaryWriter outputWriter = null!;
        /// <summary>Data relating to the VM</summary>
        private readonly VMData data;
        #endregion
        
        #region Properties
        /// <summary>
        /// The current VM State
        /// </summary>
        public VMStates State { get; private set; }
        
        /// <summary>
        /// If the Intcode VM is currently halted
        /// </summary>
        public bool IsHalted => this.State is VMStates.HALTED;

        /// <summary>
        /// Fetches the current instruction in memory
        /// </summary>
        private int Fetch => this.memory[this.pointer];
        
        /// <summary>
        /// If the VM has disposed of it's input and output streams
        /// </summary>
        public bool IsDisposed { get; private set; }

        private MemoryStream inputStream = null!;
        /// <summary>
        /// The input stream of the IntcodeVM
        /// </summary>
        public MemoryStream In
        {
            get => this.inputStream;
            set
            {
                this.inputStream = value;
                this.inputReader = new BinaryReader(this.inputStream);
                this.inputWriter = new BinaryWriter(this.inputStream);
            }
        }

        private MemoryStream outputStream = null!;
        /// <summary>
        /// The output stream of the IntcodeVM
        /// </summary>
        public MemoryStream Out
        {
            get => this.outputStream;
            set
            {
                this.outputStream = value;
                this.outputWriter = new BinaryWriter(this.outputStream);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Intcode VM by parsing the given code, and with an empty input buffer and resizable output buffer
        /// </summary>
        /// <param name="code">Comma separated Intcode to parse</param>
        public IntcodeVM(string code) : this(code, new MemoryStream(DEFAULT_SIZE), new MemoryStream(DEFAULT_SIZE)) { }
        
        /// <summary>
        /// Creates a new Intcode VM by parsing the given code, and with the specified combined input and output array
        /// </summary>
        /// <param name="code">Comma separated Intcode to parse</param>
        /// <param name="inputOutput">Combined input and output array</param>
        public IntcodeVM(string code, int[] inputOutput) : this(code, GetBuffer(inputOutput)) { }
        
        /// <summary>
        /// Creates a new Intcode VM by parsing the given code, and with the specified combined input and output buffer
        /// </summary>
        /// <param name="code">Comma separated Intcode to parse</param>
        /// <param name="inputOutputBuffer">Combined input and output buffer</param>
        public IntcodeVM(string code, byte[] inputOutputBuffer) : this(code, inputOutputBuffer, inputOutputBuffer) { }
        
        /// <summary>
        /// Creates a new Intcode VM by parsing the given code, and with the specified byte arrays as input and output buffers
        /// </summary>
        /// <param name="code">Comma separated Intcode to parse</param>
        /// <param name="input">Input buffer</param>
        /// <param name="output">Output buffer</param>
        public IntcodeVM(string code, byte[] input, byte[] output) : this(code, new MemoryStream(input), new MemoryStream(output)) { }
        
        /// <summary>
        /// Creates a new Intcode VM by parsing the given code, and with the specified input and output MemoryStreams
        /// </summary>
        /// <param name="code">Comma separated Intcode to parse</param>
        /// <param name="input">The input Stream for this Intcode VM</param>
        /// <param name="output">The output Steam for this Intcode VM</param>
        public IntcodeVM(string code, MemoryStream input, MemoryStream output)
        {
            this.originalState = Array.ConvertAll(code.Split(splitters, OPTIONS), int.Parse);
            this.memory = new int[this.originalState.Length];
            this.originalState.CopyTo(this.memory);
            this.In = input;
            this.Out = output;
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
        public VMStates Run()
        {
            //Make sure we aren't already halted
            if (this.IsHalted) throw new InvalidOperationException("Intcode program is already in a halted state and must be reset to run again");
 
            //Program loop
            this.State = VMStates.RUNNING;
            while (this.State is VMStates.RUNNING)
            {
                //Fetch
                int op = this.Fetch;
                //Decode
                (Instruction instruction, Modes modes) = Instructions.Decode(op);
                //Execute
                this.State = instruction(ref this.pointer, this.data, modes);
            }

            return this.State;
        }

        /// <summary>
        /// Runs the Intcode VM until it reaches a halted state, then returns the value at the specified address.<br/>
        /// The "noun" and "verb" are values inserted into the first and second operand of the first instruction.
        /// </summary>
        /// <param name="noun">Value to insert into the first operand</param>
        /// <param name="verb">Value to insert into the second operand</param>
        /// <param name="resultAddress">Address of the return value, defaults to null</param>
        /// <param name="result">Output parameter where the result is stored</param>
        /// <returns>Value at the return address after the VM halts if there is one, otherwise null</returns>
        /// <exception cref="InvalidOperationException">If the VM is already halted when started</exception>
        /// <exception cref="InvalidEnumArgumentException">If an Invalid Opcode is detected</exception>
        public VMStates Run(int noun, int verb, int resultAddress, out int result)
        {
            //Set noun and verb
            this.memory[1] = noun;
            this.memory[2] = verb;
            
            //Run normally
            VMStates state = Run();

            //Set result
            result = this.memory[resultAddress];

            return state;
        }

        /// <summary>
        /// Resets the input buffer to the start
        /// </summary>
        public void ResetInput()
        {
            if (!this.IsDisposed)
            {
                this.In.Seek(0L, SeekOrigin.Begin);
                this.inputWriter.Seek(0, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Resets and clears the input buffer
        /// </summary>
        public void ClearInput()
        {
            if (!this.IsDisposed)
            {
                this.In.Seek(0L, SeekOrigin.Begin);
                this.In.SetLength(0L);
                this.inputWriter.Seek(0, SeekOrigin.Begin);
            }
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
                
                ClearInput();
                this.Out.Seek(0L, SeekOrigin.Begin);
                this.Out.SetLength(0L);

                this.State = VMStates.READY;
            }
        }

        /// <summary>
        /// Sets the input array for this IntcodeVM
        /// </summary>
        /// <param name="array">Array to set as input</param>
        public void SetInput(int[] array)
        {
            this.In.Dispose();
            this.inputReader.Dispose();

            this.In = new MemoryStream(GetBuffer(array));
            this.inputReader = new BinaryReader(this.In);
        }

        /// <summary>
        /// Gets the output array from this Intcode VM
        /// </summary>
        /// <returns>A new copy of the current output array of the VM</returns>
        public int[] GetOutput()
        {
            if (this.Out.Length is 0) return Array.Empty<int>();
            
            byte[] serialized = this.Out.ToArray();
            int[] output = new int[serialized.Length / sizeof(int)];
            Buffer.BlockCopy(serialized, 0, output, 0, serialized.Length);
            return output;
        }

        /// <summary>
        /// Gets the next available int from the input reader
        /// </summary>
        /// <returns>The next integer in the input stream</returns>
        /// <exception cref="InvalidOperationException">If no input stream is specified</exception>
        private int GetNextInput() => this.inputReader.ReadInt32();

        /// <summary>
        /// Writes a value to the input stream
        /// </summary>
        /// <param name="value">Value to write</param>
        public void WriteToInput(int value) => this.inputWriter.Write(value);

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
        public void Dispose(bool closeStreams)
        {
            if (!this.IsDisposed)
            {
                if (closeStreams)
                {
                    this.In.Dispose();
                    this.Out.Dispose();
                }
                
                Dispose();
            }
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.inputReader.Dispose();
                this.outputWriter.Dispose();
                GC.SuppressFinalize(this);

                this.IsDisposed = true;
            }
        }
        #endregion
        
        #region Static methods
        /// <summary>
        /// Creates and returns the buffer for a given array
        /// </summary>
        /// <param name="array">Array to create the buffer for</param>
        /// <returns>Buffer of the array</returns>
        public static byte[] GetBuffer(int[] array)
        {
            byte[] buffer = new byte[array.Length * sizeof(int)];
            Buffer.BlockCopy(array, 0, buffer, 0, buffer.Length);
            return buffer;
        }
        #endregion
    }
}