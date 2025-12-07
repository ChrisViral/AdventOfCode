using System;

namespace AdventOfCode.Intcode;

public partial class IntcodeVM
{
    /// <summary>
    /// Intcode parameter mode
    /// </summary>
    private enum ParamMode : byte
    {
        /// <summary>
        /// Position offset mode
        /// </summary>
        POSITION  = 0,
        /// <summary>
        /// Immediate (literal) value mode
        /// </summary>
        IMMEDIATE = 1,
        /// <summary>
        /// Relative offset mode
        /// </summary>
        RELATIVE = 2
    }

    /// <summary>
    /// Operand Modes
    /// </summary>
    private readonly ref struct Modes
    {
        /// <summary>
        /// First parameter mode
        /// </summary>
        public readonly ParamMode first;
        /// <summary>
        /// Second parameter mode
        /// </summary>
        public readonly ParamMode second;
        /// <summary>
        /// Third parameter mode
        /// </summary>
        public readonly ParamMode third;

        /// <summary>
        /// Creates a new set of Modes from the given values
        /// </summary>
        /// <param name="first">First parameter mode</param>
        private Modes(int first) => this.first = (ParamMode)first;

        /// <summary>
        /// Creates a new set of Modes from the given values
        /// </summary>
        /// <param name="first">First parameter mode</param>
        /// <param name="second">Second parameter mode</param>
        private Modes(int first, int second)
        {
            this.first  = (ParamMode)first;
            this.second = (ParamMode)second;
        }

        /// <summary>
        /// Creates a new set of Modes from the given values
        /// </summary>
        /// <param name="first">First parameter mode</param>
        /// <param name="second">Second parameter mode</param>
        /// <param name="third">Third parameter mode</param>
        private Modes(int first, int second, int third)
        {
            this.first  = (ParamMode)first;
            this.second = (ParamMode)second;
            this.third  = (ParamMode)third;
        }

        /// <summary>
        /// Creates modes for a single operand opcode
        /// </summary>
        /// <param name="modes">Modes value</param>
        /// <returns>Resulting modes struct</returns>
        public static Modes OneOperand(int modes) => new(modes);

        /// <summary>
        /// Creates modes for a two operand opcode
        /// </summary>
        /// <param name="modes">Modes value</param>
        /// <returns>Resulting modes struct</returns>
        public static Modes TwoOperands(int modes)
        {
            (int second, int first) = Math.DivRem(modes, 10);
            return new Modes(first, second);
        }

        /// <summary>
        /// Creates modes for a three operand opcode
        /// </summary>
        /// <param name="modes">Modes value</param>
        /// <returns>Resulting modes struct</returns>
        public static Modes ThreeOperands(int modes)
        {
            (modes, int first)      = Math.DivRem(modes, 10);
            (int third, int second) = Math.DivRem(modes, 10);
            return new Modes(first, second, third);
        }
    }
}
