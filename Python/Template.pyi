import sys


def main(args):
    """
    Application entry point
    :param args: Argument list, should contain the file to load
    """

    # File read stub
    with open(args[1], "r") as f:
        for line in f:
            pass


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
