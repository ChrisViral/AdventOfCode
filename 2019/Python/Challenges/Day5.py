import sys
from typing import List
from Utils import IntcodeComp


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # File read stub
    with open(args[1], "r") as f:
        # Simply run the program and input the number needed for the required solution
        IntcodeComp(f.readline(), True).run_program()


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
