import sys
from typing import List, Tuple, DefaultDict, Iterator
from collections import defaultdict


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Create the grid to simulate in
    grid: DefaultDict[int, DefaultDict[int, int]] = defaultdict(lambda: defaultdict(int))
    # Set the current closest point out of bounds and shortest trip to something ridiculous
    closest: int = 999999
    shortest: int = 999999

    # File read stub
    with open(args[1], "r") as f:
        # Each line of input is a wire, i is an id/index to know which one we're simulating
        for i, line in enumerate(f):
            # Starting position and step counter
            x: int = 0
            y: int = 0
            steps: int = 0
            # Parse all instructions for this wire
            for dx, dy, length in parse_instructions(line):
                # For the length of this instruction
                for _ in range(length):
                    # Move in the given direction
                    x += dx
                    y += dy
                    steps += 1
                    curr: int = grid[x][y]
                    # If first wire, set the current position to the amount of steps if not already done
                    if i == 0 and curr == 0:
                        grid[x][y] = steps
                    # For the second wire, if an amount is set, we're crossing the first pipe
                    elif i == 1 and curr != 0:
                        # Calculate the potentially new closest and shortest points
                        closest = min(closest, abs(x) + abs(y))
                        shortest = min(shortest, steps + curr)

    # Print the results
    print(closest)
    print(shortest)


def parse_instructions(instructions: str) -> Iterator[Tuple[int, int, int]]:
    # Splits the line into individual instructions
    for instruction in instructions.split(","):
        # Get direction and length
        direction: str = instruction[0]
        length: int = int(instruction[1:])
        # Yield the appropriate direction vector and length of movement
        if direction == "R":
            yield 1, 0, length
        elif direction == "L":
            yield -1, 0, length
        elif direction == "U":
            yield 0, 1, length
        elif direction == "D":
            yield 0, -1, length


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
