using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AdventOfCode.Intcode.Input;
using AdventOfCode.Intcode.Output;

namespace AdventOfCode.Intcode;

/// <summary>
/// Intcode VM
/// </summary>
public sealed unsafe partial class IntcodeVM : IDisposable
{
    /// <summary>
    /// VM state
    /// </summary>
    public enum State
    {
        READY,
        HALTED,
    }

    #region Constants
    /// <summary>
    /// VM Memory buffer size (8kb of long values)
    /// </summary>
    private const int BUFFER_SIZE = 1024;
    /// <summary>
    /// True literal
    /// </summary>
    private const long TRUE  = 1L;
    /// <summary>
    /// False literal
    /// </summary>
    private const long FALSE = 0L;
    #endregion

    #region Fields
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
    /// If the VM is disposed or not
    /// </summary>
    private bool isDisposed;
    /// <summary>
    /// VM buffer size
    /// </summary>
    private readonly int bufferSize;
    #endregion

    #region Indexers
    /// <summary>
    /// Gets the value at the given address in the VM's buffer
    /// </summary>
    /// <param name="index">Index offset within the buffer</param>
    /// <exception cref="ObjectDisposedException">If the VM has been disposed</exception>
    /// <exception cref="ArgumentOutOfRangeException">If the address is outside of the bounds of the VM's buffer</exception>
    public ref long this[int index]
    {
        get
        {
            ObjectDisposedException.ThrowIf(this.isDisposed, this);
            if (index < 0 || index >= this.bufferSize) throw new ArgumentOutOfRangeException(nameof(index), index, "Intcode VM buffer index out of range");

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
        get
        {
            ObjectDisposedException.ThrowIf(this.isDisposed, this);
            int offset = index.GetOffset(this.bufferSize);
            if (offset < 0 || offset >= this.bufferSize) throw new ArgumentOutOfRangeException(nameof(index), offset, "Intcode VM buffer index out of range");

            return ref *(this.buffer + offset);
        }
    }
    #endregion

    #region Properties
    /// <summary>
    /// Current VM state
    /// </summary>
    public State Status { get; private set; }

    /// <summary>
    /// VM's input provider
    /// </summary>
    public IInputProvider InputProvider { get; set; }

    /// <summary>
    /// VM's output provider
    /// </summary>
    public IOutputProvider OutputProvider { get; set; }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new VM from the specified Intcode source
    /// </summary>
    /// <param name="source">Intcode source</param>
    /// <param name="inputProvider">VM input provider</param>
    /// <param name="outputProvider">VM output provider</param>
    public IntcodeVM(ReadOnlySpan<char> source, IInputProvider? inputProvider = null, IOutputProvider? outputProvider = null)
    {
        // Initialize the input and output
        this.InputProvider  = inputProvider  ?? new QueueInput();
        this.OutputProvider = outputProvider ?? new QueueOutput();

        // Get parsed code length
        int count = source.Count(',') + 1;
        Span<Range> splits = stackalloc Range[count];
        int splitCount = source.Split(splits, ',');

        // Create unmanaged buffer
        this.bufferSize   = splitCount + BUFFER_SIZE;
        this.handle       = Marshal.AllocHGlobal(this.bufferSize * sizeof(long));
        this.buffer       = (long*)this.handle;
        this.ip           = this.buffer;

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
    /// Finalizer
    /// </summary>
    ~IntcodeVM() => ReleaseUnmanagedResources();
    #endregion

    #region Methods
    /// <summary>
    /// Runs the Intcode VM
    /// </summary>
    public void Run()
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, this);
        if (this.Status is State.HALTED) return;

        while (true)
        {
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

                case Opcode.IN:
                    Input(modes);
                    break;

                case Opcode.OUT:
                    Output(modes);
                    break;

                case Opcode.JNZ:
                    JumpNotZero(modes);
                    break;

                case Opcode.JZ:
                    JumpZero(modes);
                    break;

                case Opcode.TLT:
                    TestLessThan(modes);
                    break;

                case Opcode.TEQ:
                    TestEquals(modes);
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
        this.ip     = this.buffer;
        this.Status = State.READY;
        this.initialState.CopyTo(new Span<long>(this.buffer, this.bufferSize));
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref long GetOperand(ParamMode mode)
    {
        // ReSharper disable once ConvertSwitchStatementToSwitchExpression
        switch (mode)
        {
            case ParamMode.POSITION:
                return ref *(this.buffer + ReadNextInt64());

            case ParamMode.IMMEDIATE:
                return ref ReadNextInt64();

            default:
                throw new InvalidEnumArgumentException(nameof(mode), (int)mode, typeof(ParamMode));
        }
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
    #endregion
}