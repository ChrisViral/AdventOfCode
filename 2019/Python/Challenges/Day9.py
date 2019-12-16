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
        # Get IntcodeComp
        comp: IntcodeComp = IntcodeComp(f.readline().strip())

    # Run diagnostics mode
    comp.input_buffer.append(1)
    comp.run_program()
    print(comp.output_buffer.pop())

    # Run Sensor program
    comp.input_buffer.append(2)
    comp.run_program()
    print(comp.output_buffer.pop())


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
