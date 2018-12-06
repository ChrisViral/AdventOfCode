import sys
import re
from itertools import chain


def main(args):
    """
    Application entry point
    :param args: Argument list, should contain the file to load
    """

    width = 1000
    fabric = [[0] * width for _ in range(width)]
    splitter = re.compile(r"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)")

    with open(args[1], "r") as f:
        for line in f:
            req, x, y, w, h = map(int, splitter.search(line).groups())

            for i in range(x, x + w):
                for j in range(y, y + h):
                    fabric[i][j] += 1

    count = sum(1 for i in chain.from_iterable(fabric) if i > 1)
    print(f"Part one count: {count}")


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
