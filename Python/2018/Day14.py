import sys
import re as regex
from typing import List, Optional, Pattern, Match


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Get input number
    with open(args[1], "r") as f:
        value: str = f.readline().strip()

    # Setup data
    number: int = int(value)
    size: int = len(value) + 1
    pattern: Pattern = regex.compile(value + ".?$")
    recipes: str = "37"
    first: int = 0
    second: int = 1

    # While a match has not been found
    part1: bool = False
    m: Optional[Match] = None
    while not m:
        # Get new recipe
        a = int(recipes[first])
        b = int(recipes[second])
        new = a + b
        for c in str(new):
            recipes += c

        # Check for part 1
        if not part1 and len(recipes) >= number + 10:
            print("Part one score:", recipes[number:number + 10])
            part1 = True

        # Calculate next indices
        length = len(recipes)
        first = (first + a + 1) % length
        second = (second + b + 1) % length
        # Look for match
        m = pattern.search(recipes, length - size)

    # Print final result
    print("Part two count:", m.start())


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
