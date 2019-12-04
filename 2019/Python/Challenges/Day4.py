import sys
from typing import List, Pattern
import re as regex


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # File read stub
    with open(args[1], "r") as f:
        # Get the max and min from the input
        a: int
        b: int
        a, b = tuple(map(int, f.readline().split("-")))

    # This pattern matches a string made of digits in increasing order
    increasing: Pattern = regex.compile(r"^1*2*3*4*5*6*7*8*9*$")
    # This pattern matches any digit repeated at least once
    multiple: Pattern = regex.compile(r"(\d)\1")
    # Start by finding all the numbers matching these requirements
    possible: List[str] = list(filter(lambda n: increasing.match(n) and multiple.search(n), map(str, range(a, b + 1))))
    # First answer is the length of that list+
    print(len(possible))

    # This pattern matches if the digit is only repeated twice
    pair: Pattern = regex.compile(r"(?:^|(.)(?!\1))(\d)\2(?!\2)")
    # Count how many satisfy this new condition
    print(sum(1 for n in possible if pair.search(n)))


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
