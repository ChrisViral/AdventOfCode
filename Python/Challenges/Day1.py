import sys


def main(args):
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Final result
    frequency = 0
    jumps = []
    frequencies = set()

    # Read file for frequency jumps
    with open(args[1], "r") as f:
        for line in f:
            jump: int = int(line)
            jumps.append(jump)
            frequency += jump
            frequencies.add(frequency)

    # Part one: final frequency
    print(f"Part one: {frequency}")

    # Loop through jumps until a duplicate is found
    found: bool = False
    while not found:
        for jump in jumps:
            frequency += jump
            # If found, exit
            if frequency in frequencies:
                found = True
                break

            # If not, just add it to the set
            frequencies.add(frequency)

    print(f"Part two: {frequency}")


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
