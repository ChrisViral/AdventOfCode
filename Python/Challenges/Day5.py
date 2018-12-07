import sys
from typing import List


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """
    # Get polymer chain
    polymer: str
    with open(args[1], "r") as f:
        polymer = f.readline()

    reactions: int = 0
    while react(polymer):
        reactions += 1

    print(f"Part one polymer length: {len(polymer)}\nProduct found after {reactions} reactions")


def react(polymer: str) -> bool:
    # Can only react if there is 2 or more components
    if len(polymer) < 2:
        return False

    prev: chr = polymer[0]
    return False


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
