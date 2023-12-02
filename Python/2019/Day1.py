import sys
from typing import List


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Keep a record of the required fuel for each module
    modules: List[int] = []

    # File read stub
    with open(args[1], "r") as f:
        # Calculate the required mass for each module
        for mass in map(int, f):
            modules.append((mass // 3) - 2)

    # Calculate the total mass
    fuel: int = sum(modules)
    print(fuel)

    # For each amount of fuel, calculate the extra needed fuel
    for amount in modules:
        extra: int = (amount // 3) - 2
        while extra > 0:
            fuel += extra
            extra = (extra // 3) - 2
    print(fuel)


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
