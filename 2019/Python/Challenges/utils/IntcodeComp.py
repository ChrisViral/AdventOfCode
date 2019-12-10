from __future__ import annotations
from typing import Tuple, List, Deque, Dict, Callable, Optional, Final
from collections import deque
from enum import Enum, IntFlag
from time import sleep


class ParamMode(IntFlag):
    """
    Input mode flags for the IntcodeComp
    """

    # region Enum members
    NONE   = 0b000
    FIRST  = 0b001
    SECOND = 0b010
    THIRD  = 0b100
    # endregion

    # region Static methods
    def has_flag(self, flag: ParamMode) -> bool:
        """
        Returns if the given modes value has the specified flag
        :param flag: Flag to test for
        :return: True if the value has the flag, false otherwise
        """

        return self & flag == flag
    # endregion


class Opcode(Enum):
    """
    Opcodes for the IntcodeComp
    """

    # region Enum members
    ADD = 1
    MUL = 2
    INP = 3
    OUT = 4
    JIT = 5
    JIF = 6
    TLT = 7
    TEQ = 8
    HLT = 99
    # endregion


class IntcodeComp:
    """
    Intcode Computer VM
    """

    # region Static methods
    @staticmethod
    def _get_digits(value: int, start: int, end: Optional[int] = None) -> int:
        """
        Returns the value of the specified digits of the given number
        :param value: Value to extract digits from
        :param start: Starting index of the digits to extract
        :param end: Ending index of the digits to extract, defaults to the end of the string
        """

        return int(f"{value:06}"[start:end])

    @staticmethod
    def __get_param(prog: List[int], ip: int, modes: ParamMode, param: ParamMode) -> int:
        """
        Gets the parameter in either immediate mode or address mode
        :param prog: Intcode program to get the parameter from
        :param ip: Current instruction pointer
        :param modes: Current input modes for this Opcode
        :param param: Current parameter of the instruction
        :return: The value of the parameter, accounting for the correct input mode
        """

        x: int = prog[ip + param.bit_length()]
        return x if modes.has_flag(param) else prog[x]

    @staticmethod
    def _add(prog: List[int], ip: int, comp: IntcodeComp, modes: ParamMode = ParamMode.NONE) -> int:
        """
        Add Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """

        a = IntcodeComp.__get_param(prog, ip, modes, ParamMode.FIRST)
        b = IntcodeComp.__get_param(prog, ip, modes, ParamMode.SECOND)

        prog[prog[ip + 3]] = a + b
        return ip + 4

    @staticmethod
    def _mul(prog: List[int], ip: int, comp: IntcodeComp,  modes: ParamMode = ParamMode.NONE) -> int:
        """
        Multiply Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = IntcodeComp.__get_param(prog, ip, modes, ParamMode.FIRST)
        b = IntcodeComp.__get_param(prog, ip, modes, ParamMode.SECOND)

        prog[prog[ip + 3]] = a * b
        return ip + 4

    @staticmethod
    def _inp(prog: List[int], ip: int, comp: IntcodeComp,  modes: ParamMode = ParamMode.NONE) -> int:
        """
        Input Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        # If nothing is in the input buffer, busy wait until something is produced
        while len(comp._input_buffer) == 0:
            sleep(1E-3)  # 1ms

        # Pop the input buffer
        prog[prog[ip + 1]] = comp._input_buffer.popleft()
        return ip + 2

    @staticmethod
    def _out(prog: List[int], ip: int, comp: IntcodeComp,  modes: ParamMode = ParamMode.NONE) -> int:
        """
        Output Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = IntcodeComp.__get_param(prog, ip, modes, ParamMode.FIRST)

        # Add to the output buffer
        comp._output_buffer.append(a)
        return ip + 2

    @staticmethod
    def _jit(prog: List[int], ip: int, comp: IntcodeComp,  modes: ParamMode = ParamMode.NONE) -> int:
        """
        Jump-If-True Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = IntcodeComp.__get_param(prog, ip, modes, ParamMode.FIRST)
        b = IntcodeComp.__get_param(prog, ip, modes, ParamMode.SECOND)

        return b if a != 0 else ip + 3

    @staticmethod
    def _jif(prog: List[int], ip: int, comp: IntcodeComp,  modes: ParamMode = ParamMode.NONE) -> int:
        """
        Jump-If-False Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = IntcodeComp.__get_param(prog, ip, modes, ParamMode.FIRST)
        b = IntcodeComp.__get_param(prog, ip, modes, ParamMode.SECOND)

        return b if a == 0 else ip + 3

    @staticmethod
    def _tlt(prog: List[int], ip: int, comp: IntcodeComp,  modes: ParamMode = ParamMode.NONE) -> int:
        """
        Test-Less-Than Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = IntcodeComp.__get_param(prog, ip, modes, ParamMode.FIRST)
        b = IntcodeComp.__get_param(prog, ip, modes, ParamMode.SECOND)

        prog[prog[ip + 3]] = 1 if a < b else 0
        return ip + 4

    @staticmethod
    def _teq(prog: List[int], ip: int, comp: IntcodeComp,  modes: ParamMode = ParamMode.NONE) -> int:
        """
        Test-Equals Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = IntcodeComp.__get_param(prog, ip, modes, ParamMode.FIRST)
        b = IntcodeComp.__get_param(prog, ip, modes, ParamMode.SECOND)

        prog[prog[ip + 3]] = 1 if a == b else 0
        return ip + 4
    # endregion

    # region Properties
    @property
    def input_buffer(self) -> Deque[int]:
        """
        This Intcode Computer's input buffer
        :return: The input buffer
        """
        return self._input_buffer

    @input_buffer.setter
    def input_buffer(self, value: Deque[int]) -> None:
        """
        Sets this Intcode Computer's input buffer
        :param value: New input buffer
        """
        self._input_buffer = value

    @property
    def output_buffer(self) -> Deque[int]:
        """
        This Intcode Computer's output buffer
        :return: The output buffer
        """
        return self._output_buffer

    @output_buffer.setter
    def output_buffer(self, value: Deque[int]) -> None:
        """
        Sets this Intcode Computer's output buffer
        :param value: New output buffer
        """
        self._output_buffer = value
    # endregion

    # region Constructor
    def __init__(self, code: str, modes: bool = True) -> None:
        """
        Creates a new IntcodeComputer for the given program
        :param code: Code that the computer has to run
        :param modes: If input modes are active for this computer
        """

        # Setup the code into the list it needs to be
        self._program: List[int] = list(map(int, code.split(",")))
        self._modes: bool = modes
        self._input_buffer: Deque[int] = deque()
        self._output_buffer: Deque[int] = deque()
    # endregion

    # region Methods
    def run_program(self, noun: Optional[int] = None, verb: Optional[int] = None) -> Tuple[List[int], Deque[int]]:
        """
        Runs the Intcode program associated to this computer, with the given noun and verb
        :param noun: First parameter of the program
        :param verb: Second parameter of the program
        :return: A tuple containing the final state of the program's memory, as well as the final output buffer
        """

        # Make sure we make a copy of the program first, then set the verb and noun
        prog: List[int] = self._program.copy()
        if noun is not None:
            prog[1] = noun
            if verb is not None:
                prog[2] = verb

        if self._modes:
            return self._run_program_modes(prog)

        # Set the Instruction Pointer and the starting Opcode
        ip: int = 0
        opcode: Opcode = Opcode(prog[ip])

        # Run forever
        while opcode is not Opcode.HLT:
            # Get the a register value, b register value, and c register value
            try:
                ip = _operations[opcode](prog, ip, self)
            except KeyError:
                raise ValueError("Invalid Opcode detected")

            opcode = Opcode(prog[ip])

        # Return output of the program
        return prog, self._output_buffer

    def _run_program_modes(self, prog: List[int]) -> Tuple[List[int], Deque[int]]:
        """
        Runs the given Intcode program with input modes activated
        :param prog: Program code
        :return: A tuple containing the final state of the program's memory, as well as the final output buffer
        """

        # Set the Instruction Pointer and the starting Opcode
        ip: int = 0
        opcode: Opcode = Opcode(IntcodeComp._get_digits(prog[0], -2))
        modes: ParamMode = ParamMode(IntcodeComp._get_digits(prog[0], 0, -2))

        # Run forever
        while opcode is not Opcode.HLT:
            # Get the a register value, b register value, and c register value
            try:
                ip = _operations[opcode](prog, ip, self, modes)
            except KeyError:
                raise ValueError("Invalid Opcode detected")

            opcode = Opcode(IntcodeComp._get_digits(prog[ip], -2))
            modes = ParamMode(IntcodeComp._get_digits(prog[ip], 0, -2))

        # Return output of the program
        return prog, self._output_buffer
    # endregion


# Operation function signature
Operation = Callable[[List[int], int, IntcodeComp, ParamMode], int]
# Opcode/Operations map
_operations: Final[Dict[Opcode, Operation]] = {
    Opcode.ADD: IntcodeComp._add,
    Opcode.MUL: IntcodeComp._mul,
    Opcode.INP: IntcodeComp._inp,
    Opcode.OUT: IntcodeComp._out,
    Opcode.JIT: IntcodeComp._jit,
    Opcode.JIF: IntcodeComp._jif,
    Opcode.TLT: IntcodeComp._tlt,
    Opcode.TEQ: IntcodeComp._teq,
    Opcode.HLT: lambda _, ip, __, ___: ip
}
