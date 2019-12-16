import sys
from typing import List
from utils import IntcodeComp


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # File read stub
    with open(args[1], "r") as f:
        # Create the computer from the program
        comp: IntcodeComp = IntcodeComp(f.readline().strip())

    # Run the first diagnostics
    comp.input_buffer.append(1)
    comp.run_program()

    # Run the second diagnostics
    comp.input_buffer.append(5)
    _, out = comp.run_program()

    # Print the output
    print("\n".join(map(str, out)))


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
