using System;
using System.Collections.Generic;
using System.ComponentModel;
using AdventOfCode.Utils;
using Instruction = AdventOfCode.Intcode.Instructions.Instruction;
using Modes = AdventOfCode.Intcode.Instructions.Modes;

namespace AdventOfCode.Intcode
{
    /// <summary>
    /// Intcode computer Virtual Machine, using a Fetch/Decode/Execute architecture, and input and output queues
    /// </summary>
    public class IntcodeVM
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
            public readonly long[] memory;
            /// <summary>Input function of the VM</summary>
            public readonly InputGetter getInput;
            /// <summary>Output function of the VM</summary>
            public readonly OutputSetter setOutput;
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
                this.setOutput = vm.AddOutput;
            }
            #endregion
        }

        /// <summary>
        /// Delegate which gets the next input value
        /// </summary>
        /// <param name="input">The returned input</param>
        /// <returns>True if an input was fetched, false otherwise</returns>
        public delegate bool InputGetter(out long input);

        /// <summary>
        /// Delegate which sets the next output value
        /// </summary>
        /// <param name="output">Output value to set</param>
        public delegate void OutputSetter(long output);

        /// <summary>
        /// Output event
        /// </summary>
        public event Action OnOutput;

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
        /// Default output stream size
        /// </summary>
        public const int DEFAULT_SIZE = 16;
        /// <summary>
        /// Extra buffer memory added to the VM (2k integers, 16kb memory)
        /// </summary>
        public const int BUFFER_SIZE = 2048;
        /// <summary>
        /// Input parsing splitting options
        /// </summary>
        private const StringSplitOptions OPTIONS = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        /// <summary>
        /// Input parsing splitting character
        /// </summary>
        private static readonly char[] splitters = { ',' };
        #endregion
        
        #region Fields
        /// <summary>Intcode program memory pointer</summary>
        private int pointer = DEFAULT;
        /// <summary>Relative base of the VM</summary>
        private int relative = DEFAULT;
        /// <summary>Intcode program memory</summary>
        private readonly long[] memory;
        /// <summary>Original memory state of the program</summary>
        private readonly ReadOnlyMemory<long> originalState;
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
        private long Fetch => this.memory[this.pointer];

        /// <summary>
        /// The input queue of the IntcodeVM
        /// </summary>
        public Queue<long> In { get; set; }
        
        /// <summary>
        /// The output queue of the IntcodeVM
        /// </summary>
        public Queue<long> Out { get; set; }

        /// <summary>
        /// If the VM has any available output values left
        /// </summary>
        public bool HasOutputs => !this.Out.IsEmpty();
        #endregion

        #region Indexers
        /// <summary>
        /// Accesses the memory of the VM
        /// </summary>
        /// <param name="index">Index to access</param>
        /// <returns>Value in the memory at the specified index</returns>
        public long this[int index]
        {
            get => this.memory[index];
            set => this.memory[index] = value;
        }

        /// <summary>
        /// Accesses the memory of the VM
        /// </summary>
        /// <param name="index">Index to access</param>
        /// <returns>Value in the memory at the specified index</returns>
        public long this[Index index]
        {
            get => this.memory[index];
            set => this.memory[index] = value;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Intcode VM by parsing the given code, and with empty input and output queues
        /// </summary>
        /// <param name="code">Comma separated Intcode to parse</param>
        public IntcodeVM(string code) : this(code, new Queue<long>(DEFAULT_SIZE), new Queue<long>(DEFAULT_SIZE)) { }
        
        /// <summary>
        /// Creates a new Intcode VM by parsing the given code, and with the input and output values
        /// </summary>
        /// <param name="code">Comma separated Intcode to parse</param>
        /// <param name="input">Input values</param>
        public IntcodeVM(string code, IEnumerable<long> input) : this(code, new Queue<long>(input), new Queue<long>(DEFAULT_SIZE)) { }
        
        /// <summary>
        /// Creates a new Intcode VM by parsing the given code, and with the specified input and output Queues
        /// </summary>
        /// <param name="code">Comma separated Intcode to parse</param>
        /// <param name="input">The input Queue for this Intcode VM</param>
        /// <param name="output">The output Queue for this Intcode VM</param>
        public IntcodeVM(string code, Queue<long> input, Queue<long> output)
        {
            this.originalState = Array.ConvertAll(code.Split(splitters, OPTIONS), long.Parse);
            this.memory = new long[this.originalState.Length + BUFFER_SIZE];
            this.originalState.CopyTo(this.memory);
            this.In = input;
            this.Out = output;
            this.data = new VMData(this);
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// Runs the Intcode VM until it reaches a stopped state, then returns it's current state.
        /// </summary>
        /// <returns>Current state of the VM</returns>
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
                long opcode = this.Fetch;
                //Decode
                (Instruction instruction, Modes modes) = Instructions.Decode(opcode);
                //Execute
                this.State = instruction(ref this.pointer, ref this.relative, this.data, modes);
            }

            return this.State;
        }

        /// <summary>
        /// Resets the Intcode VM to it's original state so it can be run again
        /// </summary>
        public void Reset()
        {
            this.pointer = DEFAULT;
            this.relative = DEFAULT;
            this.originalState.CopyTo(this.memory);
                
            this.In.Clear();
            this.Out.Clear();

            this.State = VMStates.READY;
        }

        /// <summary>
        /// Sets a new input queue with the specified data
        /// </summary>
        /// <param name="input">Data to set as input</param>
        public void SetInput(IEnumerable<long> input) => this.In = new Queue<long>(input);

        /// <summary>
        /// Adds the given value to the input queue
        /// </summary>
        /// <param name="value">Value to add</param>
        public void AddInput(long value) => this.In.Enqueue(value);

        /// <summary>
        /// Adds the string as a character array to the input values
        /// </summary>
        /// <param name="value">String to add</param>
        public void AddInput(string value) => value.ForEach(c => this.In.Enqueue(c));
        
        /// <summary>
        /// Gets the next available int from the input if available
        /// </summary>
        /// <returns>The next integer in the input queue</returns>
        private bool GetNextInput(out long input) => this.In.TryDequeue(out input);

        /// <summary>
        /// Adds an integer to the output stream
        /// </summary>
        /// <param name="output">Value to add to the output</param>
        /// <exception cref="InvalidOperationException">If no output stream is specified</exception>
        private void AddOutput(long output)
        {
            this.Out.Enqueue(output);
            this.OnOutput?.Invoke();
        }

        /// <summary>
        /// Gets the next available int from the output
        /// </summary>
        /// <returns>The next output value</returns>
        public long GetNextOutput() => this.Out.Dequeue();

        /// <summary>
        /// Gets all the output from this Intcode VM
        /// </summary>
        /// <returns>A new copy of the current output  of the VM in an array</returns>
        public long[] GetOutput()
        {
            if (this.Out.Count is 0) return Array.Empty<long>();
            
            long[] output = new long[this.Out.Count];
            this.Out.CopyTo(output, 0);
            this.Out.Clear();
            return output;
        }

        /// <summary>
        /// Gets the next two outputs into two variables through deconstruction
        /// </summary>
        /// <param name="a">First output</param>
        /// <param name="b">Second output</param>
        public void Deconstruct(out long a, out long b)
        {
            a = GetNextOutput();
            b = GetNextOutput();
        }
        
        /// <summary>
        /// Gets the next three outputs into two variables through deconstruction
        /// </summary>
        /// <param name="a">First output</param>
        /// <param name="b">Second output</param>
        /// <param name="c">Third output</param>
        public void Deconstruct(out long a, out long b, out long c)
        {
            a = GetNextOutput();
            b = GetNextOutput();
            c = GetNextOutput();
        }
        
        /// <summary>
        /// Gets the next four outputs into two variables through deconstruction
        /// </summary>
        /// <param name="a">First output</param>
        /// <param name="b">Second output</param>
        /// <param name="c">Third output</param>
        /// <param name="d">Fourth output</param>
        public void Deconstruct(out long a, out long b, out long c, out long d)
        {
            a = GetNextOutput();
            b = GetNextOutput();
            c = GetNextOutput();
            d = GetNextOutput();
        }
        
        /// <summary>
        /// Gets the next five outputs into two variables through deconstruction
        /// </summary>
        /// <param name="a">First output</param>
        /// <param name="b">Second output</param>
        /// <param name="c">Third output</param>
        /// <param name="d">Fourth output</param>
        /// <param name="e">Fifth output</param>
        public void Deconstruct(out long a, out long b, out long c, out long d, out long e)
        {
            a = GetNextOutput();
            b = GetNextOutput();
            c = GetNextOutput();
            d = GetNextOutput();
            e = GetNextOutput();
        }
        #endregion
    }
}