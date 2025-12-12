using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AdventOfCode.Intcode.Input;
using AdventOfCode.Intcode.Output;

namespace AdventOfCode.Intcode;

// ReSharper disable RedundantOverflowCheckingContext

/// <summary>
/// Intcode VM
/// </summary>
[DebuggerDisplay("State: {Status}")]
public sealed unsafe partial class IntcodeVM : IDisposable
{
    /// <summary>
    /// VM state
    /// </summary>
    public enum State
    {
        /// <summary>
        /// Ready to run
        /// </summary>
        READY,
        /// <summary>
        /// Currently running
        /// </summary>
        RUNNING,
        /// <summary>
        /// Stalled for input
        /// </summary>
        STALLED,
        /// <summary>
        /// Execution halted
        /// </summary>
        HALTED,
    }

    /// <summary>
    /// VM Memory buffer size (8kb of long values)
    /// </summary>
    private const int BUFFER_SIZE = 2048;
    /// <summary>
    /// True literal
    /// </summary>
    public const long TRUE  = 1L;
    /// <summary>
    /// False literal
    /// </summary>
    public const long FALSE = 0L;

    /// <summary>
    /// Initial VM state
    /// </summary>
    private readonly ImmutableArray<long> initialState;
    /// <summary>
    /// Allocation handle
    /// </summary>
    private readonly IntPtr handle;
    /// <summary>
    /// Buffer address
    /// </summary>
    private readonly long* buffer;
    /// <summary>
    /// Instruction pointer
    /// </summary>
    private long* ip;
    /// <summary>
    /// Relative base
    /// </summary>
    private long* relative;
    /// <summary>
    /// If the VM is disposed or not
    /// </summary>
    private bool isDisposed;
    /// <summary>
    /// VM buffer size
    /// </summary>
    private readonly int bufferSize;

    /// <summary>
    /// Gets the value at the given address in the VM's buffer
    /// </summary>
    /// <param name="index">Index offset within the buffer</param>
    /// <exception cref="ObjectDisposedException">If the VM has been disposed</exception>
    /// <exception cref="ArgumentOutOfRangeException">If the address is outside of the bounds of the VM's buffer</exception>
    public ref long this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ObjectDisposedException.ThrowIf(this.isDisposed, this);
#if DEBUG
            if (index < 0 || index >= this.bufferSize) throw new ArgumentOutOfRangeException(nameof(index), index, "Intcode VM buffer index out of range");
#endif

            return ref *(this.buffer + index);
        }
    }

    /// <summary>
    /// Gets the value at the given address in the VM's buffer
    /// </summary>
    /// <param name="index">Index offset within the buffer</param>
    /// <exception cref="ObjectDisposedException">If the VM has been disposed</exception>
    /// <exception cref="ArgumentOutOfRangeException">If the address is outside of the bounds of the VM's buffer</exception>
    public ref long this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ObjectDisposedException.ThrowIf(this.isDisposed, this);
            int offset = index.GetOffset(this.bufferSize);
#if DEBUG
            if (offset < 0 || offset >= this.bufferSize) throw new ArgumentOutOfRangeException(nameof(index), offset, "Intcode VM buffer index out of range");
#endif

            return ref *(this.buffer + offset);
        }
    }

    /// <summary>
    /// Current VM state
    /// </summary>
    public State Status { get; private set; }

    /// <summary>
    /// If this VM is in a halted state
    /// </summary>
    public bool IsHalted
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Status is State.HALTED;
    }

    /// <summary>
    /// VM's input provider
    /// </summary>
    public IInputProvider Input { get; set; }

    /// <summary>
    /// VM's output provider
    /// </summary>
    public IOutputProvider Output { get; set; }

    /// <summary>
    /// Creates a new VM from the specified Intcode source
    /// </summary>
    /// <param name="source">Intcode source</param>
    /// <param name="inputProvider">VM input provider</param>
    /// <param name="outputProvider">VM output provider</param>
    public IntcodeVM(ReadOnlySpan<char> source, IInputProvider? inputProvider = null, IOutputProvider? outputProvider = null)
    {
        // Initialize the input and output
        this.Input  = inputProvider  ?? new QueueInput();
        this.Output = outputProvider ?? new QueueOutput();

        // Get parsed code length
        int count          = source.Count(',') + 1;
        Span<Range> splits = stackalloc Range[count];
        int splitCount     = source.Split(splits, ',');

        // Create unmanaged buffer
        this.bufferSize = splitCount + BUFFER_SIZE;
        this.handle     = Marshal.AllocHGlobal(this.bufferSize * sizeof(long));
        this.buffer     = (long*)this.handle;
        this.ip         = this.buffer;
        this.relative   = this.buffer;

        // Populate buffer
        ImmutableArray<long>.Builder initialStateBuilder = ImmutableArray.CreateBuilder<long>(this.bufferSize);
        for (int i = 0; i < splitCount; i++, this.ip++)
        {
            long value = long.Parse(source[splits[i]]);
            initialStateBuilder.Add(value);
            *this.ip = value;
        }
        this.initialState = initialStateBuilder.ToImmutable();

        // Reset instruction pointer
        this.ip = this.buffer;
    }

    /// <summary>
    /// Clones an IntcodeVM from another one
    /// </summary>
    /// <param name="other">Other VM to clone from</param>
    public IntcodeVM(IntcodeVM other)
    {
        ObjectDisposedException.ThrowIf(other.isDisposed, other);

        // Copy fields
        this.initialState   = other.initialState;
        this.bufferSize     = other.bufferSize;
        this.Status         = other.Status;

        // Create clones of input/output providers
        this.Input  = other.Input.Clone();
        this.Output = other.Output.Clone();

        // Create buffer
        this.handle   = Marshal.AllocHGlobal(this.bufferSize * sizeof(long));
        this.buffer   = (long*)this.handle;
        this.initialState.CopyTo(new Span<long>(this.buffer, this.bufferSize));

        // Initialize pointers to correct address
        this.ip       = this.buffer + (other.ip - other.buffer);
        this.relative = this.buffer + (other.relative - other.buffer);
    }

    /// <summary>
    /// Finalizer
    /// </summary>
    ~IntcodeVM() => ReleaseUnmanagedResources();

    /// <summary>
    /// Runs the Intcode VM
    /// </summary>
    /// ReSharper disable once CognitiveComplexity
    public void Run(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, this);
        if (this.Status is State.HALTED or State.RUNNING) return;

        this.Status = State.RUNNING;
        while (true)
        {
            // Halt on cancellation
            if (cancellationToken.IsCancellationRequested)
            {
                this.Status = State.HALTED;
                return;
            }

            (int modes, Opcode opcode) = ReadOpcode();
            switch (opcode)
            {
                case Opcode.NOP:
                    // No operation
                    break;

                case Opcode.ADD:
                    Add(modes);
                    break;

                case Opcode.MUL:
                    Multiply(modes);
                    break;

                case Opcode.INP:
                    if (!TakeInput(modes)) return;
                    break;

                case Opcode.OUT:
                    PushOutput(modes);
                    break;

                case Opcode.JNZ:
                    JumpNotZero(modes);
                    break;

                case Opcode.JEZ:
                    JumpZero(modes);
                    break;

                case Opcode.TLT:
                    TestLessThan(modes);
                    break;

                case Opcode.TEQ:
                    TestEquals(modes);
                    break;

                case Opcode.REL:
                    RelativeSet(modes);
                    break;

                case Opcode.HLT:
                    Halt();
                    return;

                default:
                    throw new InvalidEnumArgumentException(nameof(opcode), (int)opcode, typeof(Opcode));
            }
        }
    }

    /// <summary>
    /// Resets the VM and primes it for running again
    /// </summary>
    public void Reset()
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, this);

        this.ip       = this.buffer;
        this.relative = this.buffer;
        this.Status   = State.READY;
        this.initialState.CopyTo(new Span<long>(this.buffer, this.bufferSize));
        this.Input.Clear();
        this.Output.Clear();
    }

    /// <summary>
    /// Reads the next <see cref="Opcode"/> in the VM's buffer
    /// </summary>
    /// <returns>The read <see cref="Opcode"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (int modes, Opcode opcode) ReadOpcode()
    {
        (long modes, long opcode) = Math.DivRem(*this.ip++, 100);
        return ((int)modes, (Opcode)opcode);
    }

    /// <summary>
    /// Reads the next <see cref="long"/> in the VM's buffer
    /// </summary>
    /// <returns>The read <see cref="long"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref long ReadNextInt64() => ref *this.ip++;

    /// <summary>
    /// Gets a reference to the operand in the specified mode
    /// </summary>
    /// <param name="mode">Mode to get the operand for</param>
    /// <returns>A reference to the operand value</returns>
    /// <exception cref="InvalidEnumArgumentException">For unknown values of <paramref name="mode"/></exception>
#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private ref long GetOperand(ParamMode mode)
    {
        // ReSharper disable once ConvertSwitchStatementToSwitchExpression
#if DEBUG
        switch (mode)
        {
            case ParamMode.POSITION:
                long* address = unchecked(this.buffer + ReadNextInt64());
                if (address < this.buffer || address > this.buffer + this.bufferSize) throw new AccessViolationException("Accessing memory not managed by the IntcodeVM");
                return ref *address;

            case ParamMode.IMMEDIATE:
                return ref ReadNextInt64();

            case ParamMode.RELATIVE:
                address = unchecked(this.relative + ReadNextInt64());
                if (address < this.buffer || address > this.buffer + this.bufferSize) throw new AccessViolationException("Accessing memory not managed by the IntcodeVM");
                return ref *address;

            default:
                throw new InvalidEnumArgumentException(nameof(mode), (int)mode, typeof(ParamMode));
        }
#else
        switch (mode)
        {
            case ParamMode.POSITION:
                return ref *(this.buffer + ReadNextInt64());

            case ParamMode.IMMEDIATE:
                return ref ReadNextInt64();

            case ParamMode.RELATIVE:
                return ref *(this.relative + ReadNextInt64());

            default:
                throw new InvalidEnumArgumentException(nameof(mode), (int)mode, typeof(ParamMode));
        }
#endif
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (this.isDisposed) return;

        // Release resources
        ReleaseUnmanagedResources();

        // Mark as disposed
        this.isDisposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged resources owned by the VM
    /// </summary>
    private void ReleaseUnmanagedResources()
    {
        // Free the memory
        Marshal.FreeHGlobal(this.handle);
        this.ip = null;
    }
}
