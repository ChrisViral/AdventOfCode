import sys
from typing import List
from enum import IntEnum


class Opcode(IntEnum):
    """
    Existing opcodes
    """
    ADD = 1
    MUL = 2
    HALT = 99


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # File read stub
    with open(args[1], "r") as f:
        for line in f:
            # Get program from file
            program: List[int] = list(map(int, line.split(",")))

    # Run the original problem
    print(run_intcode(program, 12, 2))

    # Value to find
    value: int = 19690720
    # Loop through all possible nouns and verbs
    for noun in range(100):
        for verb in range(100):
            # Run the program and compare
            if run_intcode(program, noun, verb) == value:
                # If it matches, print the hash and we are done
                print((100 * noun) + verb)
                return


def run_intcode(program: List[int], noun: int, verb: int) -> int:
    """
    Runs an intcode program
    :param program: Program to run
    :param noun: First parameter of the program
    :param verb: Second parameter of the program
    :return: The result of the program
    """

    # Make sure we make a copy of the program first, then set the verb and noun
    program = program.copy()
    program[1] = noun
    program[2] = verb

    # Set the Instruction Pointer and the starting Opcode
    ip: int = 0
    opcode: int = program[ip]

    # Run forever
    while opcode != Opcode.HALT:
        # Get the a register value, b register value, and c register value
        a = program[ip + 1]
        b = program[ip + 2]
        c = program[ip + 3]

        # Handle opcodes
        if opcode == Opcode.ADD:
            program[c] = program[a] + program[b]
        elif opcode == Opcode.MUL:
            program[c] = program[a] * program[b]
        # If opcode not handled, throw
        else:
            raise Exception("Opcode Invalid")

        # Increment IP and set new opcode
        ip += 4
        opcode = program[ip]

    # Return output of the program
    return program[0]


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
