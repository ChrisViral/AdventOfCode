import sys
import re
from itertools import chain


def main(args):
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Create fabric tile
    width = 1000
    fabric = [[0] * width for _ in range(width)]
    # Regex match for input
    splitter = re.compile(r"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)")

    # Read file
    with open(args[1], "r") as f:
        for line in f:
            # Parse input
            req, x, y, w, h = map(int, splitter.search(line).groups())

            # Increment use count on the fabric for this request
            for i in range(x, x + w):
                for j in range(y, y + h):
                    fabric[i][j] += 1

    # Get amount of squares with more than one request access
    count = sum(1 for i in chain.from_iterable(fabric) if i > 1)
    print(f"Part one count: {count}")


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
