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
        comp: IntcodeComp = IntcodeComp(f.readline().strip(), uses_modes=False)

    # Run the original problem
    print(comp.run_program(12, 2)[0][0])

    # Value to find
    value: int = 19690720
    # Loop through all possible nouns and verbs
    for noun in range(100):
        for verb in range(100):
            # Run the program and compare
            if comp.run_program(noun, verb)[0][0] == value:
                # If it matches, print the hash and we are done
                print((100 * noun) + verb)
                return


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
