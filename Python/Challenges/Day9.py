import sys
from collections import deque
import re as regex
from typing import List, Deque


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Reading the file
    with open(args[1], "r") as f:
        player_count: int
        final_marble: int
        pattern: regex = regex.search(r"(\d+) players; last marble is worth (\d+) points", f.readline().strip())
        player_count, final_marble = map(int, pattern.groups())

    highscore: int = calculate_score(player_count, final_marble)
    print("Part one high score:", highscore)

    highscore = calculate_score(player_count, final_marble * 100)
    print("Part one high score:", highscore)


def calculate_score(player_count: int, final_marble: int) -> int:
    """
    Calculates the score for a given game
    :param player_count: Amount of players in the game
    :param final_marble: Final marble played
    :return: The highscore of the winning player
    """

    # Initialize the data
    scores: List[int] = [0] * player_count
    circle: Deque[int] = deque([0])
    highscore: int = 0

    # Loop from 1 to the final marble, inclusively
    for marble in range(1, final_marble + 1):
        if marble % 23 == 0:
            player: int = ((marble - 1) % player_count)
            circle.rotate(7)
            scores[player] += marble + circle.pop()
            circle.rotate(-1)
            highscore = max(highscore, scores[player])
        else:
            circle.rotate(-1)
            circle.append(marble)

    # Return highest score
    return highscore


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
