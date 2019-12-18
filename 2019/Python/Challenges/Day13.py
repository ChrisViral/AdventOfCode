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
        comp: IntcodeComp = IntcodeComp(f.readline().strip())

    blocks: int = 0
    comp.run_program()
    while len(comp.output_buffer) >= 3:
        comp.next_output()
        comp.next_output()
        blocks += 1 if comp.next_output() == 2 else 0

    print(blocks)


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
