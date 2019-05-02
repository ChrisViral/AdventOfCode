import sys
import re as regex
from typing import List, Pattern


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    pattern: Pattern = regex.compile(r"\d+")

    # File read stub
    with open(args[1], "r") as f:
        spreadsheet: List[List[int]] = []
        for line in f:
            spreadsheet.append(list(map(int, pattern.findall(line))))

    checksum: int = 0
    for row in spreadsheet:
        max_value: int = 0
        min_value: int = 999999
        for n in row:
            max_value = max(max_value, n)
            min_value = min(min_value, n)

        checksum += max_value - min_value

    print(checksum)

    checksum = 0
    for row in spreadsheet:
        for i, a in enumerate(row[:-1]):
            for b in row[i + 1:]:
                if a % b == 0:
                    checksum += a // b
                elif b % a == 0:
                    checksum += b // a

    print(checksum)


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
