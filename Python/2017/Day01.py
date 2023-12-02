import sys
from typing import List


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # File read stub
    with open(args[1], "r") as f:
        list: str = f.readline()
        list += list[0]

    captcha: int = 0
    for i, n in enumerate(list[:-1]):
        if n == list[i + 1]:
            captcha += int(n)

    print(captcha)

    captcha = 0
    jump: int = len(list) // 2
    for i, n in enumerate(list[:jump]):
        if n == list[i + jump]:
            captcha += int(n) * 2

    print(captcha)


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
