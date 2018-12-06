import sys
from collections import Counter


def main(args):
    """
    Application entry point
    :param args: Argument list, should contain the file to load
    """

    twos = 0
    threes = 0
    ids = []

    # Read file
    with open(args[1], "r") as f:
        for line in f:
            ids.append(line)
            counts = set(Counter(line).values())
            if 2 in counts:
                twos += 1
            if 3 in counts:
                threes += 1

    print(f"Part one checksum: {twos * threes}")

    # While the list has more than one element
    while len(ids) > 1:
        # Get last element, keep the rest for the secondary loop
        first = ids.pop()
        # Get second element
        for second in ids:
            mismatch = False
            diff = ""
            # Check character by character
            for a, b in zip(first, second):
                # If mismatch
                if a != b:
                    # If a mismatch was already found, break out
                    if mismatch:
                        break
                    # Else raise mismatch flag
                    mismatch = True

                # If not mismatched, add character to the diff
                else:
                    diff += a

            # If only one mismatch, the target has been found
            else:
                if mismatch:
                    print("Part two diff: " + diff)
                    return


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
