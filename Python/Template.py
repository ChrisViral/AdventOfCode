import sys
from typing import List


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # File read stub
    with open(args[1], "r") as f:
        for line in f:
            pass


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
