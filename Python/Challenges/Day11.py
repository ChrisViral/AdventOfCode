import sys
from typing import List, Tuple


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Setup data
    width: int = 301
    grid: List[List[int]] = [[0] * width for _ in range(width)]

    with open(args[1], "r") as f:
        serial: int = int(f.readline().strip())

    # Setup power in the grid
    for x in range(1, width):
        for y in range(1, width):
            rack: int = x + 10
            power: int = rack * y
            power += serial
            power *= rack
            power = (power // 100) % 10  # Extracting the third digit
            power -= 5
            grid[x][y] = power

    top: int = 0
    best: Tuple[int, int]
    for x in range(1, width - 3):
        for y in range(1, width - 3):
            curr: int = 0
            # Loop through increasing sizes
            for i in range(x, x + 3):
                for j in range(y, y + 3):
                    curr += grid[i][j]

            # Test size
            if curr > top:
                top = curr
                best = (x, y)

    print(f"Part one coordinates {best} with score {top}\n")

    top = 0
    best: Tuple[int, int, int]
    # Loop through top left coordinates
    for x in range(1, width):
        for y in range(1, width):
            curr: int = 0
            # Loop through increasing sizes
            for size in range(width - max(x, y)):
                # Add the vertical rightmost band
                i: int = x + size
                for j in range(y, y + size + 1):
                    curr += grid[i][j]

                # Add the horizontal bottom band
                j: int = y + size
                for i in range(x, x + size):
                    curr += grid[i][j]

                # Test size
                if curr > top:
                    top = curr
                    best = (x, y, size + 1)
                    print(f"Temporary new best {best} with score {top}")

    print(f"\nPart two coordinates {best} with score {top}")


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
