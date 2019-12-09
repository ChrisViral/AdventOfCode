from __future__ import annotations
from typing import List, Dict, Callable, Optional
from enum import IntEnum, IntFlag


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


class Opcode(IntEnum):
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

    # region Operations
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
    def _add(prog: List[int], ip: int, modes: ParamMode = ParamMode.NONE) -> int:
        """
        Add Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """

        a = Opcode.__get_param(prog, ip, modes, ParamMode.FIRST)
        b = Opcode.__get_param(prog, ip, modes, ParamMode.SECOND)

        prog[prog[ip + 3]] = a + b
        return ip + 4

    @staticmethod
    def _mul(prog: List[int], ip: int, modes: ParamMode = ParamMode.NONE) -> int:
        """
        Multiply Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = Opcode.__get_param(prog, ip, modes, ParamMode.FIRST)
        b = Opcode.__get_param(prog, ip, modes, ParamMode.SECOND)

        prog[prog[ip + 3]] = a * b
        return ip + 4

    @staticmethod
    def _inp(prog: List[int], ip: int, modes: ParamMode = ParamMode.NONE) -> int:
        """
        Input Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        prog[prog[ip + 1]] = int(input("Intcode Input: "))
        return ip + 2

    @staticmethod
    def _out(prog: List[int], ip: int, modes: ParamMode = ParamMode.NONE) -> int:
        """
        Output Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = Opcode.__get_param(prog, ip, modes, ParamMode.FIRST)

        print(a)
        return ip + 2

    @staticmethod
    def _jit(prog: List[int], ip: int, modes: ParamMode = ParamMode.NONE) -> int:
        """
        Jump-If-True Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = Opcode.__get_param(prog, ip, modes, ParamMode.FIRST)
        b = Opcode.__get_param(prog, ip, modes, ParamMode.SECOND)

        return b if a != 0 else ip + 3

    @staticmethod
    def _jif(prog: List[int], ip: int, modes: ParamMode = ParamMode.NONE) -> int:
        """
        Jump-If-False Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = Opcode.__get_param(prog, ip, modes, ParamMode.FIRST)
        b = Opcode.__get_param(prog, ip, modes, ParamMode.SECOND)

        return b if a == 0 else ip + 3

    @staticmethod
    def _tlt(prog: List[int], ip: int, modes: ParamMode = ParamMode.NONE) -> int:
        """
        Test-Less-Than Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = Opcode.__get_param(prog, ip, modes, ParamMode.FIRST)
        b = Opcode.__get_param(prog, ip, modes, ParamMode.SECOND)

        prog[prog[ip + 3]] = 1 if a < b else 0
        return ip + 4

    @staticmethod
    def _teq(prog: List[int], ip: int, modes: ParamMode = ParamMode.NONE) -> int:
        """
        Test-Equals Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = Opcode.__get_param(prog, ip, modes, ParamMode.FIRST)
        b = Opcode.__get_param(prog, ip, modes, ParamMode.SECOND)

        prog[prog[ip + 3]] = 1 if a == b else 0
        return ip + 4
    # endregion


class IntcodeComp:
    """
    Intcode Computer VM
    """

    # region Static fields
    # Opcodes/Operations map
    _operations: Dict[Opcode, Callable[[List[int], int, ParamMode], int]] = {
        Opcode.ADD: Opcode._add,
        Opcode.MUL: Opcode._mul,
        Opcode.INP: Opcode._inp,
        Opcode.OUT: Opcode._out,
        Opcode.JIT: Opcode._jit,
        Opcode.JIF: Opcode._jif,
        Opcode.TLT: Opcode._tlt,
        Opcode.TEQ: Opcode._teq,
        Opcode.HLT: lambda prog, ip: ip
    }
    # endregion

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
    def _run_program_modes(prog: List[int], out: bool) -> Optional[int]:
        """
        Runs the given Intcode program with input modes activated
        :param prog: Program code
        :param out: If the function should output memory location 0 after running or not
        :return: The result of the program
        """

        # Set the Instruction Pointer and the starting Opcode
        ip: int = 0
        opcode: Opcode = Opcode(IntcodeComp._get_digits(prog[0], -2))
        modes: ParamMode = ParamMode(IntcodeComp._get_digits(prog[0], 0, -2))

        # Run forever
        while opcode != Opcode.HLT:
            # Get the a register value, b register value, and c register value
            try:
                ip = IntcodeComp._operations[opcode](prog, ip, modes)
            except KeyError:
                raise ValueError("Invalid Opcode detected")

            opcode = Opcode(IntcodeComp._get_digits(prog[ip], -2))
            modes = ParamMode(IntcodeComp._get_digits(prog[ip], 0, -2))

        # Return output of the program
        if out:
            return prog[0]
    # endregion

    # region Constructor
    def __init__(self, code: str, modes: bool = False) -> None:
        """
        Creates a new IntcodeComputer for the given program
        :param code: Code that the computer has to run
        :param modes: If input modes are active for this computer
        """

        # Setup the code into the list it needs to be
        self._program: List[int] = list(map(int, code.split(",")))
        self._modes = modes
    # endregion

    # region Methods
    def run_program(self, noun: Optional[int] = None, verb: Optional[int] = None, out: bool = False) -> Optional[int]:
        """
        Runs the Intcode program associated to this computer, with the given noun and verb
        :param noun: First parameter of the program
        :param verb: Second parameter of the program
        :param out: If the function should output memory location 0 after running or not
        :return: The result of the program
        """

        # Make sure we make a copy of the program first, then set the verb and noun
        prog: List[int] = self._program.copy()
        if noun is not None:
            prog[1] = noun
            if verb is not None:
                prog[2] = verb

        if self._modes:
            return IntcodeComp._run_program_modes(prog, out)

        # Set the Instruction Pointer and the starting Opcode
        ip: int = 0
        opcode: Opcode = Opcode(prog[ip])

        # Run forever
        while opcode != Opcode.HLT:
            # Get the a register value, b register value, and c register value
            try:
                ip = IntcodeComp._operations[opcode](prog, ip)
            except KeyError:
                raise ValueError("Invalid Opcode detected")

            opcode = Opcode(prog[ip])

        # Return output of the program
        if out:
            return prog[0]
    # endregion
