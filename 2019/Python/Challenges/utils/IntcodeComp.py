from __future__ import annotations
from typing import Tuple, List, Deque, Dict, Callable, Optional, Final
from collections import deque
from enum import Enum, IntEnum
from time import sleep


class Parameter(IntEnum):
    """
    Input mode flags for the IntcodeComp
    """

    # region Enum members
    FIRST  = 1
    SECOND = 2
    THIRD = 3
    # endregion


class ParamMode(Enum):
    """
    Intcode Parameter Modes
    """

    # region Enum members
    ABSOLUTE = 0
    IMMEDIATE = 1
    RELATIVE = 2
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
    REF = 9
    HLT = 99
    # endregion


class IntcodeComp:
    """
    Intcode Computer VM
    """

    # region Static methods
    @staticmethod
    def _decode_instruction(instruction: int) -> Tuple[Opcode, str]:
        """
        Turns an intcode instruction into it's corresponding Opcode and parameter modes string
        :param instruction: Intcode instruction to decode
        """

        instruction = f"{instruction:05}"
        return Opcode(int(instruction[-2:])), instruction[0:-2]

    @staticmethod
    def _get_param(prog: List[int], ip: int, modes: str, param: Parameter, ref: int) -> int:
        """
        Gets the parameter in either immediate mode, relative mode, or address mode
        :param prog: Intcode program to get the parameter from
        :param ip: Current instruction pointer
        :param modes: Current parameter modes for this Opcode
        :param param: Current parameter of the instruction
        :param ref: Reference base for relative instructions
        :return: The value of the parameter, accounting for the correct input mode
        """

        # Instruction parameter
        x: int = prog[ip + param]
        # Default to absolute if parameter modes are not provided
        if modes is None:
            return prog[x]
        # Use the correct parameter mode
        mode: ParamMode = ParamMode(int(modes[-param]))
        if mode == mode.ABSOLUTE:
            return prog[x]
        elif mode == mode.IMMEDIATE:
            return x
        elif mode == mode.RELATIVE:
            return prog[x + ref]
        else:
            raise ValueError("Invalid ParameterMode detected")

    @staticmethod
    def _get_register(prog: List[int], ip: int, modes: str, param: Parameter, ref: int) -> int:
        """
                Gets the register address in either relative mode or address mode
                :param prog: Intcode program to get the parameter from
                :param ip: Current instruction pointer
                :param modes: Current parameter modes for this Opcode
                :param param: Current parameter of the instruction
                :param ref: Reference base for relative instructions
                :return: The value of the parameter, accounting for the correct input mode
                """

        # Instruction parameter
        x: int = prog[ip + param]
        # Default to absolute if parameter modes are not provided
        if modes is None:
            return x
        # Use the correct parameter mode
        mode: ParamMode = ParamMode(int(modes[-param]))
        if mode == mode.ABSOLUTE:
            return x
        elif mode == mode.IMMEDIATE:
            raise ValueError("Immediate mode not supported for writes")
        elif mode == mode.RELATIVE:
            return x + ref
        else:
            raise ValueError("Invalid ParameterMode detected")

    @staticmethod
    def _add(prog: List[int], ip: int, comp: IntcodeComp, modes: Optional[str] = None) -> int:
        """
        Add Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param comp: IntcodeComp this operation is executed from
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """

        a = IntcodeComp._get_param(prog, ip, modes, Parameter.FIRST, comp._reference)
        b = IntcodeComp._get_param(prog, ip, modes, Parameter.SECOND, comp._reference)
        c = IntcodeComp._get_register(prog, ip, modes, Parameter.THIRD, comp._reference)

        prog[c] = a + b
        return ip + 4

    @staticmethod
    def _mul(prog: List[int], ip: int, comp: IntcodeComp, modes: Optional[str] = None) -> int:
        """
        Multiply Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param comp: IntcodeComp this operation is executed from
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = IntcodeComp._get_param(prog, ip, modes, Parameter.FIRST, comp._reference)
        b = IntcodeComp._get_param(prog, ip, modes, Parameter.SECOND, comp._reference)
        c = IntcodeComp._get_register(prog, ip, modes, Parameter.THIRD, comp._reference)

        prog[c] = a * b
        return ip + 4

    @staticmethod
    def _inp(prog: List[int], ip: int, comp: IntcodeComp, modes: Optional[str] = None) -> int:
        """
        Input Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param comp: IntcodeComp this operation is executed from
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        if len(comp._input_buffer) == 0:
            # If nothing is in the input buffer and in a threaded environment, busy wait until something is produced
            if comp._threaded:
                while len(comp._input_buffer) == 0:
                    sleep(1E-3)  # 1ms
            # Else throw
            else:
                raise ValueError("Input buffer empty")

        # Pop the input buffer
        a = IntcodeComp._get_register(prog, ip, modes, Parameter.FIRST, comp._reference)
        inp = comp._input_buffer.popleft()

        prog[a] = inp
        return ip + 2

    @staticmethod
    def _out(prog: List[int], ip: int, comp: IntcodeComp, modes: Optional[str] = None) -> int:
        """
        Output Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param comp: IntcodeComp this operation is executed from
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = IntcodeComp._get_param(prog, ip, modes, Parameter.FIRST, comp._reference)

        # Add to the output buffer
        comp._output_buffer.append(a)
        return ip + 2

    @staticmethod
    def _jit(prog: List[int], ip: int, comp: IntcodeComp, modes: Optional[str] = None) -> int:
        """
        Jump-If-True Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param comp: IntcodeComp this operation is executed from
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = IntcodeComp._get_param(prog, ip, modes, Parameter.FIRST, comp._reference)
        b = IntcodeComp._get_param(prog, ip, modes, Parameter.SECOND, comp._reference)

        return b if a != 0 else ip + 3

    @staticmethod
    def _jif(prog: List[int], ip: int, comp: IntcodeComp, modes: Optional[str] = None) -> int:
        """
        Jump-If-False Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param comp: IntcodeComp this operation is executed from
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = IntcodeComp._get_param(prog, ip, modes, Parameter.FIRST, comp._reference)
        b = IntcodeComp._get_param(prog, ip, modes, Parameter.SECOND, comp._reference)

        return b if a == 0 else ip + 3

    @staticmethod
    def _tlt(prog: List[int], ip: int, comp: IntcodeComp, modes: Optional[str] = None) -> int:
        """
        Test-Less-Than Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param comp: IntcodeComp this operation is executed from
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = IntcodeComp._get_param(prog, ip, modes, Parameter.FIRST, comp._reference)
        b = IntcodeComp._get_param(prog, ip, modes, Parameter.SECOND, comp._reference)
        c = IntcodeComp._get_register(prog, ip, modes, Parameter.THIRD, comp._reference)

        prog[c] = 1 if a < b else 0
        return ip + 4

    @staticmethod
    def _teq(prog: List[int], ip: int, comp: IntcodeComp, modes: Optional[str] = None) -> int:
        """
        Test-Equals Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param comp: IntcodeComp this operation is executed from
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = IntcodeComp._get_param(prog, ip, modes, Parameter.FIRST, comp._reference)
        b = IntcodeComp._get_param(prog, ip, modes, Parameter.SECOND, comp._reference)
        c = IntcodeComp._get_register(prog, ip, modes, Parameter.THIRD, comp._reference)

        prog[c] = 1 if a == b else 0
        return ip + 4

    @staticmethod
    def _ref(prog: List[int], ip: int, comp: IntcodeComp, modes: Optional[str] = None) -> int:
        """
        Reference-Adjust Opcode operation
        :param prog: Program memory
        :param ip: Current instruction pointer
        :param comp: IntcodeComp this operation is executed from
        :param modes: Input modes
        :return: The new instruction pointer after execution
        """
        a = IntcodeComp._get_param(prog, ip, modes, Parameter.FIRST, comp._reference)
        comp._reference += a

        return ip + 2
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
    def __init__(self, code: str, modes: bool = True, threaded: bool = False) -> None:
        """
        Creates a new IntcodeComputer for the given program
        :param code: Code that the computer has to run
        :param modes: If input modes are active for this computer
        """

        # Setup the code into the list it needs to be
        self._program: List[int] = list(map(int, code.split(","))) + ([0] * 2000)  # Additional memory
        self._modes: bool = modes
        self._threaded: bool = threaded
        self._input_buffer: Deque[int] = deque()
        self._output_buffer: Deque[int] = deque()
        self._reference: int = 0
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
        self._reference = 0
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
        opcode: Opcode
        modes: str
        opcode, modes = IntcodeComp._decode_instruction(prog[ip])

        # Run forever
        while opcode is not Opcode.HLT:
            # Get the a register value, b register value, and c register value
            try:
                ip = _operations[opcode](prog, ip, self, modes)
            except KeyError:
                raise ValueError("Invalid Opcode detected")

            opcode, modes = IntcodeComp._decode_instruction(prog[ip])

        # Return output of the program
        return prog, self._output_buffer
    # endregion


# Operation function signature
Operation = Callable[[List[int], int, IntcodeComp, Optional[str]], int]
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
    Opcode.REF: IntcodeComp._ref,
    Opcode.HLT: lambda _, ip, __, ___: ip
}
