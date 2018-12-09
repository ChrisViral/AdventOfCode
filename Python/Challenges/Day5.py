import sys
import string
import re as regex
from typing import List, Tuple


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Get polymer chain
    with open(args[1], "r") as f:
        original: str = f.readline().strip()

    # Fully react the polymer
    polymer = fully_react(original)
    minimum: int = len(polymer)

    # Print results
    print("Part one polymer length:", minimum)

    # Try with each letter of the alphabet
    for c in string.ascii_lowercase:
        # Remove the letter from the original polymer, case insensitive, then test
        polymer = regex.sub(c, "", original, flags=regex.IGNORECASE)
        polymer = fully_react(polymer)
        minimum = min(minimum, len(polymer))

    # Print results
    print("Part two polymer length:", minimum)


def fully_react(polymer: str) -> str:
    """
    Fully reacts a polymer until it cannot react further
    :param polymer: Polymer chain to react
    :return: The fully reacted polymer chain
    """

    # React as long as possible
    reacting: bool = True
    while reacting:
        reacting, polymer = react(polymer)

    # When done, return the result
    return polymer


def react(polymer: str) -> Tuple[bool, str]:
    """
    Reacts the polymer string and removes the next polymer reaction if any is found
    :param polymer: Polymer string to react
    :return: True if a reaction occurred, false otherwise
    """

    # Can only react if there is 2 or more components
    if len(polymer) < 2:
        return False, polymer

    # Previous/Current character
    prev: str = polymer[0]

    # Loop through all the remaining characters
    for curr in polymer[1:]:
        # If the same character and case is different
        if prev.lower() == curr.lower() and prev.islower() != curr.islower():
            # Remove the character pair from the string and return
            return True, polymer.replace(prev + curr, "")

        # If no reaction, set current character as previous
        prev = curr

    # If completed normally, no replacements have been done
    return False, polymer


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
