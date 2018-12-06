import sys
from typing import List


def main(args: List[str]):
    """
    Application entry point
    :param args: Argument list, should contain the file to load
    """

    # File read stub
    with open(args[0], "r") as f:
        for line in f:
            pass

# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)