using System.Runtime.CompilerServices;

namespace AdventOfCode.Intcode;

public partial class IntcodeVM
{
    #region Methods
    /// <summary>
    /// Adds values
    /// </summary>
    /// <param name="modesValue">Operand modes value</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Add(int modesValue)
    {
        Modes modes = Modes.ThreeOperands(modesValue);
        ref long a = ref GetOperand(modes.first);
        ref long b = ref GetOperand(modes.second);
        ref long c = ref GetOperand(modes.third);
        c = a + b;
    }

    /// <summary>
    /// Multiplies values
    /// </summary>
    /// <param name="modesValue">Operand modes value</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Multiply(int modesValue)
    {
        Modes modes = Modes.ThreeOperands(modesValue);
        ref long a = ref GetOperand(modes.first);
        ref long b = ref GetOperand(modes.second);
        ref long c = ref GetOperand(modes.third);
        c = a * b;
    }

    /// <summary>
    /// Fetches and stores an input value
    /// </summary>
    /// <param name="modesValue">Operand modes value</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Input(int modesValue)
    {
        Modes modes = Modes.OneOperand(modesValue);
        ref long destination = ref GetOperand(modes.first);
        destination = this.InputProvider.Input();
    }

    /// <summary>
    /// Outputs a value
    /// </summary>
    /// <param name="modesValue">Operand modes value</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Output(int modesValue)
    {
        Modes modes = Modes.OneOperand(modesValue);
        this.OutputProvider.Output(GetOperand(modes.first));
    }

    /// <summary>
    /// Jumps if not zero
    /// </summary>
    /// <param name="modesValue">Operand modes value</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void JumpNotZero(int modesValue)
    {
        Modes modes = Modes.TwoOperands(modesValue);
        if (GetOperand(modes.first) is not 0L)
        {
            this.ip = this.buffer + GetOperand(modes.second);
        }
        else
        {
            this.ip++;
        }
    }
    
    /// <summary>
    /// Jumps if zero
    /// </summary>
    /// <param name="modesValue">Operand modes value</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void JumpZero(int modesValue)
    {
        Modes modes = Modes.TwoOperands(modesValue);
        if (GetOperand(modes.first) is 0L)
        {
            this.ip = this.buffer + GetOperand(modes.second);
        }
        else
        {
            this.ip++;
        }
    }

    /// <summary>
    /// Test less than
    /// </summary>
    /// <param name="modesValue">Operand modes value</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void TestLessThan(int modesValue)
    {
        Modes modes = Modes.ThreeOperands(modesValue);
        ref long a = ref GetOperand(modes.first);
        ref long b = ref GetOperand(modes.second);
        ref long c = ref GetOperand(modes.third);
        c = a < b ? TRUE : FALSE;
    }

    /// <summary>
    /// Test equals
    /// </summary>
    /// <param name="modesValue">Operand modes value</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void TestEquals(int modesValue)
    {
        Modes modes = Modes.ThreeOperands(modesValue);
        ref long a = ref GetOperand(modes.first);
        ref long b = ref GetOperand(modes.second);
        ref long c = ref GetOperand(modes.third);
        c = a == b ? TRUE : FALSE;
    }

    /// <summary>
    /// Halts execution
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Halt() => this.Status = State.HALTED;
    #endregion
}