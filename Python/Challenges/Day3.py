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
    claims = {}

    # Read file
    with open(args[1], "r") as f:
        for line in f:
            # Parse input
            req, x, y, w, h = map(int, splitter.search(line).groups())
            claims[req] = (x, y, w, h)

            # Increment use count on the fabric for this request
            for i in range(x, x + w):
                for j in range(y, y + h):
                    fabric[i][j] += 1

    # Get amount of squares with more than one request access
    count = sum(1 for i in chain.from_iterable(fabric) if i > 1)
    print(f"Part one count: {count}")

    # Loop through all requests
    for req in claims:
        # Get request data
        x, y, w, h = claims[req]
        for i in range(x, x + w):
            for j in range(y, y + h):
                # If not 1, another request overlaps
                if fabric[i][j] != 1:
                    break
            else:
                continue
            break
        # Both loops exited normally, match found
        else:
            print(f"Part two ID: {req}")
            return


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
